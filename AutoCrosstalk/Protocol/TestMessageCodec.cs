using AutoCrosstalk.Models;

namespace AutoCrosstalk.Protocol;

/// <summary>
/// 负责测试消息与 &amp;|...|@ 文本报文之间的转换。
/// </summary>
public static class TestMessageCodec
{
    public const string StartMarker = "&|";
    public const string EndMarker = "|@";

    /// <summary>按协议规定的字段顺序生成待发送文本。</summary>
    public static string Serialize(TestMessage message)
    {
        var fields = new[]
        {
            message.Target,
            message.Result,
            message.TestItem.ToString(),
            message.TestName,
            message.ImageType.ToString(),
            message.CardName,
            message.PointTemplateName,
            message.ExposureMode.ToString(),
            message.RecognitionMode.ToString(),
            message.RedExposure,
            message.GreenExposure,
            message.BlueExposure,
            message.Gain,
            message.Threshold,
            message.EyePositionMode.ToString(),
            message.SpecifiedEyePositions
        };

        return StartMarker + string.Join('|', fields) + EndMarker;
    }

    /// <summary>尝试解析完整报文，便于核对对端返回的数据。</summary>
    public static bool TryDeserialize(string text, out TestMessage? message, out string error)
    {
        message = null;
        error = string.Empty;

        if (!text.StartsWith(StartMarker, StringComparison.Ordinal) ||
            !text.EndsWith(EndMarker, StringComparison.Ordinal))
        {
            error = "报文必须以 &| 开始、以 |@ 结束。";
            return false;
        }

        string body = text[StartMarker.Length..^EndMarker.Length];
        string[] fields = body.Split('|');
        if (fields.Length != 16)
        {
            error = $"字段数量应为 16，实际为 {fields.Length}。";
            return false;
        }

        if (!int.TryParse(fields[2], out int testItem) ||
            !int.TryParse(fields[4], out int imageType) ||
            !int.TryParse(fields[7], out int exposureMode) ||
            !int.TryParse(fields[8], out int recognitionMode) ||
            !int.TryParse(fields[14], out int eyePositionMode))
        {
            error = "报文中的枚举或测试项字段不是有效整数。";
            return false;
        }

        message = new TestMessage
        {
            Target = fields[0],
            Result = fields[1],
            TestItem = testItem,
            TestName = fields[3],
            ImageType = imageType,
            CardName = fields[5],
            PointTemplateName = fields[6],
            ExposureMode = exposureMode,
            RecognitionMode = recognitionMode,
            RedExposure = fields[9],
            GreenExposure = fields[10],
            BlueExposure = fields[11],
            Gain = fields[12],
            Threshold = fields[13],
            EyePositionMode = eyePositionMode,
            SpecifiedEyePositions = fields[15]
        };

        return true;
    }

    /// <summary>协议没有转义规则，因此字段中不能出现控制字符。</summary>
    public static bool ContainsReservedCharacter(string value) =>
        value.IndexOfAny(['|', '&', '@', '\r', '\n']) >= 0;
}
