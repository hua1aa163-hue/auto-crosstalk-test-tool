#nullable disable

namespace AutoCrosstalk;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        grpTcp = new GroupBox();
        lblStatus = new Label();
        btnConnect = new Button();
        nudPort = new NumericUpDown();
        lblPort = new Label();
        txtHost = new TextBox();
        lblHost = new Label();
        txtTarget = new TextBox();
        lblTarget = new Label();
        pnlLeft = new Panel();
        btnBatchTest = new Button();
        lblTimedSendInterval = new Label();
        numTimedSendInterval = new NumericUpDown();
        btnTimedSend = new Button();
        btnCancel = new Button();
        btnConfirm = new Button();
        btnEditPointTemplate = new Button();
        cmbPointTemplate = new ComboBox();
        lblPointTemplate = new Label();
        grpImageCard = new GroupBox();
        btnEditBmp = new Button();
        btnEditLogical = new Button();
        cmbBmpCard = new ComboBox();
        cmbLogicalCard = new ComboBox();
        rbBmp = new RadioButton();
        rbLogical = new RadioButton();
        grpResult = new GroupBox();
        rbResultNg = new RadioButton();
        rbResultOk = new RadioButton();
        txtTestName = new TextBox();
        lblTestName = new Label();
        cmbTestType = new ComboBox();
        lblTestType = new Label();
        pnlRight = new Panel();
        grpCommunication = new GroupBox();
        btnLoadReceived = new Button();
        btnClearLog = new Button();
        txtReceiveLog = new TextBox();
        lblReceive = new Label();
        txtSendPreview = new TextBox();
        lblSendPreview = new Label();
        grpEyePosition = new GroupBox();
        rbLowerEyeBox = new RadioButton();
        rbCenterEyeBox = new RadioButton();
        rbUpperEyeBox = new RadioButton();
        rbFullEyeBox = new RadioButton();
        rbCenterEye = new RadioButton();
        txtSpecifiedEyes = new TextBox();
        rbSpecifiedEye = new RadioButton();
        grpRecognition = new GroupBox();
        btnRestoreDefaults = new Button();
        txtThreshold = new TextBox();
        lblThreshold = new Label();
        txtGain = new TextBox();
        lblGain = new Label();
        txtBlueExposure = new TextBox();
        lblBlueExposure = new Label();
        txtGreenExposure = new TextBox();
        lblGreenExposure = new Label();
        txtRedExposure = new TextBox();
        lblRedExposure = new Label();
        pnlRecognitionMode = new Panel();
        rbThresholdRecognition = new RadioButton();
        rbAutoRecognition = new RadioButton();
        pnlExposureMode = new Panel();
        rbManualExposure = new RadioButton();
        rbAutoExposure = new RadioButton();
        timedSendTimer = new System.Windows.Forms.Timer(components);
        grpTcp.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
        pnlLeft.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numTimedSendInterval).BeginInit();
        grpImageCard.SuspendLayout();
        grpResult.SuspendLayout();
        pnlRight.SuspendLayout();
        grpCommunication.SuspendLayout();
        grpEyePosition.SuspendLayout();
        grpRecognition.SuspendLayout();
        pnlRecognitionMode.SuspendLayout();
        pnlExposureMode.SuspendLayout();
        SuspendLayout();
        // 
        // grpTcp
        // 
        grpTcp.Controls.Add(lblStatus);
        grpTcp.Controls.Add(btnConnect);
        grpTcp.Controls.Add(nudPort);
        grpTcp.Controls.Add(lblPort);
        grpTcp.Controls.Add(txtHost);
        grpTcp.Controls.Add(lblHost);
        grpTcp.Controls.Add(txtTarget);
        grpTcp.Controls.Add(lblTarget);
        grpTcp.Location = new Point(10, 7);
        grpTcp.Name = "grpTcp";
        grpTcp.Size = new Size(1145, 67);
        grpTcp.TabIndex = 0;
        grpTcp.TabStop = false;
        grpTcp.Text = "TCP 服务器";
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.ForeColor = Color.Maroon;
        lblStatus.Location = new Point(1020, 24);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(91, 27);
        lblStatus.TabIndex = 7;
        lblStatus.Text = "● 未启动";
        // 
        // btnConnect
        // 
        btnConnect.BackColor = Color.WhiteSmoke;
        btnConnect.Location = new Point(842, 20);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(150, 40);
        btnConnect.TabIndex = 6;
        btnConnect.Text = "启动监听";
        btnConnect.UseVisualStyleBackColor = false;
        btnConnect.Click += btnConnect_Click;
        // 
        // nudPort
        // 
        nudPort.Location = new Point(681, 24);
        nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudPort.Name = "nudPort";
        nudPort.ReadOnly = true;
        nudPort.Size = new Size(94, 33);
        nudPort.TabIndex = 5;
        nudPort.Value = new decimal(new int[] { 9527, 0, 0, 0 });
        // 
        // lblPort
        // 
        lblPort.AutoSize = true;
        lblPort.Location = new Point(633, 28);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(52, 27);
        lblPort.TabIndex = 4;
        lblPort.Text = "端口";
        // 
        // txtHost
        // 
        txtHost.Location = new Point(429, 24);
        txtHost.Name = "txtHost";
        txtHost.ReadOnly = true;
        txtHost.Size = new Size(187, 33);
        txtHost.TabIndex = 3;
        txtHost.Text = "127.0.0.1";
        // 
        // lblHost
        // 
        lblHost.AutoSize = true;
        lblHost.Location = new Point(362, 28);
        lblHost.Name = "lblHost";
        lblHost.Size = new Size(76, 27);
        lblHost.TabIndex = 2;
        lblHost.Text = "监听 IP";
        // 
        // txtTarget
        // 
        txtTarget.Location = new Point(86, 24);
        txtTarget.Name = "txtTarget";
        txtTarget.Size = new Size(247, 33);
        txtTarget.TabIndex = 1;
        txtTarget.Text = "Target";
        // 
        // lblTarget
        // 
        lblTarget.AutoSize = true;
        lblTarget.Location = new Point(17, 28);
        lblTarget.Name = "lblTarget";
        lblTarget.Size = new Size(73, 27);
        lblTarget.TabIndex = 0;
        lblTarget.Text = "Target";
        // 
        // pnlLeft
        // 
        pnlLeft.BackColor = Color.FromArgb(156, 185, 212);
        pnlLeft.BorderStyle = BorderStyle.FixedSingle;
        pnlLeft.Controls.Add(btnBatchTest);
        pnlLeft.Controls.Add(lblTimedSendInterval);
        pnlLeft.Controls.Add(numTimedSendInterval);
        pnlLeft.Controls.Add(btnTimedSend);
        pnlLeft.Controls.Add(btnCancel);
        pnlLeft.Controls.Add(btnConfirm);
        pnlLeft.Controls.Add(btnEditPointTemplate);
        pnlLeft.Controls.Add(cmbPointTemplate);
        pnlLeft.Controls.Add(lblPointTemplate);
        pnlLeft.Controls.Add(grpImageCard);
        pnlLeft.Controls.Add(grpResult);
        pnlLeft.Controls.Add(txtTestName);
        pnlLeft.Controls.Add(lblTestName);
        pnlLeft.Controls.Add(cmbTestType);
        pnlLeft.Controls.Add(lblTestType);
        pnlLeft.Location = new Point(21, 109);
        pnlLeft.Name = "pnlLeft";
        pnlLeft.Size = new Size(365, 688);
        pnlLeft.TabIndex = 1;
        // 
        // btnBatchTest
        // 
        btnBatchTest.BackColor = Color.FromArgb(225, 245, 229);
        btnBatchTest.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
        btnBatchTest.Location = new Point(8, 442);
        btnBatchTest.Name = "btnBatchTest";
        btnBatchTest.Size = new Size(347, 64);
        btnBatchTest.TabIndex = 9;
        btnBatchTest.Text = "批量亮色度测试";
        btnBatchTest.UseVisualStyleBackColor = false;
        btnBatchTest.Click += btnBatchTest_Click;
        // 
        // lblTimedSendInterval
        // 
        lblTimedSendInterval.AutoSize = true;
        lblTimedSendInterval.Location = new Point(8, 516);
        lblTimedSendInterval.Name = "lblTimedSendInterval";
        lblTimedSendInterval.Size = new Size(132, 27);
        lblTimedSendInterval.TabIndex = 10;
        lblTimedSendInterval.Text = "发送间隔(分钟)";
        // 
        // numTimedSendInterval
        // 
        numTimedSendInterval.DecimalPlaces = 1;
        numTimedSendInterval.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        numTimedSendInterval.Location = new Point(140, 511);
        numTimedSendInterval.Maximum = new decimal(new int[] { 1440, 0, 0, 0 });
        numTimedSendInterval.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
        numTimedSendInterval.Name = "numTimedSendInterval";
        numTimedSendInterval.Size = new Size(72, 33);
        numTimedSendInterval.TabIndex = 11;
        numTimedSendInterval.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // btnTimedSend
        // 
        btnTimedSend.BackColor = Color.FromArgb(255, 246, 215);
        btnTimedSend.Location = new Point(218, 510);
        btnTimedSend.Name = "btnTimedSend";
        btnTimedSend.Size = new Size(137, 35);
        btnTimedSend.TabIndex = 12;
        btnTimedSend.Text = "开始定时发送";
        btnTimedSend.UseVisualStyleBackColor = false;
        btnTimedSend.Click += btnTimedSend_Click;
        // 
        // btnCancel
        // 
        btnCancel.BackColor = Color.WhiteSmoke;
        btnCancel.Font = new Font("Microsoft YaHei UI", 18F);
        btnCancel.Location = new Point(8, 616);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(347, 61);
        btnCancel.TabIndex = 14;
        btnCancel.Text = "取消";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;
        // 
        // btnConfirm
        // 
        btnConfirm.BackColor = Color.White;
        btnConfirm.Font = new Font("Microsoft YaHei UI", 18F);
        btnConfirm.Location = new Point(8, 546);
        btnConfirm.Name = "btnConfirm";
        btnConfirm.Size = new Size(347, 63);
        btnConfirm.TabIndex = 13;
        btnConfirm.Text = "确定（发送）";
        btnConfirm.UseVisualStyleBackColor = false;
        btnConfirm.Click += btnConfirm_Click;
        // 
        // btnEditPointTemplate
        // 
        btnEditPointTemplate.BackColor = Color.WhiteSmoke;
        btnEditPointTemplate.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        btnEditPointTemplate.Location = new Point(316, 354);
        btnEditPointTemplate.Name = "btnEditPointTemplate";
        btnEditPointTemplate.Size = new Size(36, 34);
        btnEditPointTemplate.TabIndex = 8;
        btnEditPointTemplate.Text = "✎";
        btnEditPointTemplate.UseVisualStyleBackColor = false;
        btnEditPointTemplate.Click += btnEditPointTemplate_Click;
        // 
        // cmbPointTemplate
        // 
        cmbPointTemplate.FormattingEnabled = true;
        cmbPointTemplate.Items.AddRange(new object[] { "9", "1", "5", "13" });
        cmbPointTemplate.Location = new Point(123, 356);
        cmbPointTemplate.Name = "cmbPointTemplate";
        cmbPointTemplate.Size = new Size(184, 35);
        cmbPointTemplate.TabIndex = 7;
        cmbPointTemplate.Text = "9";
        // 
        // lblPointTemplate
        // 
        lblPointTemplate.AutoSize = true;
        lblPointTemplate.Location = new Point(21, 359);
        lblPointTemplate.Name = "lblPointTemplate";
        lblPointTemplate.Size = new Size(92, 27);
        lblPointTemplate.TabIndex = 6;
        lblPointTemplate.Text = "点模板：";
        // 
        // grpImageCard
        // 
        grpImageCard.Controls.Add(btnEditBmp);
        grpImageCard.Controls.Add(btnEditLogical);
        grpImageCard.Controls.Add(cmbBmpCard);
        grpImageCard.Controls.Add(cmbLogicalCard);
        grpImageCard.Controls.Add(rbBmp);
        grpImageCard.Controls.Add(rbLogical);
        grpImageCard.Location = new Point(11, 190);
        grpImageCard.Name = "grpImageCard";
        grpImageCard.Size = new Size(341, 143);
        grpImageCard.TabIndex = 5;
        grpImageCard.TabStop = false;
        grpImageCard.Text = "图卡模板";
        // 
        // btnEditBmp
        // 
        btnEditBmp.BackColor = Color.WhiteSmoke;
        btnEditBmp.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        btnEditBmp.Location = new Point(304, 86);
        btnEditBmp.Name = "btnEditBmp";
        btnEditBmp.Size = new Size(36, 34);
        btnEditBmp.TabIndex = 5;
        btnEditBmp.Text = "↗";
        btnEditBmp.UseVisualStyleBackColor = false;
        btnEditBmp.Click += btnEditBmp_Click;
        // 
        // btnEditLogical
        // 
        btnEditLogical.BackColor = Color.WhiteSmoke;
        btnEditLogical.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
        btnEditLogical.Location = new Point(304, 34);
        btnEditLogical.Name = "btnEditLogical";
        btnEditLogical.Size = new Size(36, 34);
        btnEditLogical.TabIndex = 2;
        btnEditLogical.Text = "✎";
        btnEditLogical.UseVisualStyleBackColor = false;
        btnEditLogical.Click += btnEditLogical_Click;
        // 
        // cmbBmpCard
        // 
        cmbBmpCard.FormattingEnabled = true;
        cmbBmpCard.Items.AddRange(new object[] { "Pattern.bmp", "Checker.bmp" });
        cmbBmpCard.Location = new Point(142, 89);
        cmbBmpCard.Name = "cmbBmpCard";
        cmbBmpCard.Size = new Size(155, 35);
        cmbBmpCard.TabIndex = 4;
        // 
        // cmbLogicalCard
        // 
        cmbLogicalCard.FormattingEnabled = true;
        cmbLogicalCard.Items.AddRange(new object[] { "White", "Black", "Red", "Green", "Blue" });
        cmbLogicalCard.Location = new Point(142, 37);
        cmbLogicalCard.Name = "cmbLogicalCard";
        cmbLogicalCard.Size = new Size(155, 35);
        cmbLogicalCard.TabIndex = 1;
        cmbLogicalCard.Text = "White";
        // 
        // rbBmp
        // 
        rbBmp.AutoSize = true;
        rbBmp.Location = new Point(15, 91);
        rbBmp.Name = "rbBmp";
        rbBmp.Size = new Size(102, 31);
        rbBmp.TabIndex = 3;
        rbBmp.Text = "BMP图";
        rbBmp.UseVisualStyleBackColor = true;
        rbBmp.CheckedChanged += ImageType_CheckedChanged;
        // 
        // rbLogical
        // 
        rbLogical.AutoSize = true;
        rbLogical.Checked = true;
        rbLogical.Location = new Point(15, 39);
        rbLogical.Name = "rbLogical";
        rbLogical.Size = new Size(124, 31);
        rbLogical.TabIndex = 0;
        rbLogical.TabStop = true;
        rbLogical.Text = "Logical图";
        rbLogical.UseVisualStyleBackColor = true;
        rbLogical.CheckedChanged += ImageType_CheckedChanged;
        // 
        // grpResult
        // 
        grpResult.Controls.Add(rbResultNg);
        grpResult.Controls.Add(rbResultOk);
        grpResult.Location = new Point(11, 124);
        grpResult.Name = "grpResult";
        grpResult.Size = new Size(341, 60);
        grpResult.TabIndex = 4;
        grpResult.TabStop = false;
        grpResult.Text = "勾选结果";
        // 
        // rbResultNg
        // 
        rbResultNg.AutoSize = true;
        rbResultNg.Location = new Point(192, 24);
        rbResultNg.Name = "rbResultNg";
        rbResultNg.Size = new Size(68, 31);
        rbResultNg.TabIndex = 1;
        rbResultNg.Text = "NG";
        rbResultNg.UseVisualStyleBackColor = true;
        // 
        // rbResultOk
        // 
        rbResultOk.AutoSize = true;
        rbResultOk.Checked = true;
        rbResultOk.Location = new Point(65, 24);
        rbResultOk.Name = "rbResultOk";
        rbResultOk.Size = new Size(66, 31);
        rbResultOk.TabIndex = 0;
        rbResultOk.TabStop = true;
        rbResultOk.Text = "OK";
        rbResultOk.UseVisualStyleBackColor = true;
        // 
        // txtTestName
        // 
        txtTestName.Location = new Point(123, 82);
        txtTestName.Name = "txtTestName";
        txtTestName.Size = new Size(229, 33);
        txtTestName.TabIndex = 3;
        txtTestName.Text = "brightness";
        // 
        // lblTestName
        // 
        lblTestName.AutoSize = true;
        lblTestName.Location = new Point(21, 85);
        lblTestName.Name = "lblTestName";
        lblTestName.Size = new Size(112, 27);
        lblTestName.TabIndex = 2;
        lblTestName.Text = "测试名称：";
        // 
        // cmbTestType
        // 
        cmbTestType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTestType.FormattingEnabled = true;
        cmbTestType.Items.AddRange(new object[] { "0 - 外观", "1 - 亮度", "2 - 色度", "3 - 均匀性" });
        cmbTestType.Location = new Point(123, 31);
        cmbTestType.Name = "cmbTestType";
        cmbTestType.Size = new Size(229, 35);
        cmbTestType.TabIndex = 1;
        cmbTestType.SelectedIndexChanged += cmbTestType_SelectedIndexChanged;
        // 
        // lblTestType
        // 
        lblTestType.AutoSize = true;
        lblTestType.Location = new Point(21, 34);
        lblTestType.Name = "lblTestType";
        lblTestType.Size = new Size(112, 27);
        lblTestType.TabIndex = 0;
        lblTestType.Text = "测试类型：";
        // 
        // pnlRight
        // 
        pnlRight.BackColor = Color.FromArgb(156, 185, 212);
        pnlRight.BorderStyle = BorderStyle.FixedSingle;
        pnlRight.Controls.Add(grpCommunication);
        pnlRight.Controls.Add(grpEyePosition);
        pnlRight.Controls.Add(grpRecognition);
        pnlRight.Location = new Point(439, 109);
        pnlRight.Name = "pnlRight";
        pnlRight.Size = new Size(682, 824);
        pnlRight.TabIndex = 2;
        // 
        // grpCommunication
        // 
        grpCommunication.Controls.Add(btnLoadReceived);
        grpCommunication.Controls.Add(btnClearLog);
        grpCommunication.Controls.Add(txtReceiveLog);
        grpCommunication.Controls.Add(lblReceive);
        grpCommunication.Controls.Add(txtSendPreview);
        grpCommunication.Controls.Add(lblSendPreview);
        grpCommunication.Location = new Point(7, 438);
        grpCommunication.Name = "grpCommunication";
        grpCommunication.Size = new Size(666, 366);
        grpCommunication.TabIndex = 2;
        grpCommunication.TabStop = false;
        grpCommunication.Text = "报文预览 / 接收";
        // 
        // btnLoadReceived
        // 
        btnLoadReceived.BackColor = Color.WhiteSmoke;
        btnLoadReceived.Location = new Point(508, 320);
        btnLoadReceived.Name = "btnLoadReceived";
        btnLoadReceived.Size = new Size(140, 40);
        btnLoadReceived.TabIndex = 5;
        btnLoadReceived.Text = "载入最后报文";
        btnLoadReceived.UseVisualStyleBackColor = false;
        btnLoadReceived.Click += btnLoadReceived_Click;
        // 
        // btnClearLog
        // 
        btnClearLog.BackColor = Color.WhiteSmoke;
        btnClearLog.Location = new Point(415, 320);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(75, 40);
        btnClearLog.TabIndex = 4;
        btnClearLog.Text = "清空";
        btnClearLog.UseVisualStyleBackColor = false;
        btnClearLog.Click += btnClearLog_Click;
        // 
        // txtReceiveLog
        // 
        txtReceiveLog.BackColor = Color.White;
        txtReceiveLog.Location = new Point(100, 131);
        txtReceiveLog.Multiline = true;
        txtReceiveLog.Name = "txtReceiveLog";
        txtReceiveLog.ReadOnly = true;
        txtReceiveLog.ScrollBars = ScrollBars.Vertical;
        txtReceiveLog.Size = new Size(548, 187);
        txtReceiveLog.TabIndex = 3;
        // 
        // lblReceive
        // 
        lblReceive.AutoSize = true;
        lblReceive.Location = new Point(2, 134);
        lblReceive.Name = "lblReceive";
        lblReceive.Size = new Size(92, 27);
        lblReceive.TabIndex = 2;
        lblReceive.Text = "接收日志";
        // 
        // txtSendPreview
        // 
        txtSendPreview.BackColor = Color.White;
        txtSendPreview.Location = new Point(100, 26);
        txtSendPreview.Multiline = true;
        txtSendPreview.Name = "txtSendPreview";
        txtSendPreview.ReadOnly = true;
        txtSendPreview.ScrollBars = ScrollBars.Vertical;
        txtSendPreview.Size = new Size(548, 99);
        txtSendPreview.TabIndex = 1;
        // 
        // lblSendPreview
        // 
        lblSendPreview.AutoSize = true;
        lblSendPreview.Location = new Point(2, 26);
        lblSendPreview.Name = "lblSendPreview";
        lblSendPreview.Size = new Size(92, 27);
        lblSendPreview.TabIndex = 0;
        lblSendPreview.Text = "发送报文";
        // 
        // grpEyePosition
        // 
        grpEyePosition.Controls.Add(rbLowerEyeBox);
        grpEyePosition.Controls.Add(rbCenterEyeBox);
        grpEyePosition.Controls.Add(rbUpperEyeBox);
        grpEyePosition.Controls.Add(rbFullEyeBox);
        grpEyePosition.Controls.Add(rbCenterEye);
        grpEyePosition.Controls.Add(txtSpecifiedEyes);
        grpEyePosition.Controls.Add(rbSpecifiedEye);
        grpEyePosition.Location = new Point(7, 282);
        grpEyePosition.Name = "grpEyePosition";
        grpEyePosition.Size = new Size(666, 149);
        grpEyePosition.TabIndex = 1;
        grpEyePosition.TabStop = false;
        grpEyePosition.Text = "眼位选择";
        // 
        // rbLowerEyeBox
        // 
        rbLowerEyeBox.AutoSize = true;
        rbLowerEyeBox.Location = new Point(491, 101);
        rbLowerEyeBox.Name = "rbLowerEyeBox";
        rbLowerEyeBox.Size = new Size(97, 31);
        rbLowerEyeBox.TabIndex = 6;
        rbLowerEyeBox.Text = "下眼盒";
        rbLowerEyeBox.UseVisualStyleBackColor = true;
        rbLowerEyeBox.CheckedChanged += EyePosition_CheckedChanged;
        // 
        // rbCenterEyeBox
        // 
        rbCenterEyeBox.AutoSize = true;
        rbCenterEyeBox.Location = new Point(339, 101);
        rbCenterEyeBox.Name = "rbCenterEyeBox";
        rbCenterEyeBox.Size = new Size(117, 31);
        rbCenterEyeBox.TabIndex = 5;
        rbCenterEyeBox.Text = "中心眼盒";
        rbCenterEyeBox.UseVisualStyleBackColor = true;
        rbCenterEyeBox.CheckedChanged += EyePosition_CheckedChanged;
        // 
        // rbUpperEyeBox
        // 
        rbUpperEyeBox.AutoSize = true;
        rbUpperEyeBox.Location = new Point(190, 101);
        rbUpperEyeBox.Name = "rbUpperEyeBox";
        rbUpperEyeBox.Size = new Size(97, 31);
        rbUpperEyeBox.TabIndex = 4;
        rbUpperEyeBox.Text = "上眼盒";
        rbUpperEyeBox.UseVisualStyleBackColor = true;
        rbUpperEyeBox.CheckedChanged += EyePosition_CheckedChanged;
        // 
        // rbFullEyeBox
        // 
        rbFullEyeBox.AutoSize = true;
        rbFullEyeBox.Location = new Point(405, 60);
        rbFullEyeBox.Name = "rbFullEyeBox";
        rbFullEyeBox.Size = new Size(97, 31);
        rbFullEyeBox.TabIndex = 3;
        rbFullEyeBox.Text = "全眼盒";
        rbFullEyeBox.UseVisualStyleBackColor = true;
        rbFullEyeBox.CheckedChanged += EyePosition_CheckedChanged;
        // 
        // rbCenterEye
        // 
        rbCenterEye.AutoSize = true;
        rbCenterEye.Checked = true;
        rbCenterEye.Location = new Point(257, 60);
        rbCenterEye.Name = "rbCenterEye";
        rbCenterEye.Size = new Size(117, 31);
        rbCenterEye.TabIndex = 2;
        rbCenterEye.TabStop = true;
        rbCenterEye.Text = "中心眼位";
        rbCenterEye.UseVisualStyleBackColor = true;
        rbCenterEye.CheckedChanged += EyePosition_CheckedChanged;
        // 
        // txtSpecifiedEyes
        // 
        txtSpecifiedEyes.Location = new Point(380, 25);
        txtSpecifiedEyes.Name = "txtSpecifiedEyes";
        txtSpecifiedEyes.Size = new Size(153, 33);
        txtSpecifiedEyes.TabIndex = 1;
        txtSpecifiedEyes.Text = "5,9,13";
        // 
        // rbSpecifiedEye
        // 
        rbSpecifiedEye.AutoSize = true;
        rbSpecifiedEye.Location = new Point(257, 27);
        rbSpecifiedEye.Name = "rbSpecifiedEye";
        rbSpecifiedEye.Size = new Size(117, 31);
        rbSpecifiedEye.TabIndex = 0;
        rbSpecifiedEye.Text = "指定眼位";
        rbSpecifiedEye.UseVisualStyleBackColor = true;
        rbSpecifiedEye.CheckedChanged += EyePosition_CheckedChanged;
        // 
        // grpRecognition
        // 
        grpRecognition.Controls.Add(btnRestoreDefaults);
        grpRecognition.Controls.Add(txtThreshold);
        grpRecognition.Controls.Add(lblThreshold);
        grpRecognition.Controls.Add(txtGain);
        grpRecognition.Controls.Add(lblGain);
        grpRecognition.Controls.Add(txtBlueExposure);
        grpRecognition.Controls.Add(lblBlueExposure);
        grpRecognition.Controls.Add(txtGreenExposure);
        grpRecognition.Controls.Add(lblGreenExposure);
        grpRecognition.Controls.Add(txtRedExposure);
        grpRecognition.Controls.Add(lblRedExposure);
        grpRecognition.Controls.Add(pnlRecognitionMode);
        grpRecognition.Controls.Add(pnlExposureMode);
        grpRecognition.Location = new Point(7, 8);
        grpRecognition.Name = "grpRecognition";
        grpRecognition.Size = new Size(666, 267);
        grpRecognition.TabIndex = 0;
        grpRecognition.TabStop = false;
        grpRecognition.Text = "属性 / 图像识别";
        // 
        // btnRestoreDefaults
        // 
        btnRestoreDefaults.BackColor = Color.WhiteSmoke;
        btnRestoreDefaults.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
        btnRestoreDefaults.Location = new Point(576, 27);
        btnRestoreDefaults.Name = "btnRestoreDefaults";
        btnRestoreDefaults.Size = new Size(42, 38);
        btnRestoreDefaults.TabIndex = 4;
        btnRestoreDefaults.Text = "↻";
        btnRestoreDefaults.UseVisualStyleBackColor = false;
        btnRestoreDefaults.Click += btnRestoreDefaults_Click;
        // 
        // txtThreshold
        // 
        txtThreshold.Location = new Point(446, 151);
        txtThreshold.Name = "txtThreshold";
        txtThreshold.Size = new Size(202, 33);
        txtThreshold.TabIndex = 14;
        txtThreshold.Text = "0.1";
        // 
        // lblThreshold
        // 
        lblThreshold.AutoSize = true;
        lblThreshold.Location = new Point(372, 154);
        lblThreshold.Name = "lblThreshold";
        lblThreshold.Size = new Size(72, 27);
        lblThreshold.TabIndex = 13;
        lblThreshold.Text = "阈值：";
        // 
        // txtGain
        // 
        txtGain.Location = new Point(446, 116);
        txtGain.Name = "txtGain";
        txtGain.Size = new Size(202, 33);
        txtGain.TabIndex = 12;
        txtGain.Text = "0";
        // 
        // lblGain
        // 
        lblGain.AutoSize = true;
        lblGain.Location = new Point(372, 119);
        lblGain.Name = "lblGain";
        lblGain.Size = new Size(72, 27);
        lblGain.TabIndex = 11;
        lblGain.Text = "增益：";
        // 
        // txtBlueExposure
        // 
        txtBlueExposure.Location = new Point(136, 189);
        txtBlueExposure.Name = "txtBlueExposure";
        txtBlueExposure.Size = new Size(202, 33);
        txtBlueExposure.TabIndex = 10;
        txtBlueExposure.Text = "143383";
        // 
        // lblBlueExposure
        // 
        lblBlueExposure.AutoSize = true;
        lblBlueExposure.Location = new Point(31, 192);
        lblBlueExposure.Name = "lblBlueExposure";
        lblBlueExposure.Size = new Size(120, 27);
        lblBlueExposure.TabIndex = 9;
        lblBlueExposure.Text = "B曝光(us)：";
        // 
        // txtGreenExposure
        // 
        txtGreenExposure.Location = new Point(136, 154);
        txtGreenExposure.Name = "txtGreenExposure";
        txtGreenExposure.Size = new Size(202, 33);
        txtGreenExposure.TabIndex = 8;
        txtGreenExposure.Text = "74434";
        // 
        // lblGreenExposure
        // 
        lblGreenExposure.AutoSize = true;
        lblGreenExposure.Location = new Point(30, 157);
        lblGreenExposure.Name = "lblGreenExposure";
        lblGreenExposure.Size = new Size(122, 27);
        lblGreenExposure.TabIndex = 7;
        lblGreenExposure.Text = "G曝光(us)：";
        // 
        // txtRedExposure
        // 
        txtRedExposure.Location = new Point(136, 119);
        txtRedExposure.Name = "txtRedExposure";
        txtRedExposure.Size = new Size(202, 33);
        txtRedExposure.TabIndex = 6;
        txtRedExposure.Text = "84848";
        // 
        // lblRedExposure
        // 
        lblRedExposure.AutoSize = true;
        lblRedExposure.Location = new Point(30, 122);
        lblRedExposure.Name = "lblRedExposure";
        lblRedExposure.Size = new Size(120, 27);
        lblRedExposure.TabIndex = 5;
        lblRedExposure.Text = "R曝光(us)：";
        // 
        // pnlRecognitionMode
        // 
        pnlRecognitionMode.Controls.Add(rbThresholdRecognition);
        pnlRecognitionMode.Controls.Add(rbAutoRecognition);
        pnlRecognitionMode.Location = new Point(240, 73);
        pnlRecognitionMode.Name = "pnlRecognitionMode";
        pnlRecognitionMode.Size = new Size(293, 40);
        pnlRecognitionMode.TabIndex = 3;
        // 
        // rbThresholdRecognition
        // 
        rbThresholdRecognition.AutoSize = true;
        rbThresholdRecognition.Checked = true;
        rbThresholdRecognition.Location = new Point(147, 4);
        rbThresholdRecognition.Name = "rbThresholdRecognition";
        rbThresholdRecognition.Size = new Size(117, 31);
        rbThresholdRecognition.TabIndex = 3;
        rbThresholdRecognition.TabStop = true;
        rbThresholdRecognition.Text = "阈值识别";
        rbThresholdRecognition.UseVisualStyleBackColor = true;
        // 
        // rbAutoRecognition
        // 
        rbAutoRecognition.AutoSize = true;
        rbAutoRecognition.Location = new Point(17, 3);
        rbAutoRecognition.Name = "rbAutoRecognition";
        rbAutoRecognition.Size = new Size(117, 31);
        rbAutoRecognition.TabIndex = 2;
        rbAutoRecognition.Text = "自动识别";
        rbAutoRecognition.UseVisualStyleBackColor = true;
        // 
        // pnlExposureMode
        // 
        pnlExposureMode.Controls.Add(rbManualExposure);
        pnlExposureMode.Controls.Add(rbAutoExposure);
        pnlExposureMode.Location = new Point(240, 19);
        pnlExposureMode.Name = "pnlExposureMode";
        pnlExposureMode.Size = new Size(293, 46);
        pnlExposureMode.TabIndex = 2;
        // 
        // rbManualExposure
        // 
        rbManualExposure.AutoSize = true;
        rbManualExposure.Location = new Point(147, 4);
        rbManualExposure.Name = "rbManualExposure";
        rbManualExposure.Size = new Size(117, 31);
        rbManualExposure.TabIndex = 1;
        rbManualExposure.Text = "手动曝光";
        rbManualExposure.UseVisualStyleBackColor = true;
        // 
        // rbAutoExposure
        // 
        rbAutoExposure.AutoSize = true;
        rbAutoExposure.Checked = true;
        rbAutoExposure.Location = new Point(10, 4);
        rbAutoExposure.Name = "rbAutoExposure";
        rbAutoExposure.Size = new Size(117, 31);
        rbAutoExposure.TabIndex = 0;
        rbAutoExposure.TabStop = true;
        rbAutoExposure.Text = "自动曝光";
        rbAutoExposure.UseVisualStyleBackColor = true;
        // 
        // timedSendTimer
        // 
        timedSendTimer.Interval = 60000;
        timedSendTimer.Tick += timedSendTimer_Tick;
        // 
        // MainForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(232, 242, 247);
        ClientSize = new Size(1196, 945);
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);
        Controls.Add(grpTcp);
        Font = new Font("Microsoft YaHei UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "编辑测试项";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        Load += MainForm_Load;
        grpTcp.ResumeLayout(false);
        grpTcp.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
        pnlLeft.ResumeLayout(false);
        pnlLeft.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numTimedSendInterval).EndInit();
        grpImageCard.ResumeLayout(false);
        grpImageCard.PerformLayout();
        grpResult.ResumeLayout(false);
        grpResult.PerformLayout();
        pnlRight.ResumeLayout(false);
        grpCommunication.ResumeLayout(false);
        grpCommunication.PerformLayout();
        grpEyePosition.ResumeLayout(false);
        grpEyePosition.PerformLayout();
        grpRecognition.ResumeLayout(false);
        grpRecognition.PerformLayout();
        pnlRecognitionMode.ResumeLayout(false);
        pnlRecognitionMode.PerformLayout();
        pnlExposureMode.ResumeLayout(false);
        pnlExposureMode.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox grpTcp;
    private Label lblStatus;
    private Button btnConnect;
    private NumericUpDown nudPort;
    private Label lblPort;
    private TextBox txtHost;
    private Label lblHost;
    private TextBox txtTarget;
    private Label lblTarget;
    private Panel pnlLeft;
    private Button btnBatchTest;
    private Label lblTimedSendInterval;
    private NumericUpDown numTimedSendInterval;
    private Button btnTimedSend;
    private Button btnCancel;
    private Button btnConfirm;
    private Button btnEditPointTemplate;
    private ComboBox cmbPointTemplate;
    private Label lblPointTemplate;
    private GroupBox grpImageCard;
    private Button btnEditBmp;
    private Button btnEditLogical;
    private ComboBox cmbBmpCard;
    private ComboBox cmbLogicalCard;
    private RadioButton rbBmp;
    private RadioButton rbLogical;
    private GroupBox grpResult;
    private RadioButton rbResultNg;
    private RadioButton rbResultOk;
    private TextBox txtTestName;
    private Label lblTestName;
    private ComboBox cmbTestType;
    private Label lblTestType;
    private Panel pnlRight;
    private GroupBox grpCommunication;
    private Button btnLoadReceived;
    private Button btnClearLog;
    private TextBox txtReceiveLog;
    private Label lblReceive;
    private TextBox txtSendPreview;
    private Label lblSendPreview;
    private GroupBox grpEyePosition;
    private RadioButton rbLowerEyeBox;
    private RadioButton rbCenterEyeBox;
    private RadioButton rbUpperEyeBox;
    private RadioButton rbFullEyeBox;
    private RadioButton rbCenterEye;
    private TextBox txtSpecifiedEyes;
    private RadioButton rbSpecifiedEye;
    private GroupBox grpRecognition;
    private Button btnRestoreDefaults;
    private TextBox txtThreshold;
    private Label lblThreshold;
    private TextBox txtGain;
    private Label lblGain;
    private TextBox txtBlueExposure;
    private Label lblBlueExposure;
    private TextBox txtGreenExposure;
    private Label lblGreenExposure;
    private TextBox txtRedExposure;
    private Label lblRedExposure;
    private Panel pnlRecognitionMode;
    private RadioButton rbThresholdRecognition;
    private RadioButton rbAutoRecognition;
    private Panel pnlExposureMode;
    private RadioButton rbManualExposure;
    private RadioButton rbAutoExposure;
    private System.Windows.Forms.Timer timedSendTimer;
}
