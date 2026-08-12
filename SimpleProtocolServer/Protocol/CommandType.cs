namespace SimpleProtocolServer.Protocol;

/// <summary>界面支持的 8 类命令，顺序与下拉框一致。</summary>
internal enum CommandType
{
    Stop,
    SwitchRecipe,
    SaveRecipe,
    GetFocalLength,
    SingleAutomatic,
    SingleManual,
    ContinuousAutomatic,
    ContinuousManual
}
