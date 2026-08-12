using System.Text.Json;

namespace AutoCrosstalk.Settings;

/// <summary>以 JSON 保存用户设置；写入使用临时文件替换，避免异常退出留下半个文件。</summary>
public static class AppSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static string SettingsPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AutoCrosstalk", "settings.json");

    public static AppSettings Load() => Load(SettingsPath);

    public static void Save(AppSettings settings) => Save(settings, SettingsPath);

    internal static AppSettings Load(string path)
    {
        try
        {
            if (!File.Exists(path)) return new AppSettings();
            string json = File.ReadAllText(path);
            AppSettings settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();

            // 手工修改或旧版 JSON 可能包含 null，恢复默认值避免启动崩溃。
            settings.MainForm ??= new MainFormSettings();
            settings.BatchForm ??= new BatchFormSettings();
            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }

    internal static void Save(AppSettings settings, string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        string temporaryPath = path + ".tmp";
        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(settings, JsonOptions));
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
    }
}
