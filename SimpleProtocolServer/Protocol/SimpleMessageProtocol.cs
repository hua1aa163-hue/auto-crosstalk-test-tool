namespace SimpleProtocolServer.Protocol;

/// <summary>负责根据界面选择生成 &amp;|...|@ 报文。</summary>
internal static class SimpleMessageProtocol
{
    public const string StartMarker = "&|";
    public const string EndMarker = "|@";

    public static string GetParameterLabel(CommandType command) => command switch
    {
        CommandType.SwitchRecipe or CommandType.SaveRecipe => "配方名称：",
        CommandType.GetFocalLength => "VID(mm)：",
        _ => "命令参数："
    };

    public static string GetDefaultParameter(CommandType command) => command switch
    {
        CommandType.SwitchRecipe => "qwerty",
        CommandType.SaveRecipe => "asdfgh",
        CommandType.GetFocalLength => "7500",
        _ => string.Empty
    };

    public static bool RequiresParameter(CommandType command) => command is
        CommandType.SwitchRecipe or CommandType.SaveRecipe or CommandType.GetFocalLength;

    public static string GetExpectedResponse(CommandType command) => command switch
    {
        CommandType.Stop => "&|Stop|OK|@",
        CommandType.SwitchRecipe => "&|Elems|OK|@  或  &|Elems|NG|@",
        CommandType.SaveRecipe => "&|Elems|OK|@",
        CommandType.GetFocalLength => "&|FF|焦距|@  例如：&|FF|50|@",
        CommandType.SingleAutomatic => "&|Meas|A|A|Run|@  →  &|Meas|A|OK|@",
        CommandType.SingleManual => "&|Meas|A|M|Run|@  →  &|Meas|A|OK|@",
        CommandType.ContinuousAutomatic => "&|Meas|S|A|Run|@  →  &|Meas|S|OK|@",
        CommandType.ContinuousManual => "&|Meas|S|M|Run|@  →  &|Meas|S|OK|@",
        _ => string.Empty
    };

    /// <summary>校验参数并生成完整报文。</summary>
    public static bool TryBuildMessage(
        CommandType command,
        string parameter,
        out string message,
        out string error)
    {
        message = string.Empty;
        error = string.Empty;
        parameter = parameter.Trim();

        if (RequiresParameter(command) && parameter.Length == 0)
        {
            error = command == CommandType.GetFocalLength ? "请输入 VID。" : "请输入配方名称。";
            return false;
        }

        if (parameter.IndexOfAny(['|', '&', '@', '\r', '\n']) >= 0)
        {
            error = "参数不能包含 |、&、@或换行符。";
            return false;
        }

        if (command == CommandType.GetFocalLength &&
            (!int.TryParse(parameter, out int vid) || vid <= 0))
        {
            error = "VID 必须是大于 0 的整数，例如 7500。";
            return false;
        }

        message = command switch
        {
            CommandType.Stop => "&|Stop|@",
            CommandType.SwitchRecipe => $"&|Elems|C|{parameter}|@",
            CommandType.SaveRecipe => $"&|Elems|S|{parameter}|@",
            CommandType.GetFocalLength => $"&|FF|{parameter}|@",
            CommandType.SingleAutomatic => "&|Meas|A|A|@",
            CommandType.SingleManual => "&|Meas|A|M|@",
            CommandType.ContinuousAutomatic => "&|Meas|S|A|@",
            CommandType.ContinuousManual => "&|Meas|S|M|@",
            _ => throw new ArgumentOutOfRangeException(nameof(command))
        };
        return true;
    }

    /// <summary>检查手工编辑的文本是否为一条完整报文。</summary>
    public static bool TryValidateMessage(string text, out string message, out string error)
    {
        message = text.Trim();
        error = string.Empty;

        if (message.Length == 0)
        {
            error = "请输入待发送报文。";
            return false;
        }

        if (message.Contains('\r') || message.Contains('\n'))
        {
            error = "每次只能发送一条报文，请删除换行符。";
            return false;
        }

        if (!message.StartsWith(StartMarker, StringComparison.Ordinal) ||
            !message.EndsWith(EndMarker, StringComparison.Ordinal))
        {
            error = "报文必须以 &| 开始，以 |@ 结束。";
            return false;
        }

        if (message.IndexOf(StartMarker, StartMarker.Length, StringComparison.Ordinal) >= 0 ||
            message.IndexOf(EndMarker, 0, message.Length - EndMarker.Length,
                StringComparison.Ordinal) >= 0)
        {
            error = "发送框中不能同时放置多条报文。";
            return false;
        }

        return true;
    }
}
