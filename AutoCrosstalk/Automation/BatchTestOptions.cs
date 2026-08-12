namespace AutoCrosstalk.Automation;

/// <summary>按文件夹内实际图片数量执行批量测试所需的参数。</summary>
public sealed class BatchTestOptions
{
    public required IReadOnlyList<string> ImageFiles { get; init; }
    public required string TargetMessage { get; init; }
    public DisplayTopology Topology { get; init; }
    public TimeSpan SwitchDelay { get; init; } = TimeSpan.FromSeconds(1);
    public TimeSpan TargetTimeout { get; init; } = TimeSpan.FromSeconds(10);
    public TimeSpan MeasurementTimeout { get; init; } = TimeSpan.FromSeconds(120);
    public bool ContinueAfterFailure { get; init; }
    public bool RestoreWallpaper { get; init; } = true;
}

public enum BatchImageStatus
{
    Waiting,
    Switching,
    Testing,
    Completed,
    Failed,
    Cancelled
}

public sealed class BatchProgressEventArgs(
    int imageIndex,
    string imagePath,
    BatchImageStatus status,
    int processedCount,
    string detail = "") : EventArgs
{
    public int ImageIndex { get; } = imageIndex;
    public string ImagePath { get; } = imagePath;
    public BatchImageStatus Status { get; } = status;
    public int ProcessedCount { get; } = processedCount;
    public string Detail { get; } = detail;
}
