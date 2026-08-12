using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleProtocolServer.Protocol;

namespace SimpleProtocolServer.Networking;

/// <summary>
/// 单客户端 TCP 服务器。客户端断开后会继续监听，并按 |@ 拆分完整报文。
/// </summary>
internal sealed class TcpMessageServer : IAsyncDisposable
{
    private const int MaxPendingCharacters = 1024 * 1024;
    private readonly object _stateLock = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private TcpListener? _listener;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private CancellationTokenSource? _serverCancellation;
    private Task? _acceptTask;
    private bool _disposed;

    public bool IsListening
    {
        get { lock (_stateLock) return _listener is not null; }
    }

    public bool IsConnected
    {
        get { lock (_stateLock) return _stream is not null; }
    }

    public int Port { get; private set; }

    public event EventHandler<string>? ClientConnected;
    public event EventHandler<string>? MessageReceived;
    public event EventHandler<string>? ConnectionClosed;

    public Task StartAsync(IPAddress address, int port)
    {
        lock (_stateLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_listener is not null) throw new InvalidOperationException("TCP 服务器已在监听。");

            var listener = new TcpListener(address, port);
            listener.Start(1);

            _listener = listener;
            Port = ((IPEndPoint)listener.LocalEndpoint).Port;
            _serverCancellation = new CancellationTokenSource();
            _acceptTask = AcceptLoopAsync(listener, _serverCancellation.Token);
        }

        return Task.CompletedTask;
    }

    public async Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        NetworkStream stream;
        lock (_stateLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            stream = _stream ?? throw new InvalidOperationException("尚无 TCP 客户端连接。");
        }

        byte[] data = Encoding.UTF8.GetBytes(message);
        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            // 排队期间如果客户端已更换，不把旧报文发给新客户端。
            lock (_stateLock)
            {
                if (!ReferenceEquals(_stream, stream))
                {
                    throw new IOException("客户端连接已更换，本次报文未发送。");
                }
            }

            await stream.WriteAsync(data, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public async Task StopAsync()
    {
        CancellationTokenSource? cancellation;
        TcpListener? listener;
        TcpClient? client;
        Task? acceptTask;

        lock (_stateLock)
        {
            cancellation = _serverCancellation;
            listener = _listener;
            client = _client;
            acceptTask = _acceptTask;

            _serverCancellation = null;
            _listener = null;
            _client = null;
            _stream = null;
            _acceptTask = null;
            Port = 0;
        }

        cancellation?.Cancel();
        listener?.Stop();
        client?.Dispose();

        if (acceptTask is not null)
        {
            try
            {
                await acceptTask;
            }
            catch (OperationCanceledException)
            {
                // 主动停止监听。
            }
            catch (SocketException) when (cancellation?.IsCancellationRequested == true)
            {
                // listener.Stop() 用于唤醒正在等待的 Accept。
            }
        }

        cancellation?.Dispose();
    }

    private async Task AcceptLoopAsync(TcpListener listener, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await listener.AcceptTcpClientAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            client.NoDelay = true;
            NetworkStream stream = client.GetStream();
            lock (_stateLock)
            {
                if (!ReferenceEquals(_listener, listener))
                {
                    stream.Dispose();
                    client.Dispose();
                    break;
                }

                _client = client;
                _stream = stream;
            }

            ClientConnected?.Invoke(this, client.Client.RemoteEndPoint?.ToString() ?? "未知客户端");
            string? closeReason = await ReceiveLoopAsync(stream, cancellationToken);

            lock (_stateLock)
            {
                if (ReferenceEquals(_client, client))
                {
                    _client = null;
                    _stream = null;
                }
            }

            stream.Dispose();
            client.Dispose();
            if (closeReason is not null) ConnectionClosed?.Invoke(this, closeReason);
        }
    }

    private async Task<string?> ReceiveLoopAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        var pending = new StringBuilder();

        try
        {
            using var reader = new StreamReader(
                stream, new UTF8Encoding(false), false, 1024, leaveOpen: true);
            var buffer = new char[1024];

            while (!cancellationToken.IsCancellationRequested)
            {
                int count = await reader.ReadAsync(buffer.AsMemory(), cancellationToken);
                if (count == 0) return "TCP 客户端已断开，服务器继续等待连接。";

                pending.Append(buffer, 0, count);
                ExtractCompleteMessages(pending);
                if (pending.Length > MaxPendingCharacters)
                {
                    return "客户端报文超过 1 MB，连接已关闭。";
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 主动停止服务器。
        }
        catch (Exception ex)
        {
            return $"客户端连接已关闭：{ex.Message}";
        }

        return null;
    }

    private void ExtractCompleteMessages(StringBuilder pending)
    {
        while (true)
        {
            string text = pending.ToString();
            int start = text.IndexOf(SimpleMessageProtocol.StartMarker, StringComparison.Ordinal);
            if (start < 0)
            {
                pending.Clear();
                if (text.EndsWith('&')) pending.Append('&');
                return;
            }

            if (start > 0)
            {
                pending.Remove(0, start);
                text = pending.ToString();
            }

            int end = text.IndexOf(SimpleMessageProtocol.EndMarker,
                SimpleMessageProtocol.StartMarker.Length, StringComparison.Ordinal);
            if (end < 0) return;

            int length = end + SimpleMessageProtocol.EndMarker.Length;
            string message = text[..length];
            pending.Remove(0, length);
            MessageReceived?.Invoke(this, message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        lock (_stateLock)
        {
            if (_disposed) return;
            _disposed = true;
        }

        await StopAsync();
        await _sendLock.WaitAsync();
        _sendLock.Release();
        _sendLock.Dispose();
    }
}
