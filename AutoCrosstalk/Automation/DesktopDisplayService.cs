using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace AutoCrosstalk.Automation;

/// <summary>
/// 调用 Windows API 切换桌面壁纸及投影模式。
/// 使用 Unicode API，确保包含中文的图片路径可以正常传入。
/// </summary>
public sealed class DesktopDisplayService : IDesktopDisplayService
{
    private const uint SpiGetDesktopWallpaper = 0x0073;
    private const uint SpiSetDesktopWallpaper = 0x0014;
    private const uint SpifUpdateIniFile = 0x0001;

    private const uint SdcApply = 0x00000080;
    private const uint SdcTopologyInternal = 0x00000001;
    private const uint SdcTopologyClone = 0x00000002;
    private const uint SdcTopologyExtend = 0x00000004;
    private const uint SdcTopologyExternal = 0x00000008;

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(
        uint uiAction,
        uint uiParam,
        string pvParam,
        uint fWinIni);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(
        uint uiAction,
        uint uiParam,
        StringBuilder pvParam,
        uint fWinIni);

    [DllImport("user32.dll")]
    private static extern int SetDisplayConfig(
        uint numPathArrayElements,
        IntPtr pathArray,
        uint numModeArrayElements,
        IntPtr modeArray,
        uint flags);

    public string? GetCurrentWallpaper()
    {
        var path = new StringBuilder(1024);
        return SystemParametersInfo(SpiGetDesktopWallpaper, (uint)path.Capacity, path, 0)
            ? path.ToString()
            : null;
    }

    public void ApplyTopology(DisplayTopology topology)
    {
        if (topology == DisplayTopology.None)
        {
            return;
        }

        uint topologyFlag = topology switch
        {
            DisplayTopology.Internal => SdcTopologyInternal,
            DisplayTopology.Clone => SdcTopologyClone,
            DisplayTopology.External => SdcTopologyExternal,
            DisplayTopology.Extend => SdcTopologyExtend,
            _ => throw new ArgumentOutOfRangeException(nameof(topology))
        };

        int result = SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, SdcApply | topologyFlag);
        if (result != 0)
        {
            throw new Win32Exception(result, "切换 Windows 投影模式失败。");
        }
    }

    public void SetWallpaper(string imagePath)
    {
        string fullPath = Path.GetFullPath(imagePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("桌面图片不存在。", fullPath);
        }

        // 参考 ZJS2310 的实现使用 BMP。Windows API 对 PNG 的兼容性因系统配置而异，
        // 因此先转换为 24 位 BMP 缓存，再执行切换，避免 API 返回成功但画面不变化。
        string wallpaperPath = ConvertToBmpIfNeeded(fullPath);

        bool success = SystemParametersInfo(
            SpiSetDesktopWallpaper,
            0,
            wallpaperPath,
            SpifUpdateIniFile);

        if (!success)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "切换桌面壁纸失败。");
        }
    }

    internal static string ConvertToBmpIfNeeded(string imagePath)
    {
        if (string.Equals(Path.GetExtension(imagePath), ".bmp", StringComparison.OrdinalIgnoreCase))
        {
            return imagePath;
        }

        var sourceInfo = new FileInfo(imagePath);
        string cacheKey = $"{sourceInfo.FullName}|{sourceInfo.Length}|{sourceInfo.LastWriteTimeUtc.Ticks}";
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cacheKey)))[..20];
        string cacheDirectory = Path.Combine(Path.GetTempPath(), "AutoCrosstalk", "WallpaperCache");
        Directory.CreateDirectory(cacheDirectory);
        string bmpPath = Path.Combine(cacheDirectory, $"wallpaper-{hash}.bmp");

        if (File.Exists(bmpPath))
        {
            return bmpPath;
        }

        string temporaryPath = bmpPath + ".tmp";
        try
        {
            using Image source = Image.FromFile(imagePath);
            using var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black);
                graphics.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            bitmap.Save(temporaryPath, ImageFormat.Bmp);
            File.Move(temporaryPath, bmpPath, overwrite: true);
            return bmpPath;
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }
}
