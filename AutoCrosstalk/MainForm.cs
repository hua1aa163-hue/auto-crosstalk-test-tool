using AutoCrosstalk.Models;
using AutoCrosstalk.Networking;
using AutoCrosstalk.Protocol;
using AutoCrosstalk.Settings;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace AutoCrosstalk;

public partial class MainForm : Form
{
    private const int ListenPort = 9527;
    private readonly TcpMessageServer _tcpServer = new();
    private readonly CancellationTokenSource _formCancellation = new();
    private string? _lastReceivedMessage;
    private CancellationTokenSource? _timedSendCancellation;
    private bool _isTimedSendRunning;
    private bool _isTimedSendSending;
    private bool _isClosing;

    public MainForm()
    {
        InitializeComponent();
        WireLivePreviewEvents();
        _tcpServer.ClientConnected += TcpServer_ClientConnected;
        _tcpServer.MessageReceived += TcpServer_MessageReceived;
        _tcpServer.ConnectionClosed += TcpServer_ConnectionClosed;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        ApplyMainSettings(AppSettingsStore.Load().MainForm);
        UpdateImageCardControls();
        UpdateEyePositionControls();
        UpdatePreview();
        await StartServerAsync();
    }





    private async void btnConnect_Click(object? sender, EventArgs e)
    {
        if (_tcpServer.IsListening)
        {
            StopTimedSend("TCP 服务器停止监听，定时发送已停止。");
            await _tcpServer.StopAsync();
            if (_isClosing) return;
            SetServerState(false, false, "监听已停止");
            AppendLog("TCP 服务器已停止监听。");
            return;
        }

        await StartServerAsync();
    }

