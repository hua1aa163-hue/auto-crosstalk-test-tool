using AutoCrosstalk.Automation;
using AutoCrosstalk.Models;
using AutoCrosstalk.Networking;
using AutoCrosstalk.Protocol;
using AutoCrosstalk.Settings;

namespace AutoCrosstalk;

public partial class BatchTestForm : Form
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".png", ".bmp", ".jpg", ".jpeg" };

    private readonly IDesktopDisplayService _desktopDisplay = new DesktopDisplayService();
    private TcpMessageServer? _tcpServer;
    private Func<string?>? _buildTargetMessage;
    private BatchTestRunner? _runner;
    private CancellationTokenSource? _batchCancellation;
    private List<string> _imageFiles = [];
    private int _desktopTestIndex = -1;
    private bool _isTimedProjectionRunning;
    private bool _isRunning;

    /// <summary>保留无参数构造函数，使 Visual Studio 设计器可以直接打开此窗体。</summary>
    public BatchTestForm()
    {
        InitializeComponent();
    }

    internal BatchTestForm(TcpMessageServer tcpServer, Func<string?> buildTargetMessage)
        : this()
    {
        _tcpServer = tcpServer;
        _buildTargetMessage = buildTargetMessage;
        _runner = new BatchTestRunner(tcpServer, _desktopDisplay);
        _runner.Log += Runner_Log;
        _runner.ProgressChanged += Runner_ProgressChanged;
    }

    private void BatchTestForm_Load(object? sender, EventArgs e)
    {
        ApplyBatchSettings(AppSettingsStore.Load().BatchForm);
        RefreshImageList(showError: false);
    }

    private void btnBrowseDirectory_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "选择包含测试图片的目录",
            SelectedPath = Directory.Exists(txtImageDirectory.Text) ? txtImageDirectory.Text : string.Empty,
            ShowNewFolderButton = false
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            txtImageDirectory.Text = dialog.SelectedPath;
            RefreshImageList(showError: true);
        }
    }

    private void btnRefreshImages_Click(object? sender, EventArgs e) => RefreshImageList(showError: true);

    private void btnTestDesktop_Click(object? sender, EventArgs e) =>
        SwitchToNextDesktopImage(showError: true);

    private bool SwitchToNextDesktopImage(bool showError)
    {
        if (_imageFiles.Count == 0)
        {
            RefreshImageList(showError);
            if (_imageFiles.Count == 0) return false;
        }

        _desktopTestIndex = (_desktopTestIndex + 1) % _imageFiles.Count;
        string imagePath = _imageFiles[_desktopTestIndex];

        try
        {
            _desktopDisplay.SetWallpaper(imagePath);
            DisplayTopology topology = GetSelectedTopology();
            if (topology != DisplayTopology.None)
            {
                _desktopDisplay.ApplyTopology(topology);
            }

            ShowPreview(imagePath);
            lvImages.Items[_desktopTestIndex].Selected = true;
            lvImages.Items[_desktopTestIndex].EnsureVisible();
            lblCurrentImage.Text = $"当前图片：{Path.GetFileName(imagePath)}";
            AppendLog($"桌面测试切换成功：{Path.GetFileName(imagePath)}");
            return true;
        }
        catch (Exception ex)
        {
            AppendLog($"桌面测试切换失败：{ex.Message}");
            if (showError)
            {
                MessageBox.Show(this, ex.Message, "桌面切换失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }

    private void btnTimedProjection_Click(object? sender, EventArgs e)
    {
        if (_isTimedProjectionRunning)
        {
            StopTimedProjection("定时投图已停止。");
            return;
        }

        if (_isRunning) return;
        if (_imageFiles.Count == 0)
        {
            RefreshImageList(showError: true);
            if (_imageFiles.Count == 0) return;
        }

        timedProjectionTimer.Interval = checked((int)numTimedInterval.Value * 1000);
        SetTimedProjectionState(true);
        AppendLog($"开始定时投图：共 {_imageFiles.Count} 张，间隔 {numTimedInterval.Value} 秒。");

        if (!SwitchToNextDesktopImage(showError: true))
        {
            StopTimedProjection("定时投图因切图失败而停止。");
            return;
        }

        timedProjectionTimer.Start();
    }

    private void timedProjectionTimer_Tick(object? sender, EventArgs e)
    {
        if (!SwitchToNextDesktopImage(showError: false))
        {
            StopTimedProjection("定时投图因切图失败而停止。");
        }
    }

    private void StopTimedProjection(string? logMessage = null)
    {
        timedProjectionTimer.Stop();
        SetTimedProjectionState(false);
        if (!string.IsNullOrEmpty(logMessage)) AppendLog(logMessage);
    }

    private void SetTimedProjectionState(bool running)
    {
        _isTimedProjectionRunning = running;
        btnTimedProjection.Text = running ? "停止定时投图" : "开始定时投图";
        numTimedInterval.Enabled = !running;
        grpSettings.Enabled = !running && !_isRunning;
        btnRefreshImages.Enabled = !running && !_isRunning;
        btnTestDesktop.Enabled = !running && !_isRunning;
        btnStart.Enabled = !running && !_isRunning;
    }

    private bool RefreshImageList(bool showError)
    {
        string directory = txtImageDirectory.Text.Trim();
        if (!Directory.Exists(directory))
        {
            _imageFiles = [];
            lvImages.Items.Clear();
            lblBatchStatus.Text = "图片目录不存在";
            if (showError)
            {
                MessageBox.Show(this, "图片目录不存在，请重新选择。", "图片目录",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }

        try
        {
            _imageFiles = FindImageFiles(directory);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _imageFiles = [];
            lvImages.Items.Clear();
            lblBatchStatus.Text = "读取图片目录失败";
            AppendLog($"读取图片目录失败：{ex.Message}");
            if (showError)
            {
                MessageBox.Show(this, ex.Message, "读取图片目录失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        // 刷新或更换目录后，下次从第一张图开始。
        _desktopTestIndex = -1;

        lvImages.BeginUpdate();
        try
        {
            lvImages.Items.Clear();
            for (int index = 0; index < _imageFiles.Count; index++)
            {
                var item = new ListViewItem((index + 1).ToString());
                item.SubItems.Add(Path.GetFileName(_imageFiles[index]));
                item.SubItems.Add(GetStatusText(BatchImageStatus.Waiting));
                item.Tag = _imageFiles[index];
                lvImages.Items.Add(item);
            }
        }
        finally
        {
            lvImages.EndUpdate();
        }

        progressBatch.Maximum = Math.Max(1, _imageFiles.Count);
        progressBatch.Value = 0;
        grpImages.Text = $"测试图片（{_imageFiles.Count} 张）";
        lblBatchStatus.Text = $"已加载 {_imageFiles.Count} 张图片";
        if (_imageFiles.Count > 0)
        {
            lvImages.Items[0].Selected = true;
        }

        if (showError && _imageFiles.Count == 0)
        {
            MessageBox.Show(this, "当前目录没有找到 PNG、BMP、JPG 或 JPEG 图片。",
                "图片数量", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        return _imageFiles.Count > 0;
    }

    internal static List<string> FindImageFiles(string directory) =>
        Directory.EnumerateFiles(directory)
            .Where(path => SupportedExtensions.Contains(Path.GetExtension(path)))
            .OrderBy(path => Path.GetFileName(path), StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    private async void btnStart_Click(object? sender, EventArgs e)
    {
        if (_runner is null || _tcpServer is null || _buildTargetMessage is null)
        {
            MessageBox.Show(this, "请从主界面的“批量亮色度测试”按钮打开此窗口。",
                "无法开始", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!_tcpServer.IsConnected)
        {
            MessageBox.Show(this, "尚无 TCP 客户端连接，请先让客户端连接 127.0.0.1:9527。", "无法开始",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!RefreshImageList(showError: true))
        {
            return;
        }

        string? targetMessage = _buildTargetMessage();
        if (string.IsNullOrEmpty(targetMessage))
        {
            return;
        }

        if (!TestMessageCodec.TryDeserialize(targetMessage, out TestMessage? data, out string error) ||
            data is null)
        {
            MessageBox.Show(this, $"亮色度参数报文无效：{error}", "参数检查",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (data.Target != "Target" || data.Result != "OK" || data.TestItem != 1)
        {
            MessageBox.Show(this,
                "批量流程要求：Target 字段为 Target、勾选结果为 OK、测试类型为 1 - 亮度。",
                "参数检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ResetStatuses();
        txtBatchLog.Clear();
        SetRunningState(true);
        _batchCancellation = new CancellationTokenSource();

        var options = new BatchTestOptions
        {
            ImageFiles = _imageFiles,
            TargetMessage = targetMessage,
            Topology = GetSelectedTopology(),
            SwitchDelay = TimeSpan.FromMilliseconds((double)numSwitchDelay.Value),
            TargetTimeout = TimeSpan.FromSeconds(10),
            MeasurementTimeout = TimeSpan.FromSeconds((double)numMeasurementTimeout.Value),
            ContinueAfterFailure = chkContinueAfterFailure.Checked,
            RestoreWallpaper = chkRestoreWallpaper.Checked
        };

        try
        {
            await _runner.RunAsync(options, _batchCancellation.Token);
            lblBatchStatus.Text = $"{_imageFiles.Count} 张图片测试完成";
            MessageBox.Show(this, $"{_imageFiles.Count} 张图片的亮色度测试已全部完成。", "批量测试",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            lblBatchStatus.Text = "测试已停止";
            AppendLog("用户停止了批量测试。");
        }
        catch (Exception ex)
        {
            lblBatchStatus.Text = "批量测试失败";
            AppendLog($"批量测试停止：{ex.Message}");
            MessageBox.Show(this, ex.Message, "批量测试失败",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _batchCancellation.Dispose();
            _batchCancellation = null;
            SetRunningState(false);
        }
    }

    private void btnStop_Click(object? sender, EventArgs e)
    {
        btnStop.Enabled = false;
        lblBatchStatus.Text = "正在停止...";
        _batchCancellation?.Cancel();
    }

    private void btnApplyTopology_Click(object? sender, EventArgs e)
    {
        try
        {
            DisplayTopology topology = GetSelectedTopology();
            if (topology == DisplayTopology.None)
            {
                MessageBox.Show(this, "请选择需要应用的投影模式。", "投影模式",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _desktopDisplay.ApplyTopology(topology);
            AppendLog($"已应用投影模式：{cmbTopology.Text}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "切换投影模式失败",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private DisplayTopology GetSelectedTopology() => cmbTopology.SelectedIndex switch
    {
        1 => DisplayTopology.Internal,
        2 => DisplayTopology.Clone,
        3 => DisplayTopology.External,
        4 => DisplayTopology.Extend,
        _ => DisplayTopology.None
    };

    private void ApplyBatchSettings(BatchFormSettings settings)
    {
        txtImageDirectory.Text = settings.ImageDirectory ?? @"D:\work_file\串扰\全屏 20260717";
        cmbTopology.SelectedIndex = settings.TopologyIndex >= 0 &&
                                    settings.TopologyIndex < cmbTopology.Items.Count
            ? settings.TopologyIndex
            : 4;
        SetNumericValue(numSwitchDelay, settings.SwitchDelayMilliseconds);
        SetNumericValue(numMeasurementTimeout, settings.MeasurementTimeoutSeconds);
        SetNumericValue(numTimedInterval, settings.TimedIntervalSeconds);
        chkRestoreWallpaper.Checked = settings.RestoreWallpaper;
        chkContinueAfterFailure.Checked = settings.ContinueAfterFailure;
    }

    private BatchFormSettings CaptureBatchSettings() => new()
    {
        ImageDirectory = txtImageDirectory.Text,
        TopologyIndex = cmbTopology.SelectedIndex,
        SwitchDelayMilliseconds = numSwitchDelay.Value,
        MeasurementTimeoutSeconds = numMeasurementTimeout.Value,
        RestoreWallpaper = chkRestoreWallpaper.Checked,
        ContinueAfterFailure = chkContinueAfterFailure.Checked,
        TimedIntervalSeconds = numTimedInterval.Value
    };

    private static void SetNumericValue(NumericUpDown control, decimal value) =>
        control.Value = Math.Clamp(value, control.Minimum, control.Maximum);

    private void SaveBatchSettings()
    {
        AppSettings settings = AppSettingsStore.Load();
        settings.BatchForm = CaptureBatchSettings();
        AppSettingsStore.Save(settings);
    }

    private void Runner_Log(object? sender, string message)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => AppendLog(message));
        }
        else
        {
            AppendLog(message);
        }
    }

    private void Runner_ProgressChanged(object? sender, BatchProgressEventArgs e)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyProgress(e));
        }
        else
        {
            ApplyProgress(e);
        }
    }

    private void ApplyProgress(BatchProgressEventArgs e)
    {
        if (e.ImageIndex < 0 || e.ImageIndex >= lvImages.Items.Count) return;

        ListViewItem item = lvImages.Items[e.ImageIndex];
        item.SubItems[2].Text = GetStatusText(e.Status);
        item.Selected = true;
        item.EnsureVisible();
        progressBatch.Value = Math.Clamp(e.ProcessedCount, 0, progressBatch.Maximum);
        lblCurrentImage.Text = $"当前图片：{Path.GetFileName(e.ImagePath)}";
        lblBatchStatus.Text = $"{e.ImageIndex + 1}/{_imageFiles.Count} {e.Detail}";
        ShowPreview(e.ImagePath);
    }

    private void lvImages_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lvImages.SelectedItems.Count == 1 && lvImages.SelectedItems[0].Tag is string path)
        {
            ShowPreview(path);
        }
    }

    private void ShowPreview(string imagePath)
    {
        try
        {
            using Image source = Image.FromFile(imagePath);
            int previewWidth = Math.Min(600, source.Width);
            int previewHeight = Math.Max(1, source.Height * previewWidth / source.Width);
            Image preview = new Bitmap(source, previewWidth, previewHeight);
            Image? old = picPreview.Image;
            picPreview.Image = preview;
            old?.Dispose();
        }
        catch
        {
            // 预览失败不阻止实际测试，切换桌面时会给出准确错误。
        }
    }

    private void ResetStatuses()
    {
        foreach (ListViewItem item in lvImages.Items)
        {
            item.SubItems[2].Text = GetStatusText(BatchImageStatus.Waiting);
        }
        progressBatch.Value = 0;
    }

    private static string GetStatusText(BatchImageStatus status) => status switch
    {
        BatchImageStatus.Waiting => "等待",
        BatchImageStatus.Switching => "切换图片",
        BatchImageStatus.Testing => "测试中",
        BatchImageStatus.Completed => "完成",
        BatchImageStatus.Failed => "失败",
        BatchImageStatus.Cancelled => "已停止",
        _ => status.ToString()
    };

    private void SetRunningState(bool running)
    {
        if (running && _isTimedProjectionRunning)
        {
            StopTimedProjection("定时投图已停止，开始批量测试。");
        }

        _isRunning = running;
        grpSettings.Enabled = !running;
        btnRefreshImages.Enabled = !running;
        btnTestDesktop.Enabled = !running;
        btnTimedProjection.Enabled = !running;
        numTimedInterval.Enabled = !running;
        btnStart.Enabled = !running;
        btnStop.Enabled = running;
        btnClose.Enabled = !running;
    }

    private void AppendLog(string message) =>
        txtBatchLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");

    private void btnClose_Click(object? sender, EventArgs e) => Close();

    private void BatchTestForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isRunning)
        {
            e.Cancel = true;
            MessageBox.Show(this, "请先点击“停止”，等待当前流程结束后再关闭窗口。", "测试正在运行",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_isTimedProjectionRunning) StopTimedProjection();
        try
        {
            SaveBatchSettings();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存批量界面设置失败：{ex.Message}");
        }
    }

}
