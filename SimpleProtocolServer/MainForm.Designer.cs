#nullable disable

namespace SimpleProtocolServer;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        grpServer = new GroupBox();
        lblStatus = new Label();
        btnListen = new Button();
        numPort = new NumericUpDown();
        lblPort = new Label();
        txtHost = new TextBox();
        lblHost = new Label();
        grpCommand = new GroupBox();
        txtExpectedResponse = new TextBox();
        lblExpectedResponse = new Label();
        lblParameterHint = new Label();
        txtParameter = new TextBox();
        lblParameter = new Label();
        cmbCommand = new ComboBox();
        lblCommand = new Label();
        grpSend = new GroupBox();
        lblSendState = new Label();
        btnTimedSend = new Button();
        numIntervalMinutes = new NumericUpDown();
        lblIntervalMinutes = new Label();
        btnSendOnce = new Button();
        txtSendPreview = new TextBox();
        lblSendPreview = new Label();
        grpCycle = new GroupBox();
        lblCycleHint = new Label();
        btnClearCycle = new Button();
        btnRemoveCycle = new Button();
        btnAddCycle = new Button();
        lstCycleMessages = new ListBox();
        grpLog = new GroupBox();
        btnClearLog = new Button();
        txtLog = new TextBox();
        timedSendTimer = new System.Windows.Forms.Timer(components);
        grpServer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
        grpCommand.SuspendLayout();
        grpSend.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numIntervalMinutes).BeginInit();
        grpCycle.SuspendLayout();
        grpLog.SuspendLayout();
        SuspendLayout();
        // 
        // grpServer
        // 
        grpServer.Controls.Add(lblStatus);
        grpServer.Controls.Add(btnListen);
        grpServer.Controls.Add(numPort);
        grpServer.Controls.Add(lblPort);
        grpServer.Controls.Add(txtHost);
        grpServer.Controls.Add(lblHost);
        grpServer.Location = new Point(12, 12);
        grpServer.Name = "grpServer";
        grpServer.Size = new Size(1036, 86);
        grpServer.TabIndex = 0;
        grpServer.TabStop = false;
        grpServer.Text = "TCP 服务器";
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.ForeColor = Color.Maroon;
        lblStatus.Location = new Point(826, 36);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(90, 24);
        lblStatus.TabIndex = 5;
        lblStatus.Text = "● 未监听";
        // 
        // btnListen
        // 
        btnListen.BackColor = Color.FromArgb(225, 239, 255);
        btnListen.Location = new Point(638, 27);
        btnListen.Name = "btnListen";
        btnListen.Size = new Size(154, 42);
        btnListen.TabIndex = 4;
        btnListen.Text = "启动监听";
        btnListen.UseVisualStyleBackColor = false;
        btnListen.Click += btnListen_Click;
        // 
        // numPort
        // 
        numPort.Location = new Point(486, 34);
        numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numPort.Name = "numPort";
        numPort.ReadOnly = true;
        numPort.Size = new Size(120, 30);
        numPort.TabIndex = 3;
        numPort.Value = new decimal(new int[] { 9527, 0, 0, 0 });
        // 
        // lblPort
        // 
        lblPort.AutoSize = true;
        lblPort.Location = new Point(424, 37);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(56, 24);
        lblPort.TabIndex = 2;
        lblPort.Text = "端口：";
        // 
        // txtHost
        // 
        txtHost.Location = new Point(117, 34);
        txtHost.Name = "txtHost";
        txtHost.ReadOnly = true;
        txtHost.Size = new Size(268, 30);
        txtHost.TabIndex = 1;
        txtHost.Text = "127.0.0.1";
        // 
        // lblHost
        // 
        lblHost.AutoSize = true;
        lblHost.Location = new Point(26, 37);
        lblHost.Name = "lblHost";
        lblHost.Size = new Size(85, 24);
        lblHost.TabIndex = 0;
        lblHost.Text = "监听地址：";
        // 
        // grpCommand
        // 
        grpCommand.Controls.Add(txtExpectedResponse);
        grpCommand.Controls.Add(lblExpectedResponse);
        grpCommand.Controls.Add(lblParameterHint);
        grpCommand.Controls.Add(txtParameter);
        grpCommand.Controls.Add(lblParameter);
        grpCommand.Controls.Add(cmbCommand);
        grpCommand.Controls.Add(lblCommand);
        grpCommand.Location = new Point(12, 106);
        grpCommand.Name = "grpCommand";
        grpCommand.Size = new Size(522, 279);
        grpCommand.TabIndex = 1;
        grpCommand.TabStop = false;
        grpCommand.Text = "报文选择";
        // 
        // txtExpectedResponse
        // 
        txtExpectedResponse.BackColor = Color.White;
        txtExpectedResponse.Location = new Point(117, 185);
        txtExpectedResponse.Multiline = true;
        txtExpectedResponse.Name = "txtExpectedResponse";
        txtExpectedResponse.ReadOnly = true;
        txtExpectedResponse.Size = new Size(379, 66);
        txtExpectedResponse.TabIndex = 6;
        // 
        // lblExpectedResponse
        // 
        lblExpectedResponse.AutoSize = true;
        lblExpectedResponse.Location = new Point(26, 188);
        lblExpectedResponse.Name = "lblExpectedResponse";
        lblExpectedResponse.Size = new Size(85, 24);
        lblExpectedResponse.TabIndex = 5;
        lblExpectedResponse.Text = "预期返回：";
        // 
        // lblParameterHint
        // 
        lblParameterHint.AutoSize = true;
        lblParameterHint.ForeColor = Color.DimGray;
        lblParameterHint.Location = new Point(117, 141);
        lblParameterHint.Name = "lblParameterHint";
        lblParameterHint.Size = new Size(162, 24);
        lblParameterHint.TabIndex = 4;
        lblParameterHint.Text = "此命令不需要参数";
        // 
        // txtParameter
        // 
        txtParameter.Location = new Point(117, 102);
        txtParameter.Name = "txtParameter";
        txtParameter.Size = new Size(379, 30);
        txtParameter.TabIndex = 3;
        txtParameter.TextChanged += txtParameter_TextChanged;
        // 
        // lblParameter
        // 
        lblParameter.AutoSize = true;
        lblParameter.Location = new Point(26, 105);
        lblParameter.Name = "lblParameter";
        lblParameter.Size = new Size(85, 24);
        lblParameter.TabIndex = 2;
        lblParameter.Text = "命令参数：";
        // 
        // cmbCommand
        // 
        cmbCommand.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCommand.FormattingEnabled = true;
        cmbCommand.Items.AddRange(new object[] { "一、停止", "二、切换配方", "三、保存配方", "四、获取焦距 FF", "五、单次自动测试", "六、单次手动测试", "七、循环自动测试", "八、循环手动测试" });
        cmbCommand.Location = new Point(117, 51);
        cmbCommand.Name = "cmbCommand";
        cmbCommand.Size = new Size(379, 32);
        cmbCommand.TabIndex = 1;
        cmbCommand.SelectedIndexChanged += cmbCommand_SelectedIndexChanged;
        // 
        // lblCommand
        // 
        lblCommand.AutoSize = true;
        lblCommand.Location = new Point(26, 54);
        lblCommand.Name = "lblCommand";
        lblCommand.Size = new Size(85, 24);
        lblCommand.TabIndex = 0;
        lblCommand.Text = "命令类型：";
        // 
        // grpSend
        // 
        grpSend.Controls.Add(lblSendState);
        grpSend.Controls.Add(btnTimedSend);
        grpSend.Controls.Add(numIntervalMinutes);
        grpSend.Controls.Add(lblIntervalMinutes);
        grpSend.Controls.Add(btnSendOnce);
        grpSend.Controls.Add(txtSendPreview);
        grpSend.Controls.Add(lblSendPreview);
        grpSend.Location = new Point(12, 393);
        grpSend.Name = "grpSend";
        grpSend.Size = new Size(522, 272);
        grpSend.TabIndex = 2;
        grpSend.TabStop = false;
        grpSend.Text = "发送";
        // 
        // lblSendState
        // 
        lblSendState.AutoSize = true;
        lblSendState.ForeColor = Color.DimGray;
        lblSendState.Location = new Point(26, 231);
        lblSendState.Name = "lblSendState";
        lblSendState.Size = new Size(154, 24);
        lblSendState.TabIndex = 6;
        lblSendState.Text = "定时发送未启动";
        // 
        // btnTimedSend
        // 
        btnTimedSend.BackColor = Color.FromArgb(255, 243, 205);
        btnTimedSend.Location = new Point(334, 174);
        btnTimedSend.Name = "btnTimedSend";
        btnTimedSend.Size = new Size(162, 44);
        btnTimedSend.TabIndex = 5;
        btnTimedSend.Text = "开始定时发送";
        btnTimedSend.UseVisualStyleBackColor = false;
        btnTimedSend.Click += btnTimedSend_Click;
        // 
        // numIntervalMinutes
        // 
        numIntervalMinutes.DecimalPlaces = 1;
        numIntervalMinutes.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
        numIntervalMinutes.Location = new Point(252, 181);
        numIntervalMinutes.Maximum = new decimal(new int[] { 1440, 0, 0, 0 });
        numIntervalMinutes.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
        numIntervalMinutes.Name = "numIntervalMinutes";
        numIntervalMinutes.Size = new Size(72, 30);
        numIntervalMinutes.TabIndex = 4;
        numIntervalMinutes.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // lblIntervalMinutes
        // 
        lblIntervalMinutes.AutoSize = true;
        lblIntervalMinutes.Location = new Point(167, 184);
        lblIntervalMinutes.Name = "lblIntervalMinutes";
        lblIntervalMinutes.Size = new Size(79, 24);
        lblIntervalMinutes.TabIndex = 3;
        lblIntervalMinutes.Text = "间隔(分)：";
        // 
        // btnSendOnce
        // 
        btnSendOnce.BackColor = Color.FromArgb(218, 242, 218);
        btnSendOnce.Location = new Point(26, 174);
        btnSendOnce.Name = "btnSendOnce";
        btnSendOnce.Size = new Size(125, 44);
        btnSendOnce.TabIndex = 2;
        btnSendOnce.Text = "发送一次";
        btnSendOnce.UseVisualStyleBackColor = false;
        btnSendOnce.Click += btnSendOnce_Click;
        // 
        // txtSendPreview
        // 
        txtSendPreview.BackColor = Color.White;
        txtSendPreview.Font = new Font("Consolas", 11F);
        txtSendPreview.Location = new Point(26, 75);
        txtSendPreview.Multiline = true;
        txtSendPreview.Name = "txtSendPreview";
        txtSendPreview.ScrollBars = ScrollBars.Horizontal;
        txtSendPreview.Size = new Size(470, 82);
        txtSendPreview.TabIndex = 1;
        txtSendPreview.WordWrap = false;
        // 
        // lblSendPreview
        // 
        lblSendPreview.AutoSize = true;
        lblSendPreview.Location = new Point(26, 43);
        lblSendPreview.Name = "lblSendPreview";
        lblSendPreview.Size = new Size(157, 24);
        lblSendPreview.TabIndex = 0;
        lblSendPreview.Text = "发送报文（可编辑）：";
        // 
        // grpCycle
        // 
        grpCycle.Controls.Add(lblCycleHint);
        grpCycle.Controls.Add(btnClearCycle);
        grpCycle.Controls.Add(btnRemoveCycle);
        grpCycle.Controls.Add(btnAddCycle);
        grpCycle.Controls.Add(lstCycleMessages);
        grpCycle.Location = new Point(542, 106);
        grpCycle.Name = "grpCycle";
        grpCycle.Size = new Size(506, 279);
        grpCycle.TabIndex = 3;
        grpCycle.TabStop = false;
        grpCycle.Text = "循环报文列表";
        // 
        // lblCycleHint
        // 
        lblCycleHint.AutoSize = true;
        lblCycleHint.ForeColor = Color.DimGray;
        lblCycleHint.Location = new Point(20, 241);
        lblCycleHint.Name = "lblCycleHint";
        lblCycleHint.Size = new Size(298, 24);
        lblCycleHint.TabIndex = 4;
        lblCycleHint.Text = "定时发送将按顺序循环发送列表中的报文";
        // 
        // btnClearCycle
        // 
        btnClearCycle.Location = new Point(340, 194);
        btnClearCycle.Name = "btnClearCycle";
        btnClearCycle.Size = new Size(140, 36);
        btnClearCycle.TabIndex = 3;
        btnClearCycle.Text = "清空列表";
        btnClearCycle.UseVisualStyleBackColor = true;
        btnClearCycle.Click += btnClearCycle_Click;
        // 
        // btnRemoveCycle
        // 
        btnRemoveCycle.Location = new Point(183, 194);
        btnRemoveCycle.Name = "btnRemoveCycle";
        btnRemoveCycle.Size = new Size(147, 36);
        btnRemoveCycle.TabIndex = 2;
        btnRemoveCycle.Text = "删除选中";
        btnRemoveCycle.UseVisualStyleBackColor = true;
        btnRemoveCycle.Click += btnRemoveCycle_Click;
        // 
        // btnAddCycle
        // 
        btnAddCycle.BackColor = Color.FromArgb(225, 239, 255);
        btnAddCycle.Location = new Point(20, 194);
        btnAddCycle.Name = "btnAddCycle";
        btnAddCycle.Size = new Size(153, 36);
        btnAddCycle.TabIndex = 1;
        btnAddCycle.Text = "加入循环列表";
        btnAddCycle.UseVisualStyleBackColor = false;
        btnAddCycle.Click += btnAddCycle_Click;
        // 
        // lstCycleMessages
        // 
        lstCycleMessages.Font = new Font("Consolas", 10F);
        lstCycleMessages.FormattingEnabled = true;
        lstCycleMessages.HorizontalScrollbar = true;
        lstCycleMessages.ItemHeight = 23;
        lstCycleMessages.Location = new Point(20, 35);
        lstCycleMessages.Name = "lstCycleMessages";
        lstCycleMessages.Size = new Size(460, 142);
        lstCycleMessages.TabIndex = 0;
        lstCycleMessages.DoubleClick += lstCycleMessages_DoubleClick;
        // 
        // grpLog
        // 
        grpLog.Controls.Add(btnClearLog);
        grpLog.Controls.Add(txtLog);
        grpLog.Location = new Point(542, 393);
        grpLog.Name = "grpLog";
        grpLog.Size = new Size(506, 272);
        grpLog.TabIndex = 4;
        grpLog.TabStop = false;
        grpLog.Text = "TCP 收发日志";
        // 
        // btnClearLog
        // 
        btnClearLog.Location = new Point(375, 220);
        btnClearLog.Name = "btnClearLog";
        btnClearLog.Size = new Size(105, 36);
        btnClearLog.TabIndex = 1;
        btnClearLog.Text = "清空日志";
        btnClearLog.UseVisualStyleBackColor = true;
        btnClearLog.Click += btnClearLog_Click;
        // 
        // txtLog
        // 
        txtLog.BackColor = Color.White;
        txtLog.Font = new Font("Consolas", 10F);
        txtLog.Location = new Point(20, 35);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(460, 174);
        txtLog.TabIndex = 0;
        // 
        // timedSendTimer
        // 
        timedSendTimer.Interval = 60000;
        timedSendTimer.Tick += timedSendTimer_Tick;
        // 
        // MainForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(242, 247, 250);
        ClientSize = new Size(1060, 679);
        Controls.Add(grpLog);
        Controls.Add(grpCycle);
        Controls.Add(grpSend);
        Controls.Add(grpCommand);
        Controls.Add(grpServer);
        Font = new Font("Microsoft YaHei UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "简易报文 TCP 服务器";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        Load += MainForm_Load;
        grpServer.ResumeLayout(false);
        grpServer.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
        grpCommand.ResumeLayout(false);
        grpCommand.PerformLayout();
        grpSend.ResumeLayout(false);
        grpSend.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numIntervalMinutes).EndInit();
        grpCycle.ResumeLayout(false);
        grpCycle.PerformLayout();
        grpLog.ResumeLayout(false);
        grpLog.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox grpServer;
    private Label lblStatus;
    private Button btnListen;
    private NumericUpDown numPort;
    private Label lblPort;
    private TextBox txtHost;
    private Label lblHost;
    private GroupBox grpCommand;
    private TextBox txtExpectedResponse;
    private Label lblExpectedResponse;
    private Label lblParameterHint;
    private TextBox txtParameter;
    private Label lblParameter;
    private ComboBox cmbCommand;
    private Label lblCommand;
    private GroupBox grpSend;
    private Label lblSendState;
    private Button btnTimedSend;
    private NumericUpDown numIntervalMinutes;
    private Label lblIntervalMinutes;
    private Button btnSendOnce;
    private TextBox txtSendPreview;
    private Label lblSendPreview;
    private GroupBox grpCycle;
    private Label lblCycleHint;
    private Button btnClearCycle;
    private Button btnRemoveCycle;
    private Button btnAddCycle;
    private ListBox lstCycleMessages;
    private GroupBox grpLog;
    private Button btnClearLog;
    private TextBox txtLog;
    private System.Windows.Forms.Timer timedSendTimer;
}