    private async Task StartServerAsync()
    {
        btnConnect.Enabled = false;
        lblStatus.Text = "● 启动监听中...";
        lblStatus.ForeColor = Color.DarkOrange;

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
            MessageBox.Show(this, $"无法监听 127.0.0.1:{ListenPort}：\r\n{ex.Message}", "TCP 服务器",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnConfirm_Click(object? sender, EventArgs e)
    {
        if (!TryBuildMessage(out string message))
        {
            return;
        }

        txtSendPreview.Text = message;
        if (!_tcpServer.IsConnected)
        {
            MessageBox.Show(this, "报文已经生成，但尚无 TCP 客户端连接到本服务器。",
                "尚未发送", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        btnConfirm.Enabled = false;
        try
        {
            await _tcpServer.SendAsync(message, _formCancellation.Token);
            AppendLog($"发送：{message}");
        }
        catch (OperationCanceledException) when (_isClosing)
        {
            // 窗口关闭时取消未完成的发送，不再弹出错误框。
        }
        catch (Exception ex)
        {
            if (_isClosing) return;
            SetServerState(_tcpServer.IsListening, false, "客户端已断开");
            MessageBox.Show(this, $"发送失败：\r\n{ex.Message}", "TCP 发送",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!_isClosing) btnConfirm.Enabled = !_isTimedSendRunning;
        }
    }

    private async void btnTimedSend_Click(object? sender, EventArgs e)
    {
        if (_isTimedSendRunning)
        {
            StopTimedSend("定时发送已停止。");
            return;
        }

        if (!_tcpServer.IsConnected)
        {
            MessageBox.Show(this, "尚无 TCP 客户端连接，不能启动定时发送。",
                "定时发送", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!TryBuildMessage(out _, showErrors: true)) return;

        // WinForms Timer 使用毫秒，界面输入单位是分钟。
        timedSendTimer.Interval = decimal.ToInt32(numTimedSendInterval.Value * 60000m);
        var sendCancellation = new CancellationTokenSource();
        _timedSendCancellation = sendCancellation;
        SetTimedSendState(true);
        AppendLog($"开始定时发送，间隔 {numTimedSendInterval.Value} 分钟。");

        bool sent = await SendTimedMessageAsync(sendCancellation.Token);

        // 等待发送时可能已停止并重新启动，旧任务不能操作新计时器。
        if (!ReferenceEquals(_timedSendCancellation, sendCancellation)) return;

        if (!sent)
        {
            StopTimedSend("首次定时发送失败，定时发送已停止。");
            return;
        }

        if (_isTimedSendRunning) timedSendTimer.Start();
    }

    private async void timedSendTimer_Tick(object? sender, EventArgs e)
    {
        CancellationTokenSource? sendCancellation = _timedSendCancellation;
        if (!_isTimedSendRunning || sendCancellation is null || _isTimedSendSending) return;

        _isTimedSendSending = true;
        try
        {
            bool sent = await SendTimedMessageAsync(sendCancellation.Token);
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

    private async Task<bool> SendTimedMessageAsync(CancellationToken cancellationToken)
    {
        if (!_tcpServer.IsConnected) return false;

        // 用户编辑参数导致报文暂时不完整时跳过本轮，下一个周期重新读取界面。
        if (!TryBuildMessage(out string message, showErrors: false))
        {
            AppendLog("定时发送跳过：当前参数不完整或格式有误。");
            return true;
        }

        txtSendPreview.Text = message;
        try
        {
            await _tcpServer.SendAsync(message, cancellationToken);
            AppendLog($"定时发送：{message}");
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
        numTimedSendInterval.Enabled = !running;
        btnConfirm.Enabled = !running;
        btnBatchTest.Enabled = !running;
    }

    private bool TryBuildMessage(out string message, bool showErrors = true)
    {
        message = string.Empty;
        Control[] requiredControls =
        [
            txtTarget, txtTestName,
            rbLogical.Checked ? cmbLogicalCard : cmbBmpCard,
            cmbPointTemplate, txtRedExposure, txtGreenExposure,
            txtBlueExposure, txtGain, txtThreshold
        ];

        foreach (Control control in requiredControls)
        {
            if (string.IsNullOrWhiteSpace(control.Text))
            {
                if (showErrors) ShowValidationError(control, "此字段不能为空。");
                return false;
            }

            if (TestMessageCodec.ContainsReservedCharacter(control.Text))
            {
                if (showErrors)
                {
                    ShowValidationError(control, "字段不能包含 |、&、@ 或换行符。协议未定义这些字符的转义方式。");
                }
                return false;
            }
        }

        TextBox[] numericControls =
            [txtRedExposure, txtGreenExposure, txtBlueExposure, txtGain, txtThreshold];
        foreach (TextBox control in numericControls)
        {
            if (!decimal.TryParse(control.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                if (showErrors) ShowValidationError(control, "请输入有效数字；小数点请使用英文句点“.”。");
                return false;
            }
        }

        string eyePositions = NormalizeEyePositions(txtSpecifiedEyes.Text);
        if (rbSpecifiedEye.Checked && eyePositions.Length == 0)
        {
            if (showErrors) ShowValidationError(txtSpecifiedEyes, "选择指定眼位时，请至少输入一个眼位。");
            return false;
        }

        if (eyePositions.Length > 0 && !Regex.IsMatch(eyePositions, @"^\d+(,\d+)*$"))
        {
            if (showErrors) ShowValidationError(txtSpecifiedEyes, "眼位应为逗号分隔的整数，例如：5,9,13。");
            return false;
        }

        int testItem = GetSelectedTestItem();
        var data = new TestMessage
        {
            Target = txtTarget.Text.Trim(),
            Result = rbResultOk.Checked ? "OK" : "NG",
            TestItem = testItem,
            TestName = txtTestName.Text.Trim(),
            ImageType = rbLogical.Checked ? 0 : 1,
            CardName = (rbLogical.Checked ? cmbLogicalCard.Text : cmbBmpCard.Text).Trim(),
            PointTemplateName = cmbPointTemplate.Text.Trim(),
            ExposureMode = rbAutoExposure.Checked ? 0 : 1,
            RecognitionMode = rbAutoRecognition.Checked ? 0 : 1,
            RedExposure = txtRedExposure.Text.Trim(),
            GreenExposure = txtGreenExposure.Text.Trim(),
            BlueExposure = txtBlueExposure.Text.Trim(),
            Gain = txtGain.Text.Trim(),
            Threshold = txtThreshold.Text.Trim(),
            EyePositionMode = GetEyePositionMode(),
            SpecifiedEyePositions = eyePositions
        };

        message = TestMessageCodec.Serialize(data);
        return true;
    }

    private int GetSelectedTestItem()
    {
        string selected = cmbTestType.Text;
        int separator = selected.IndexOf('-');
        string number = separator >= 0 ? selected[..separator].Trim() : selected.Trim();
        return int.TryParse(number, out int value) ? value : cmbTestType.SelectedIndex;
    }

    private int GetEyePositionMode()
    {
        if (rbSpecifiedEye.Checked) return 0;
        if (rbCenterEye.Checked) return 1;
        if (rbFullEyeBox.Checked) return 2;
        if (rbUpperEyeBox.Checked) return 3;
        if (rbCenterEyeBox.Checked) return 4;
        return 5;
    }

    private void ApplyMainSettings(MainFormSettings settings)
    {
        txtTarget.Text = settings.Target ?? "Target";
        rbResultOk.Checked = settings.ResultOk;
        rbResultNg.Checked = !settings.ResultOk;

        cmbTestType.SelectedIndex = settings.TestTypeIndex >= 0 &&
                                    settings.TestTypeIndex < cmbTestType.Items.Count
            ? settings.TestTypeIndex
            : 1;
        txtTestName.Text = settings.TestName ?? "brightness";

        rbLogical.Checked = settings.LogicalImage;
        rbBmp.Checked = !settings.LogicalImage;
        cmbLogicalCard.Text = settings.LogicalCard ?? "White";
        cmbBmpCard.Text = settings.BmpCard ?? string.Empty;
        cmbPointTemplate.Text = settings.PointTemplate ?? "9";

        rbAutoExposure.Checked = settings.AutoExposure;
        rbManualExposure.Checked = !settings.AutoExposure;
        rbAutoRecognition.Checked = settings.AutoRecognition;
        rbThresholdRecognition.Checked = !settings.AutoRecognition;
        txtRedExposure.Text = settings.RedExposure ?? "84848";
        txtGreenExposure.Text = settings.GreenExposure ?? "74434";
        txtBlueExposure.Text = settings.BlueExposure ?? "143383";
        txtGain.Text = settings.Gain ?? "0";
        txtThreshold.Text = settings.Threshold ?? "0.1";
        txtSpecifiedEyes.Text = settings.SpecifiedEyes ?? "5,9,13";

        RadioButton[] eyeModes =
            [rbSpecifiedEye, rbCenterEye, rbFullEyeBox, rbUpperEyeBox, rbCenterEyeBox, rbLowerEyeBox];
        int eyeMode = Math.Clamp(settings.EyePositionMode, 0, eyeModes.Length - 1);
        eyeModes[eyeMode].Checked = true;
        numTimedSendInterval.Value = Math.Clamp(
            settings.TimedSendIntervalMinutes,
            numTimedSendInterval.Minimum,
            numTimedSendInterval.Maximum);
    }

    private MainFormSettings CaptureMainSettings() => new()
    {
        Target = txtTarget.Text,
        ResultOk = rbResultOk.Checked,
        TestTypeIndex = cmbTestType.SelectedIndex,
        TestName = txtTestName.Text,
        LogicalImage = rbLogical.Checked,
        LogicalCard = cmbLogicalCard.Text,
        BmpCard = cmbBmpCard.Text,
        PointTemplate = cmbPointTemplate.Text,
        AutoExposure = rbAutoExposure.Checked,
        AutoRecognition = rbAutoRecognition.Checked,
        RedExposure = txtRedExposure.Text,
        GreenExposure = txtGreenExposure.Text,
        BlueExposure = txtBlueExposure.Text,
        Gain = txtGain.Text,
        Threshold = txtThreshold.Text,
        EyePositionMode = GetEyePositionMode(),
        SpecifiedEyes = txtSpecifiedEyes.Text,
        TimedSendIntervalMinutes = numTimedSendInterval.Value
    };

    private static string NormalizeEyePositions(string value) =>
        value.Replace('，', ',').Replace(" ", string.Empty).Trim();

    private void TcpServer_ClientConnected(object? sender, string remoteEndPoint)
    {
        PostToUi(() =>
        {
            SetServerState(true, true, "客户端已连接");
            AppendLog($"客户端已连接：{remoteEndPoint}");
        });
    }

    private void TcpServer_MessageReceived(object? sender, string message)
    {
        PostToUi(() =>
        {
            _lastReceivedMessage = message;
            AppendLog($"接收：{message}");
        });
    }

    private void TcpServer_ConnectionClosed(object? sender, string reason)
    {
        PostToUi(() =>
        {
            if (_isTimedSendRunning)
            {
                StopTimedSend("客户端断开，定时发送已停止。");
            }
            SetServerState(_tcpServer.IsListening, false,
                _tcpServer.IsListening ? "等待客户端" : "监听已停止");
            AppendLog(reason);
        });
    }

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
            // 检查后窗口仍可能立即关闭，此时忽略过期的界面更新。
        }
    }

    private void btnLoadReceived_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_lastReceivedMessage))
        {
            MessageBox.Show(this, "尚未收到完整报文。", "载入报文",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!TestMessageCodec.TryDeserialize(_lastReceivedMessage, out TestMessage? data, out string error))
        {
            MessageBox.Show(this, error, "报文格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ApplyMessage(data!);
        UpdatePreview();
    }

    private void ApplyMessage(TestMessage data)
    {
        txtTarget.Text = data.Target;
        rbResultOk.Checked = string.Equals(data.Result, "OK", StringComparison.OrdinalIgnoreCase);
        rbResultNg.Checked = !rbResultOk.Checked;

        int matchingIndex = -1;
        for (int i = 0; i < cmbTestType.Items.Count; i++)
        {
            if (cmbTestType.Items[i]?.ToString()?.StartsWith($"{data.TestItem} ", StringComparison.Ordinal) == true)
            {
                matchingIndex = i;
                break;
            }
        }
        if (matchingIndex >= 0) cmbTestType.SelectedIndex = matchingIndex;

        txtTestName.Text = data.TestName;
        rbLogical.Checked = data.ImageType == 0;
        rbBmp.Checked = data.ImageType != 0;
        if (rbLogical.Checked) cmbLogicalCard.Text = data.CardName;
        else cmbBmpCard.Text = data.CardName;
        cmbPointTemplate.Text = data.PointTemplateName;
        rbAutoExposure.Checked = data.ExposureMode == 0;
        rbManualExposure.Checked = data.ExposureMode != 0;
        rbAutoRecognition.Checked = data.RecognitionMode == 0;
        rbThresholdRecognition.Checked = data.RecognitionMode != 0;
        txtRedExposure.Text = data.RedExposure;
        txtGreenExposure.Text = data.GreenExposure;
        txtBlueExposure.Text = data.BlueExposure;
        txtGain.Text = data.Gain;
        txtThreshold.Text = data.Threshold;
        txtSpecifiedEyes.Text = data.SpecifiedEyePositions;

        RadioButton[] eyeModes =
            [rbSpecifiedEye, rbCenterEye, rbFullEyeBox, rbUpperEyeBox, rbCenterEyeBox, rbLowerEyeBox];
        if (data.EyePositionMode is >= 0 and < 6)
        {
            eyeModes[data.EyePositionMode].Checked = true;
        }
    }

    private void SetServerState(bool listening, bool connected, string text)
    {
        lblStatus.Text = $"● {text}";
        lblStatus.ForeColor = connected ? Color.DarkGreen
            : listening ? Color.DarkOrange
            : Color.Maroon;
        btnConnect.Text = listening ? "停止监听" : "启动监听";
        txtHost.Enabled = false;
        nudPort.Enabled = false;
        btnConnect.Enabled = true;
    }

    private void AppendLog(string line)
    {
        txtReceiveLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}");
    }

    private void ShowValidationError(Control control, string message)
    {
        MessageBox.Show(this, message, "输入检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        if (control is TextBox textBox) textBox.SelectAll();
    }

    private void UpdatePreview()
    {
        txtSendPreview.Text = TryBuildMessage(out string message, showErrors: false)
            ? message
            : "参数尚未填写完整或格式有误，修正后将自动更新报文。";
    }

    /// <summary>所有协议参数变化时实时刷新报文预览，不在输入过程中弹出校验框。</summary>
    private void WireLivePreviewEvents()
    {
        Control[] textControls =
        [
            txtTarget, cmbTestType, txtTestName, cmbLogicalCard, cmbBmpCard,
            cmbPointTemplate, txtRedExposure, txtGreenExposure, txtBlueExposure,
            txtGain, txtThreshold, txtSpecifiedEyes
        ];
        foreach (Control control in textControls)
        {
            control.TextChanged += ParameterValueChanged;
        }

        RadioButton[] optionControls =
        [
            rbResultOk, rbResultNg, rbLogical, rbBmp,
            rbAutoExposure, rbManualExposure,
            rbAutoRecognition, rbThresholdRecognition,
            rbSpecifiedEye, rbCenterEye, rbFullEyeBox,
            rbUpperEyeBox, rbCenterEyeBox, rbLowerEyeBox
        ];
        foreach (RadioButton radioButton in optionControls)
        {
            radioButton.CheckedChanged += ParameterValueChanged;
        }
    }

    private void ParameterValueChanged(object? sender, EventArgs e) => UpdatePreview();

    private void ImageType_CheckedChanged(object? sender, EventArgs e) => UpdateImageCardControls();

    private void UpdateImageCardControls()
    {
        cmbLogicalCard.Enabled = rbLogical.Checked;
        btnEditLogical.Enabled = rbLogical.Checked;
        cmbBmpCard.Enabled = rbBmp.Checked;
        btnEditBmp.Enabled = rbBmp.Checked;
    }

    private void EyePosition_CheckedChanged(object? sender, EventArgs e) => UpdateEyePositionControls();

    private void UpdateEyePositionControls()
    {
        // 即使选择中心眼位，也保留指定眼位文本并写入报文，和参考示例一致。
        txtSpecifiedEyes.BackColor = rbSpecifiedEye.Checked ? Color.LightYellow : Color.White;
    }

    private void cmbTestType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        string[] names = ["Appearance", "brightness", "Chromaticity", "Uniformity"];
        if (cmbTestType.SelectedIndex is >= 0 and < 4)
        {
            txtTestName.Text = names[cmbTestType.SelectedIndex];
        }
    }

    private void btnRestoreDefaults_Click(object? sender, EventArgs e)
    {
        rbAutoExposure.Checked = true;
        rbThresholdRecognition.Checked = true;
        txtRedExposure.Text = "84848";
        txtGreenExposure.Text = "74434";
        txtBlueExposure.Text = "143383";
        txtGain.Text = "0";
        txtThreshold.Text = "0.1";
        UpdatePreview();
    }

    private void btnEditLogical_Click(object? sender, EventArgs e) => cmbLogicalCard.Focus();
    private void btnEditPointTemplate_Click(object? sender, EventArgs e) => cmbPointTemplate.Focus();
    private void btnEditBmp_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "选择 BMP 图卡",
            Filter = "BMP 图像 (*.bmp)|*.bmp|所有文件 (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            cmbBmpCard.Text = Path.GetFileName(dialog.FileName);
        }
    }
    private void btnClearLog_Click(object? sender, EventArgs e) => txtReceiveLog.Clear();

    private void btnBatchTest_Click(object? sender, EventArgs e)
    {
        using var batchForm = new BatchTestForm(_tcpServer, BuildTargetMessageForBatch);
        batchForm.ShowDialog(this);
    }

    private string? BuildTargetMessageForBatch() =>
        TryBuildMessage(out string message) ? message : null;

    private void btnCancel_Click(object? sender, EventArgs e) => Close();

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _isClosing = true;
        _formCancellation.Cancel();
        StopTimedSend();

        // 停止后台线程的界面回调，避免关闭期间访问已释放控件。
        _tcpServer.ClientConnected -= TcpServer_ClientConnected;
        _tcpServer.MessageReceived -= TcpServer_MessageReceived;
        _tcpServer.ConnectionClosed -= TcpServer_ConnectionClosed;
    }

    private async void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        try
        {
            AppSettings settings = AppSettingsStore.Load();
            settings.MainForm = CaptureMainSettings();
            AppSettingsStore.Save(settings);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存主界面设置失败：{ex.Message}");
        }

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
