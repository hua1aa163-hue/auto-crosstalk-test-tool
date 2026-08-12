using AutoCrosstalk.Networking;
using AutoCrosstalk.Protocol;

namespace AutoCrosstalk.Automation;

/// <summary>
/// 依次完成：配置亮色度参数 → 切换图片 → 单次自动测试 → 等待完成。
/// </summary>
public sealed class BatchTestRunner(
    TcpMessageServer tcpServer,
    IDesktopDisplayService desktopDisplay)
{
    public event EventHandler<BatchProgressEventArgs>? ProgressChanged;
    public event EventHandler<string>? Log;

    public async Task RunAsync(BatchTestOptions options, CancellationToken cancellationToken)
    {
        if (!tcpServer.IsConnected)
        {
            throw new InvalidOperationException("TCP 尚未连接。");
        }

        if (options.ImageFiles.Count == 0)
        {
            throw new InvalidOperationException("没有可测试的图片。");
        }

        string? originalWallpaper = options.RestoreWallpaper
            ? desktopDisplay.GetCurrentWallpaper()
            : null;

        try
        {
            // Target 参数在整个批次开始时配置一次，避免重复创建同一测试项。
            WriteLog($"发送亮色度参数：{options.TargetMessage}");
            string targetResponse;
            try
            {
                targetResponse = await SendAndWaitAsync(
                    options.TargetMessage,
                    MeasurementProtocol.IsTargetResponse,
                    options.TargetTimeout,
                    cancellationToken);
            }
            catch (TimeoutException ex)
            {
                throw new InvalidOperationException("等待 Target 参数应答超时。", ex);
            }
            WriteLog($"参数应答：{targetResponse}");

            if (targetResponse != MeasurementProtocol.TargetSuccess)
            {
                throw new InvalidOperationException(
                    $"亮色度参数设置失败：{MeasurementProtocol.GetTargetError(targetResponse)}");
            }

            int processed = 0;
            for (int index = 0; index < options.ImageFiles.Count; index++)
            {
                string imagePath = options.ImageFiles[index];
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    Report(index, imagePath, BatchImageStatus.Switching, processed, "切换桌面图片");
                    desktopDisplay.SetWallpaper(imagePath);
                    WriteLog($"[{index + 1}/{options.ImageFiles.Count}] 已切换：{Path.GetFileName(imagePath)}");

                    // 参考 ZJS2310：设置每张壁纸后都重新应用一次所选桌面模式。
                    RefreshDesktopMode(options.Topology);

                    if (options.SwitchDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(options.SwitchDelay, cancellationToken);
                    }

                    Report(index, imagePath, BatchImageStatus.Testing, processed, "等待测试完成");
                    await RunSingleMeasurementAsync(options.MeasurementTimeout, cancellationToken);

                    processed++;
                    Report(index, imagePath, BatchImageStatus.Completed, processed, "测试完成");
                    WriteLog($"[{index + 1}/{options.ImageFiles.Count}] 测试完成：{Path.GetFileName(imagePath)}");
                }
                catch (OperationCanceledException)
                {
                    Report(index, imagePath, BatchImageStatus.Cancelled, processed, "已停止");
                    throw;
                }
                catch (Exception ex)
                {
                    processed++;
                    Report(index, imagePath, BatchImageStatus.Failed, processed, ex.Message);
                    WriteLog($"[{index + 1}/{options.ImageFiles.Count}] 失败：{ex.Message}");

                    // TCP 断开时继续切图没有意义，不受“失败后继续”选项影响。
                    bool canContinue = options.ContinueAfterFailure &&
                                       tcpServer.IsConnected &&
                                       ex is not IOException;
                    if (!canContinue)
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(originalWallpaper) && File.Exists(originalWallpaper))
            {
                try
                {
                    desktopDisplay.SetWallpaper(originalWallpaper);
                    WriteLog("已恢复测试前的桌面壁纸。");
                }
                catch (Exception ex)
                {
                    WriteLog($"恢复原桌面失败：{ex.Message}");
                }
            }
        }
    }

    private void RefreshDesktopMode(DisplayTopology topology)
    {
        if (topology != DisplayTopology.None)
        {
            desktopDisplay.ApplyTopology(topology);
            WriteLog($"已刷新桌面模式：{topology}");
        }
    }

    private async Task RunSingleMeasurementAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var completed = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnMessage(object? sender, string message)
        {
            if (!MeasurementProtocol.IsMeasurementResponse(message)) return;

            WriteLog($"测试应答：{message}");
            if (message == MeasurementProtocol.SingleAutoCompleted)
            {
                completed.TrySetResult(message);
            }
            else if (message.StartsWith("&|Meas|A|NG|", StringComparison.Ordinal))
            {
                completed.TrySetException(new InvalidOperationException($"测试返回失败：{message}"));
            }
        }

        void OnClosed(object? sender, string reason) =>
            completed.TrySetException(new IOException(reason));

        tcpServer.MessageReceived += OnMessage;
        tcpServer.ConnectionClosed += OnClosed;
        try
        {
            WriteLog($"发送测试命令：{MeasurementProtocol.SingleAutoMeasure}");
            await tcpServer.SendAsync(MeasurementProtocol.SingleAutoMeasure, cancellationToken);
            try
            {
                await completed.Task.WaitAsync(timeout, cancellationToken);
            }
            catch (TimeoutException ex)
            {
                throw new InvalidOperationException(
                    $"等待测试完成应答 {MeasurementProtocol.SingleAutoCompleted} 超时。", ex);
            }
        }
        finally
        {
            tcpServer.MessageReceived -= OnMessage;
            tcpServer.ConnectionClosed -= OnClosed;
        }
    }

    private async Task<string> SendAndWaitAsync(
        string outgoingMessage,
        Func<string, bool> responseFilter,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        var response = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnMessage(object? sender, string message)
        {
            if (responseFilter(message)) response.TrySetResult(message);
        }

        void OnClosed(object? sender, string reason) =>
            response.TrySetException(new IOException(reason));

        tcpServer.MessageReceived += OnMessage;
        tcpServer.ConnectionClosed += OnClosed;
        try
        {
            await tcpServer.SendAsync(outgoingMessage, cancellationToken);
            return await response.Task.WaitAsync(timeout, cancellationToken);
        }
        finally
        {
            tcpServer.MessageReceived -= OnMessage;
            tcpServer.ConnectionClosed -= OnClosed;
        }
    }

    private void Report(
        int imageIndex,
        string imagePath,
        BatchImageStatus status,
        int processedCount,
        string detail) =>
        ProgressChanged?.Invoke(this,
            new BatchProgressEventArgs(imageIndex, imagePath, status, processedCount, detail));

    private void WriteLog(string message) => Log?.Invoke(this, message);
}
