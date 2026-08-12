namespace AutoCrosstalk.Automation;

/// <summary>抽象桌面操作，便于把 Windows API 与批量测试流程分开测试。</summary>
public interface IDesktopDisplayService
{
    string? GetCurrentWallpaper();
    void ApplyTopology(DisplayTopology topology);
    void SetWallpaper(string imagePath);
}
