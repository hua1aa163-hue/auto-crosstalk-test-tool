namespace AutoCrosstalk.Models;

/// <summary>
/// 界面数据对应的 TCP 报文模型，属性顺序与通信协议一致。
/// </summary>
public sealed class TestMessage
{
    public string Target { get; init; } = "Target";
    public string Result { get; init; } = "OK";
    public int TestItem { get; init; }
    public string TestName { get; init; } = string.Empty;
    public int ImageType { get; init; }
    public string CardName { get; init; } = string.Empty;
    public string PointTemplateName { get; init; } = string.Empty;
    public int ExposureMode { get; init; }
    public int RecognitionMode { get; init; }
    public string RedExposure { get; init; } = string.Empty;
    public string GreenExposure { get; init; } = string.Empty;
    public string BlueExposure { get; init; } = string.Empty;
    public string Gain { get; init; } = string.Empty;
    public string Threshold { get; init; } = string.Empty;
    public int EyePositionMode { get; init; }
    public string SpecifiedEyePositions { get; init; } = string.Empty;
}
