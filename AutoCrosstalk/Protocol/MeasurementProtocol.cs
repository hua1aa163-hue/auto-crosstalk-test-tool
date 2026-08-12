namespace AutoCrosstalk.Protocol;

/// <summary>《TCP通讯协议3.0》中本程序使用的应答和测试命令。</summary>
public static class MeasurementProtocol
{
    public const string TargetSuccess = "&|Target|OK|@";
    public const string SingleAutoMeasure = "&|Meas|A|A|@";
    public const string SingleAutoRunning = "&|Meas|A|A|Run|@";
    public const string SingleAutoCompleted = "&|Meas|A|OK|@";

    public static bool IsTargetResponse(string message) =>
        message == TargetSuccess || message.StartsWith("&|Target|NG|", StringComparison.Ordinal);

    public static string GetTargetError(string message)
    {
        string code = message.Split('|').ElementAtOrDefault(3) ?? string.Empty;
        return code switch
        {
            "0" => "图卡错误",
            "1" => "点模板错误",
            "2" => "创建过多测试项",
            "3" => "其他错误",
            _ => $"未知错误（返回：{message}）"
        };
    }

    public static bool IsMeasurementResponse(string message) =>
        message == SingleAutoRunning ||
        message == SingleAutoCompleted ||
        message.StartsWith("&|Meas|A|NG|", StringComparison.Ordinal);
}
