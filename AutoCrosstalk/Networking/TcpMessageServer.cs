using System.Net;
using System.Net.Sockets;
using System.Text;
using AutoCrosstalk.Protocol;

namespace AutoCrosstalk.Networking;

/// <summary>
/// 单客户端 TCP 服务器。接收端按 &amp;| 和 |@ 自动处理粘包、拆包。
/// 客户端断开后服务器继续监听，等待下一次连接。
/// </summary>
public sealed class TcpMessageServer : IAsyncDisposable
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
        get { lock (_stateLock) return _client?.Connected == true && _stream is not null; }
    }

    public int Port { get; private set; }

    public event EventHandler<string>? ClientConnected;
    public event EventHandler<string>? MessageReceived;
    public event EventHandler<string>? ConnectionClosed;

    public Task StartAsync(
        IPAddress address,
        int port,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_stateLock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_listener is not null)
            {
                throw new InvalidOperationException("TCP 服务器已经启动。");
            }

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
            stream = _stream ?? throw new InvalidOperationException("尚无 TCP 客户端连接。");
        }

        byte[] data = Encoding.UTF8.GetBytes(message);
        await _sendLock.WaitAsync(cancellationToken);
        try
        {
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
                // listener.Stop() 用于唤醒 AcceptTcpClientAsync。
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

            string remoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "未知客户端";
            ClientConnected?.Invoke(this, remoteEndPoint);

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

            if (closeReason is not null)
            {
                ConnectionClosed?.Invoke(this, closeReason);
            }
        }
    }

    private async Task<string?> ReceiveLoopAsync(
        NetworkStream stream,
        CancellationToken cancellationToken)
    {
        var pending = new StringBuilder();

        try
        {
            using var reader = new StreamReader(
                stream, new UTF8Encoding(false), detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024, leaveOpen: true);
            var buffer = new char[1024];

            while (!cancellationToken.IsCancellationRequested)
            {
                int count = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                if (count == 0)
                {
                    return "TCP 客户端已断开，服务器继续等待连接。";
                }

                pending.Append(buffer, 0, count);
                ExtractCompleteMessages(pending);
                if (pending.Length > MaxPendingCharacters)
                {
                    return "客户端发送的报文超过 1 MB，连接已关闭。";
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
            int start = text.IndexOf(TestMessageCodec.StartMarker, StringComparison.Ordinal);
            if (start < 0)
            {
                if (pending.Length > 0 && pending[^1] == '&')
                {
                    pending.Clear().Append('&');
                }
                else
                {
                    pending.Clear();
                }
                return;
            }

            if (start > 0)
            {
                pending.Remove(0, start);
                text = pending.ToString();
            }

            int end = text.IndexOf(TestMessageCodec.EndMarker, TestMessageCodec.StartMarker.Length,
                StringComparison.Ordinal);
            if (end < 0) return;

            int messageLength = end + TestMessageCodec.EndMarker.Length;
            string message = text[..messageLength];
            pending.Remove(0, messageLength);
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

        // 等待已进入或排队的发送结束，再释放发送锁。
        await _sendLock.WaitAsync();
        _sendLock.Release();
        _sendLock.Dispose();
    }
}
