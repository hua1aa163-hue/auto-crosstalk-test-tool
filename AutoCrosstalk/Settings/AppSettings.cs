namespace AutoCrosstalk.Settings;

/// <summary>程序关闭后需要保留的用户设置。</summary>
public sealed class AppSettings
{
    public MainFormSettings MainForm { get; set; } = new();
    public BatchFormSettings BatchForm { get; set; } = new();
}

public sealed class MainFormSettings
{
    public string Target { get; set; } = "Target";
    public bool ResultOk { get; set; } = true;
    public int TestTypeIndex { get; set; } = 1;
    public string TestName { get; set; } = "brightness";
    public bool LogicalImage { get; set; } = true;
    public string LogicalCard { get; set; } = "White";
    public string BmpCard { get; set; } = string.Empty;
    public string PointTemplate { get; set; } = "9";
    public bool AutoExposure { get; set; } = true;
    public bool AutoRecognition { get; set; }
    public string RedExposure { get; set; } = "84848";
    public string GreenExposure { get; set; } = "74434";
    public string BlueExposure { get; set; } = "143383";
    public string Gain { get; set; } = "0";
    public string Threshold { get; set; } = "0.1";
    public int EyePositionMode { get; set; } = 1;
    public string SpecifiedEyes { get; set; } = "5,9,13";
    public decimal TimedSendIntervalMinutes { get; set; } = 1;
}

public sealed class BatchFormSettings
{
    public string ImageDirectory { get; set; } = @"D:\work_file\串扰\全屏 20260717";
    public int TopologyIndex { get; set; } = 4;
    public decimal SwitchDelayMilliseconds { get; set; } = 1200;
    public decimal MeasurementTimeoutSeconds { get; set; } = 120;
    public bool RestoreWallpaper { get; set; } = true;
    public bool ContinueAfterFailure { get; set; }
    public decimal TimedIntervalSeconds { get; set; } = 5;
}
