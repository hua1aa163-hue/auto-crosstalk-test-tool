#nullable disable

namespace AutoCrosstalk;

partial class BatchTestForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            picPreview.Image?.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        grpSettings = new GroupBox();
        chkContinueAfterFailure = new CheckBox();
        chkRestoreWallpaper = new CheckBox();
        numMeasurementTimeout = new NumericUpDown();
        lblMeasurementTimeout = new Label();
        numSwitchDelay = new NumericUpDown();
        lblSwitchDelay = new Label();
        btnApplyTopology = new Button();
        cmbTopology = new ComboBox();
        lblTopology = new Label();
        btnBrowseDirectory = new Button();
        txtImageDirectory = new TextBox();
        lblImageDirectory = new Label();
        grpImages = new GroupBox();
        lblBatchStatus = new Label();
        progressBatch = new ProgressBar();
        lblCurrentImage = new Label();
        picPreview = new PictureBox();
        lvImages = new ListView();
        colIndex = new ColumnHeader();
        colFileName = new ColumnHeader();
        colStatus = new ColumnHeader();
        grpLog = new GroupBox();
        txtBatchLog = new TextBox();
        btnRefreshImages = new Button();
        btnTestDesktop = new Button();
        lblTimedInterval = new Label();
        numTimedInterval = new NumericUpDown();
        btnTimedProjection = new Button();
        btnStart = new Button();
        btnStop = new Button();
        btnClose = new Button();
        timedProjectionTimer = new System.Windows.Forms.Timer(components);
        grpSettings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numMeasurementTimeout).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numSwitchDelay).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numTimedInterval).BeginInit();
        grpImages.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
        grpLog.SuspendLayout();
        SuspendLayout();
        // 
        // grpSettings
        // 
        grpSettings.Controls.Add(chkContinueAfterFailure);
        grpSettings.Controls.Add(chkRestoreWallpaper);
        grpSettings.Controls.Add(numMeasurementTimeout);
        grpSettings.Controls.Add(lblMeasurementTimeout);
        grpSettings.Controls.Add(numSwitchDelay);
        grpSettings.Controls.Add(lblSwitchDelay);
        grpSettings.Controls.Add(btnApplyTopology);
        grpSettings.Controls.Add(cmbTopology);
        grpSettings.Controls.Add(lblTopology);
        grpSettings.Controls.Add(btnBrowseDirectory);
        grpSettings.Controls.Add(txtImageDirectory);
        grpSettings.Controls.Add(lblImageDirectory);
        grpSettings.Location = new Point(10, 8);
        grpSettings.Name = "grpSettings";
        grpSettings.Size = new Size(1020, 135);
        grpSettings.TabIndex = 0;
        grpSettings.TabStop = false;
        grpSettings.Text = "批量测试设置";
        // 
        // chkContinueAfterFailure
        // 
        chkContinueAfterFailure.AutoSize = true;
        chkContinueAfterFailure.Location = new Point(188, 100);
        chkContinueAfterFailure.Name = "chkContinueAfterFailure";
        chkContinueAfterFailure.Size = new Size(121, 24);
        chkContinueAfterFailure.TabIndex = 11;
        chkContinueAfterFailure.Text = "单图失败后继续";
        chkContinueAfterFailure.UseVisualStyleBackColor = true;
        // 
        // chkRestoreWallpaper
        // 
        chkRestoreWallpaper.AutoSize = true;
        chkRestoreWallpaper.Checked = true;
        chkRestoreWallpaper.CheckState = CheckState.Checked;
        chkRestoreWallpaper.Location = new Point(18, 100);
        chkRestoreWallpaper.Name = "chkRestoreWallpaper";
        chkRestoreWallpaper.Size = new Size(136, 24);
        chkRestoreWallpaper.TabIndex = 10;
        chkRestoreWallpaper.Text = "完成后恢复原桌面";
        chkRestoreWallpaper.UseVisualStyleBackColor = true;
        // 
        // numMeasurementTimeout
        // 
        numMeasurementTimeout.Location = new Point(874, 66);
        numMeasurementTimeout.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
        numMeasurementTimeout.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
        numMeasurementTimeout.Name = "numMeasurementTimeout";
        numMeasurementTimeout.Size = new Size(117, 27);
        numMeasurementTimeout.TabIndex = 9;
        numMeasurementTimeout.Value = new decimal(new int[] { 120, 0, 0, 0 });
        // 
        // lblMeasurementTimeout
        // 
        lblMeasurementTimeout.AutoSize = true;
        lblMeasurementTimeout.Location = new Point(733, 69);
        lblMeasurementTimeout.Name = "lblMeasurementTimeout";
        lblMeasurementTimeout.Size = new Size(129, 20);
        lblMeasurementTimeout.TabIndex = 8;
        lblMeasurementTimeout.Text = "测试超时（秒）：";
        // 
        // numSwitchDelay
        // 
        numSwitchDelay.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        numSwitchDelay.Location = new Point(607, 66);
        numSwitchDelay.Maximum = new decimal(new int[] { 30000, 0, 0, 0 });
        numSwitchDelay.Name = "numSwitchDelay";
        numSwitchDelay.Size = new Size(110, 27);
        numSwitchDelay.TabIndex = 7;
        numSwitchDelay.Value = new decimal(new int[] { 1200, 0, 0, 0 });
        // 
        // lblSwitchDelay
        // 
        lblSwitchDelay.AutoSize = true;
        lblSwitchDelay.Location = new Point(466, 69);
        lblSwitchDelay.Name = "lblSwitchDelay";
        lblSwitchDelay.Size = new Size(129, 20);
        lblSwitchDelay.TabIndex = 6;
        lblSwitchDelay.Text = "切图等待（毫秒）：";
        // 
        // btnApplyTopology
        // 
        btnApplyTopology.BackColor = Color.WhiteSmoke;
        btnApplyTopology.Location = new Point(301, 62);
        btnApplyTopology.Name = "btnApplyTopology";
        btnApplyTopology.Size = new Size(143, 33);
        btnApplyTopology.TabIndex = 5;
        btnApplyTopology.Text = "立即应用投影模式";
        btnApplyTopology.UseVisualStyleBackColor = false;
        btnApplyTopology.Click += btnApplyTopology_Click;
        // 
        // cmbTopology
        // 
        cmbTopology.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTopology.FormattingEnabled = true;
        cmbTopology.Items.AddRange(new object[] { "不切换投影模式", "仅电脑屏幕", "复制屏幕", "仅第二屏幕", "扩展屏幕" });
        cmbTopology.Location = new Point(125, 65);
        cmbTopology.Name = "cmbTopology";
        cmbTopology.Size = new Size(160, 28);
        cmbTopology.TabIndex = 4;
        // 
        // lblTopology
        // 
        lblTopology.AutoSize = true;
        lblTopology.Location = new Point(18, 69);
        lblTopology.Name = "lblTopology";
        lblTopology.Size = new Size(84, 20);
        lblTopology.TabIndex = 3;
        lblTopology.Text = "投影模式：";
        // 
        // btnBrowseDirectory
        // 
        btnBrowseDirectory.BackColor = Color.WhiteSmoke;
        btnBrowseDirectory.Location = new Point(897, 25);
        btnBrowseDirectory.Name = "btnBrowseDirectory";
        btnBrowseDirectory.Size = new Size(94, 31);
        btnBrowseDirectory.TabIndex = 2;
        btnBrowseDirectory.Text = "浏览...";
        btnBrowseDirectory.UseVisualStyleBackColor = false;
        btnBrowseDirectory.Click += btnBrowseDirectory_Click;
        // 
        // txtImageDirectory
        // 
        txtImageDirectory.Location = new Point(125, 27);
        txtImageDirectory.Name = "txtImageDirectory";
        txtImageDirectory.Size = new Size(756, 27);
        txtImageDirectory.TabIndex = 1;
        txtImageDirectory.Text = "D:\\work_file\\串扰\\全屏 20260717";
        // 
        // lblImageDirectory
        // 
        lblImageDirectory.AutoSize = true;
        lblImageDirectory.Location = new Point(18, 30);
        lblImageDirectory.Name = "lblImageDirectory";
        lblImageDirectory.Size = new Size(84, 20);
        lblImageDirectory.TabIndex = 0;
        lblImageDirectory.Text = "图片目录：";
        // 
        // grpImages
        // 
        grpImages.Controls.Add(lblBatchStatus);
        grpImages.Controls.Add(progressBatch);
        grpImages.Controls.Add(lblCurrentImage);
        grpImages.Controls.Add(picPreview);
        grpImages.Controls.Add(lvImages);
        grpImages.Location = new Point(10, 149);
        grpImages.Name = "grpImages";
        grpImages.Size = new Size(1020, 405);
        grpImages.TabIndex = 1;
        grpImages.TabStop = false;
        grpImages.Text = "测试图片";
        // 
        // lblBatchStatus
        // 
        lblBatchStatus.AutoEllipsis = true;
        lblBatchStatus.Location = new Point(623, 360);
        lblBatchStatus.Name = "lblBatchStatus";
        lblBatchStatus.Size = new Size(369, 25);
        lblBatchStatus.TabIndex = 4;
        lblBatchStatus.Text = "等待开始";
        // 
        // progressBatch
        // 
        progressBatch.Location = new Point(623, 327);
        progressBatch.Maximum = 9;
        progressBatch.Name = "progressBatch";
        progressBatch.Size = new Size(369, 24);
        progressBatch.TabIndex = 3;
        // 
        // lblCurrentImage
        // 
        lblCurrentImage.AutoEllipsis = true;
        lblCurrentImage.Location = new Point(623, 289);
        lblCurrentImage.Name = "lblCurrentImage";
        lblCurrentImage.Size = new Size(369, 29);
        lblCurrentImage.TabIndex = 2;
        lblCurrentImage.Text = "当前图片：无";
        // 
        // picPreview
        // 
        picPreview.BackColor = Color.Black;
        picPreview.BorderStyle = BorderStyle.FixedSingle;
        picPreview.Location = new Point(623, 26);
        picPreview.Name = "picPreview";
        picPreview.Size = new Size(369, 252);
        picPreview.SizeMode = PictureBoxSizeMode.Zoom;
        picPreview.TabIndex = 1;
        picPreview.TabStop = false;
        // 
        // lvImages
        // 
        lvImages.Columns.AddRange(new ColumnHeader[] { colIndex, colFileName, colStatus });
        lvImages.FullRowSelect = true;
        lvImages.GridLines = true;
        lvImages.Location = new Point(15, 26);
        lvImages.MultiSelect = false;
        lvImages.Name = "lvImages";
        lvImages.Size = new Size(590, 359);
        lvImages.TabIndex = 0;
        lvImages.UseCompatibleStateImageBehavior = false;
        lvImages.View = View.Details;
        lvImages.SelectedIndexChanged += lvImages_SelectedIndexChanged;
        // 
        // colIndex
        // 
        colIndex.Text = "序号";
        colIndex.Width = 55;
        // 
        // colFileName
        // 
        colFileName.Text = "文件名";
        colFileName.Width = 360;
        // 
        // colStatus
        // 
        colStatus.Text = "状态";
        colStatus.Width = 165;
        // 
        // grpLog
        // 
        grpLog.Controls.Add(txtBatchLog);
        grpLog.Location = new Point(10, 560);
        grpLog.Name = "grpLog";
        grpLog.Size = new Size(1020, 113);
        grpLog.TabIndex = 2;
        grpLog.TabStop = false;
        grpLog.Text = "批量测试日志";
        // 
        // txtBatchLog
        // 
        txtBatchLog.BackColor = Color.White;
        txtBatchLog.Location = new Point(15, 25);
        txtBatchLog.Multiline = true;
        txtBatchLog.Name = "txtBatchLog";
        txtBatchLog.ReadOnly = true;
        txtBatchLog.ScrollBars = ScrollBars.Vertical;
        txtBatchLog.Size = new Size(977, 75);
        txtBatchLog.TabIndex = 0;
        // 
        // btnRefreshImages
        // 
        btnRefreshImages.BackColor = Color.WhiteSmoke;
        btnRefreshImages.Location = new Point(10, 682);
        btnRefreshImages.Name = "btnRefreshImages";
        btnRefreshImages.Size = new Size(122, 38);
        btnRefreshImages.TabIndex = 3;
        btnRefreshImages.Text = "刷新图片列表";
        btnRefreshImages.UseVisualStyleBackColor = false;
        btnRefreshImages.Click += btnRefreshImages_Click;
        // 
        // btnTestDesktop
        // 
        btnTestDesktop.BackColor = Color.FromArgb(228, 240, 252);
        btnTestDesktop.Location = new Point(144, 682);
        btnTestDesktop.Name = "btnTestDesktop";
        btnTestDesktop.Size = new Size(168, 38);
        btnTestDesktop.TabIndex = 4;
        btnTestDesktop.Text = "测试切换下一张";
        btnTestDesktop.UseVisualStyleBackColor = false;
        btnTestDesktop.Click += btnTestDesktop_Click;
        // 
        // lblTimedInterval
        // 
        lblTimedInterval.AutoSize = true;
        lblTimedInterval.Location = new Point(324, 691);
        lblTimedInterval.Name = "lblTimedInterval";
        lblTimedInterval.Size = new Size(84, 20);
        lblTimedInterval.TabIndex = 5;
        lblTimedInterval.Text = "定时间隔(s)";
        // 
        // numTimedInterval
        // 
        numTimedInterval.Location = new Point(410, 687);
        numTimedInterval.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
        numTimedInterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numTimedInterval.Name = "numTimedInterval";
        numTimedInterval.Size = new Size(64, 27);
        numTimedInterval.TabIndex = 6;
        numTimedInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
        // 
        // btnTimedProjection
        // 
        btnTimedProjection.BackColor = Color.FromArgb(255, 246, 215);
        btnTimedProjection.Location = new Point(482, 682);
        btnTimedProjection.Name = "btnTimedProjection";
        btnTimedProjection.Size = new Size(104, 38);
        btnTimedProjection.TabIndex = 7;
        btnTimedProjection.Text = "开始定时投图";
        btnTimedProjection.UseVisualStyleBackColor = false;
        btnTimedProjection.Click += btnTimedProjection_Click;
        // 
        // btnStart
        // 
        btnStart.BackColor = Color.FromArgb(225, 245, 229);
        btnStart.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Bold);
        btnStart.Location = new Point(596, 681);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(140, 40);
        btnStart.TabIndex = 8;
        btnStart.Text = "开始批量测试";
        btnStart.UseVisualStyleBackColor = false;
        btnStart.Click += btnStart_Click;
        // 
        // btnStop
        // 
        btnStop.BackColor = Color.FromArgb(255, 239, 226);
        btnStop.Enabled = false;
        btnStop.Location = new Point(748, 682);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(123, 38);
        btnStop.TabIndex = 9;
        btnStop.Text = "停止";
        btnStop.UseVisualStyleBackColor = false;
        btnStop.Click += btnStop_Click;
        // 
        // btnClose
        // 
        btnClose.BackColor = Color.WhiteSmoke;
        btnClose.Location = new Point(883, 682);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(147, 38);
        btnClose.TabIndex = 10;
        btnClose.Text = "关闭";
        btnClose.UseVisualStyleBackColor = false;
        btnClose.Click += btnClose_Click;
        // 
        // timedProjectionTimer
        // 
        timedProjectionTimer.Interval = 5000;
        timedProjectionTimer.Tick += timedProjectionTimer_Tick;
        // 
        // BatchTestForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(232, 242, 247);
        ClientSize = new Size(1040, 730);
        Controls.Add(btnClose);
        Controls.Add(btnStop);
        Controls.Add(btnStart);
        Controls.Add(btnTimedProjection);
        Controls.Add(numTimedInterval);
        Controls.Add(lblTimedInterval);
        Controls.Add(btnTestDesktop);
        Controls.Add(btnRefreshImages);
        Controls.Add(grpLog);
        Controls.Add(grpImages);
        Controls.Add(grpSettings);
        Font = new Font("Microsoft YaHei UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "BatchTestForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "图片亮色度自动测试";
        FormClosing += BatchTestForm_FormClosing;
        Load += BatchTestForm_Load;
        grpSettings.ResumeLayout(false);
        grpSettings.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numMeasurementTimeout).EndInit();
        ((System.ComponentModel.ISupportInitialize)numSwitchDelay).EndInit();
        ((System.ComponentModel.ISupportInitialize)numTimedInterval).EndInit();
        grpImages.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
        grpLog.ResumeLayout(false);
        grpLog.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox grpSettings;
    private CheckBox chkContinueAfterFailure;
    private CheckBox chkRestoreWallpaper;
    private NumericUpDown numMeasurementTimeout;
    private Label lblMeasurementTimeout;
    private NumericUpDown numSwitchDelay;
    private Label lblSwitchDelay;
    private Button btnApplyTopology;
    private ComboBox cmbTopology;
    private Label lblTopology;
    private Button btnBrowseDirectory;
    private TextBox txtImageDirectory;
    private Label lblImageDirectory;
    private GroupBox grpImages;
    private Label lblBatchStatus;
    private ProgressBar progressBatch;
    private Label lblCurrentImage;
    private PictureBox picPreview;
    private ListView lvImages;
    private ColumnHeader colIndex;
    private ColumnHeader colFileName;
    private ColumnHeader colStatus;
    private GroupBox grpLog;
    private TextBox txtBatchLog;
    private Button btnRefreshImages;
    private Button btnTestDesktop;
    private Label lblTimedInterval;
    private NumericUpDown numTimedInterval;
    private Button btnTimedProjection;
    private Button btnStart;
    private Button btnStop;
    private Button btnClose;
    private System.Windows.Forms.Timer timedProjectionTimer;
}
