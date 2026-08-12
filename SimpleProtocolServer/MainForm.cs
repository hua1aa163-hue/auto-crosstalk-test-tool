using System.Net;
using SimpleProtocolServer.Networking;
using SimpleProtocolServer.Protocol;

namespace SimpleProtocolServer;

public partial class MainForm : Form
{
    private const int ListenPort = 9527;
    private readonly TcpMessageServer _tcpServer = new();
    private readonly CancellationTokenSource _formCancellation = new();
    private readonly CycleMessageSequence _cycleMessages = new();
    private CancellationTokenSource? _timedSendCancellation;
    private bool _isTimedSendRunning;
    private bool _isTimedSendSending;
    private bool _isClosing;

    public MainForm()
    {
        InitializeComponent();
        _tcpServer.ClientConnected += TcpServer_ClientConnected;
        _tcpServer.MessageReceived += TcpServer_MessageReceived;
        _tcpServer.ConnectionClosed += TcpServer_ConnectionClosed;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        cmbCommand.SelectedIndex = 0;
        await StartServerAsync();
    }

    private async void btnListen_Click(object? sender, EventArgs e)
    {
        btnListen.Enabled = false;
        try
        {
            if (_tcpServer.IsListening)
            {
                StopTimedSend("TCP 服务器停止监听，定时发送已停止。");
                await _tcpServer.StopAsync();
                if (_isClosing) return;

                SetServerState(false, false, "未监听");
                AppendLog("TCP 服务器已停止监听。");
            }
            else
            {
                await StartServerAsync();
            }
        }
        catch (Exception ex)
        {
            if (!_isClosing)
            {
                MessageBox.Show(this, ex.Message, "TCP 服务器",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            if (!_isClosing) btnListen.Enabled = true;
        }
    }

    private async Task StartServerAsync()
    {
        btnListen.Enabled = false;
        SetServerState(false, false, "启动监听中...");
        try
        {
            await _tcpServer.StartAsync(IPAddress.Loopback, ListenPort);
            if (_isClosing) return;

            SetServerState(true, false, "等待客户端");
            AppendLog($"TCP 服务器正在监听 127.0.0.1:{ListenPort}");
        }
        catch (Exception ex)
        {
            if (_isClosing) return;

            SetServerState(false, false, "监听失败");
            MessageBox.Show(this, $"无法监听 127.0.0.1:{ListenPort}：\r\n{ex.Message}",
                "TCP 服务器", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!_isClosing) btnListen.Enabled = true;
        }
    }

    private void cmbCommand_SelectedIndexChanged(object? sender, EventArgs e)
    {
        CommandType command = GetSelectedCommand();
        bool needsParameter = SimpleMessageProtocol.RequiresParameter(command);

        lblParameter.Text = SimpleMessageProtocol.GetParameterLabel(command);
        txtParameter.Enabled = needsParameter;
        txtParameter.Text = SimpleMessageProtocol.GetDefaultParameter(command);
        lblParameterHint.Text = needsParameter
            ? command == CommandType.GetFocalLength
                ? "输入大于 0 的 VID 整数"
                : "输入不含 |、&、@ 的配方名称"
            : "此命令不需要参数";
        txtExpectedResponse.Text = SimpleMessageProtocol.GetExpectedResponse(command);
        UpdatePreview();
    }

    private void txtParameter_TextChanged(object? sender, EventArgs e) => UpdatePreview();

    private void btnAddCycle_Click(object? sender, EventArgs e)
    {
        if (!TryGetCurrentMessage(out string message, showError: true)) return;

        int index = lstCycleMessages.Items.Add(message);
        lstCycleMessages.SelectedIndex = index;
        UpdateCycleHint();
    }

    private void btnRemoveCycle_Click(object? sender, EventArgs e)
    {
        int index = lstCycleMessages.SelectedIndex;
        if (index < 0) return;

        lstCycleMessages.Items.RemoveAt(index);
        if (lstCycleMessages.Items.Count > 0)
        {
            lstCycleMessages.SelectedIndex = Math.Min(index, lstCycleMessages.Items.Count - 1);
        }
        UpdateCycleHint();
    }

    private void btnClearCycle_Click(object? sender, EventArgs e)
    {
        lstCycleMessages.Items.Clear();
        UpdateCycleHint();
    }

    private void lstCycleMessages_DoubleClick(object? sender, EventArgs e)
    {
        if (lstCycleMessages.SelectedItem is string message)
        {
            txtSendPreview.Text = message;
        }
    }

    private void UpdateCycleHint() =>
        lblCycleHint.Text = $"已添加 {lstCycleMessages.Items.Count} 条；定时发送将按顺序循环执行";

    private async void btnSendOnce_Click(object? sender, EventArgs e)
    {
        if (!TryGetCurrentMessage(out string message, showError: true)) return;
        if (!EnsureClientConnected()) return;

        btnSendOnce.Enabled = false;
        try
        {
            await _tcpServer.SendAsync(message, _formCancellation.Token);
            AppendLog($"发送：{message}");
        }
        catch (OperationCanceledException) when (_isClosing)
        {
            // 窗口关闭时取消发送，不再弹出错误框。
        }
        catch (Exception ex)
        {
            if (!_isClosing)
            {
                MessageBox.Show(this, ex.Message, "发送失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            if (!_isClosing) btnSendOnce.Enabled = !_isTimedSendRunning;
        }
    }

    private async void btnTimedSend_Click(object? sender, EventArgs e)
    {
        if (_isTimedSendRunning)
        {
            StopTimedSend("定时发送已停止。");
            return;
        }

        if (!EnsureClientConnected()) return;
        if (lstCycleMessages.Items.Count == 0)
        {
            MessageBox.Show(this, "请先把至少一条报文加入循环列表。",
                "循环列表为空", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        timedSendTimer.Interval = decimal.ToInt32(numIntervalMinutes.Value * 60000m);
        _cycleMessages.Reset(lstCycleMessages.Items.Cast<string>());
        var sendCancellation = new CancellationTokenSource();
        _timedSendCancellation = sendCancellation;
        SetTimedSendState(true);
        AppendLog($"开始定时发送，间隔 {numIntervalMinutes.Value} 分钟。");

        bool sent = await SendNextCycleMessageAsync(sendCancellation.Token);

        // 旧的异步发送结束时，不能操作后来重新启动的计时器。
        if (!ReferenceEquals(_timedSendCancellation, sendCancellation)) return;

        if (!sent)
        {
            StopTimedSend("首次发送失败，定时发送已停止。");
            return;
        }

        timedSendTimer.Start();
    }

    private async void timedSendTimer_Tick(object? sender, EventArgs e)
    {
        CancellationTokenSource? sendCancellation = _timedSendCancellation;
        if (!_isTimedSendRunning || sendCancellation is null || _isTimedSendSending) return;

        _isTimedSendSending = true;
        try
        {
            bool sent = await SendNextCycleMessageAsync(sendCancellation.Token);
            if (!sent && ReferenceEquals(_timedSendCancellation, sendCancellation))
            {
                StopTimedSend("定时发送失败，定时发送已停止。");
            }
        }
        finally
        {
            if (ReferenceEquals(_timedSendCancellation, sendCancellation))
            {
                _isTimedSendSending = false;
            }
        }
    }

    private async Task<bool> SendNextCycleMessageAsync(CancellationToken cancellationToken)
    {
        if (!_tcpServer.IsConnected) return false;
        if (!_cycleMessages.TryGetNext(out string message, out int position)) return false;

        try
        {
            await _tcpServer.SendAsync(message, cancellationToken);
            AppendLog($"定时发送 [{position}/{_cycleMessages.Count}]：{message}");
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        catch (Exception ex)
        {
            if (!_isClosing) AppendLog($"定时发送失败：{ex.Message}");
            return false;
        }
    }

    private void StopTimedSend(string? logMessage = null)
    {
        bool wasRunning = _isTimedSendRunning;
        timedSendTimer.Stop();
        _isTimedSendSending = false;

        CancellationTokenSource? cancellation = _timedSendCancellation;
        _timedSendCancellation = null;
        cancellation?.Cancel();
        cancellation?.Dispose();

        SetTimedSendState(false);
        if (wasRunning && !string.IsNullOrEmpty(logMessage)) AppendLog(logMessage);
    }

    private void SetTimedSendState(bool running)
    {
        _isTimedSendRunning = running;
        btnTimedSend.Text = running ? "停止定时发送" : "开始定时发送";
        btnSendOnce.Enabled = !running;
        numIntervalMinutes.Enabled = !running;
        btnAddCycle.Enabled = !running;
        btnRemoveCycle.Enabled = !running;
        btnClearCycle.Enabled = !running;
        lstCycleMessages.Enabled = !running;
        lblSendState.Text = running
            ? $"定时发送中：{_cycleMessages.Count} 条报文，每 {numIntervalMinutes.Value} 分钟一次"
            : "定时发送未启动";
        lblSendState.ForeColor = running ? Color.DarkGreen : Color.DimGray;
    }

    private bool TryGetCurrentMessage(out string message, bool showError)
    {
        bool success = SimpleMessageProtocol.TryValidateMessage(
            txtSendPreview.Text, out message, out string error);

        if (!success && showError)
        {
            MessageBox.Show(this, error, "参数检查",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSendPreview.Focus();
        }
        return success;
    }

    private void UpdatePreview()
    {
        bool success = SimpleMessageProtocol.TryBuildMessage(
            GetSelectedCommand(), txtParameter.Text, out string message, out string error);
        txtSendPreview.Text = success ? message : error;
    }

    private CommandType GetSelectedCommand() =>
        (CommandType)Math.Clamp(cmbCommand.SelectedIndex, 0, 7);

    private bool EnsureClientConnected()
    {
        if (_tcpServer.IsConnected) return true;

        MessageBox.Show(this, "尚无 TCP 客户端连接到 127.0.0.1:9527。",
            "尚未连接", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return false;
    }

    private void TcpServer_ClientConnected(object? sender, string remoteEndPoint) =>
        PostToUi(() =>
        {
            SetServerState(true, true, "客户端已连接");
            AppendLog($"客户端已连接：{remoteEndPoint}");
        });

    private void TcpServer_MessageReceived(object? sender, string message) =>
        PostToUi(() => AppendLog($"接收：{message}"));

    private void TcpServer_ConnectionClosed(object? sender, string reason) =>
        PostToUi(() =>
        {
            StopTimedSend("客户端断开，定时发送已停止。");
            SetServerState(_tcpServer.IsListening, false,
                _tcpServer.IsListening ? "等待客户端" : "未监听");
            AppendLog(reason);
        });

    /// <summary>安全地将 TCP 后台事件切回界面线程。</summary>
    private void PostToUi(Action action)
    {
        if (_isClosing || IsDisposed || Disposing || !IsHandleCreated) return;

        try
        {
            BeginInvoke(() =>
            {
                if (!_isClosing && !IsDisposed) action();
            });
        }
        catch (InvalidOperationException)
        {
            // 窗口正在关闭时忽略过期的界面更新。
        }
    }

    private void SetServerState(bool listening, bool connected, string text)
    {
        lblStatus.Text = $"● {text}";
        lblStatus.ForeColor = connected ? Color.DarkGreen
            : listening ? Color.DarkOrange
            : Color.Maroon;
        btnListen.Text = listening ? "停止监听" : "启动监听";
    }

    private void AppendLog(string text) =>
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");

    private void btnClearLog_Click(object? sender, EventArgs e) => txtLog.Clear();

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _isClosing = true;
        _formCancellation.Cancel();
        StopTimedSend();
        _tcpServer.ClientConnected -= TcpServer_ClientConnected;
        _tcpServer.MessageReceived -= TcpServer_MessageReceived;
        _tcpServer.ConnectionClosed -= TcpServer_ConnectionClosed;
    }

    private async void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        try
        {
            await _tcpServer.DisposeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"关闭 TCP 服务器失败：{ex.Message}");
        }
        finally
        {
            _formCancellation.Dispose();
        }
    }
}
