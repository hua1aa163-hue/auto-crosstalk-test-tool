namespace AutoCrosstalk.Automation;

/// <summary>Windows 投影模式。None 表示仅切换壁纸，不改变显示器拓扑。</summary>
public enum DisplayTopology
{
    None = 0,
    Internal = 1,
    Clone = 2,
    External = 3,
    Extend = 4
}
