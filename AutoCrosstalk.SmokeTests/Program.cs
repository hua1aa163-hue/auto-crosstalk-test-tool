using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AutoCrosstalk;
using AutoCrosstalk.Automation;
using AutoCrosstalk.Models;
using AutoCrosstalk.Networking;
using AutoCrosstalk.Protocol;
using AutoCrosstalk.Settings;

const string expected = "&|Target|OK|1|brightness|0|White|9|0|1|84848|74434|143383|0|0.1|1|5,9,13|@";

var data = new TestMessage
{
    Target = "Target",
    Result = "OK",
    TestItem = 1,
    TestName = "brightness",
    ImageType = 0,
    CardName = "White",
    PointTemplateName = "9",
    ExposureMode = 0,
    RecognitionMode = 1,
    RedExposure = "84848",
    GreenExposure = "74434",
    BlueExposure = "143383",
    Gain = "0",
    Threshold = "0.1",
    EyePositionMode = 1,
    SpecifiedEyePositions = "5,9,13"
};

Assert(TestMessageCodec.Serialize(data) == expected, "序列化结果与协议示例不一致");
Assert(TestMessageCodec.TryDeserialize(expected, out TestMessage? parsed, out _), "协议示例无法解析");
Assert(parsed is
    {
        TestName: "brightness", EyePositionMode: 1,
        ExposureMode: 0, RecognitionMode: 1
    }, "解析后的字段不正确，或曝光方式和识别方式没有按两个独立字段解析");

string sourcePng = Path.Combine(Path.GetTempPath(), $"AutoCrosstalk-{Guid.NewGuid():N}.png");
string? convertedBmp = null;
try
{
    using (var bitmap = new System.Drawing.Bitmap(2, 2))
    {
        bitmap.Save(sourcePng, ImageFormat.Png);
    }

    convertedBmp = DesktopDisplayService.ConvertToBmpIfNeeded(sourcePng);
    using System.Drawing.Image convertedImage = System.Drawing.Image.FromFile(convertedBmp);
    Assert(Path.GetExtension(convertedBmp).Equals(".bmp", StringComparison.OrdinalIgnoreCase) &&
           convertedImage.RawFormat.Guid == ImageFormat.Bmp.Guid,
        "桌面图片没有转换为参考项目使用的 BMP 格式");
}
finally
{
    if (File.Exists(sourcePng)) File.Delete(sourcePng);
    if (!string.IsNullOrEmpty(convertedBmp) && File.Exists(convertedBmp)) File.Delete(convertedBmp);
}

string settingsFile = Path.Combine(Path.GetTempPath(), $"AutoCrosstalk-settings-{Guid.NewGuid():N}.json");
try
{
    var savedSettings = new AppSettings
    {
        MainForm = new MainFormSettings
        {
            Target = "SavedTarget",
            ResultOk = false,
            AutoExposure = false,
            AutoRecognition = true,
            SpecifiedEyes = "1,2,3",
            TimedSendIntervalMinutes = 2.5m
        },
        BatchForm = new BatchFormSettings
        {
            ImageDirectory = @"D:\saved-images",
            TopologyIndex = 2,
            TimedIntervalSeconds = 17
        }
    };
    AppSettingsStore.Save(savedSettings, settingsFile);
    AppSettings loadedSettings = AppSettingsStore.Load(settingsFile);
    Assert(loadedSettings.MainForm.Target == "SavedTarget" &&
           !loadedSettings.MainForm.ResultOk &&
           !loadedSettings.MainForm.AutoExposure &&
           loadedSettings.MainForm.AutoRecognition &&
           loadedSettings.MainForm.SpecifiedEyes == "1,2,3" &&
           loadedSettings.MainForm.TimedSendIntervalMinutes == 2.5m &&
           loadedSettings.BatchForm.ImageDirectory == @"D:\saved-images" &&
           loadedSettings.BatchForm.TopologyIndex == 2 &&
           loadedSettings.BatchForm.TimedIntervalSeconds == 17,
        "关闭前的界面设置无法保存并恢复");

    File.WriteAllText(settingsFile, """{"MainForm":null,"BatchForm":null}""");
    AppSettings repairedSettings = AppSettingsStore.Load(settingsFile);
    Assert(repairedSettings.MainForm is not null && repairedSettings.BatchForm is not null,
        "设置 JSON 包含 null 时没有恢复默认设置");
}
finally
{
    if (File.Exists(settingsFile)) File.Delete(settingsFile);
}

string discoveryDirectory = Path.Combine(Path.GetTempPath(), $"AutoCrosstalk-images-{Guid.NewGuid():N}");
try
{
    Directory.CreateDirectory(discoveryDirectory);
    File.WriteAllBytes(Path.Combine(discoveryDirectory, "1.png"), []);
    File.WriteAllBytes(Path.Combine(discoveryDirectory, "2.bmp"), []);
    File.WriteAllBytes(Path.Combine(discoveryDirectory, "3.jpg"), []);
    File.WriteAllBytes(Path.Combine(discoveryDirectory, "ignore.txt"), []);
    Assert(BatchTestForm.FindImageFiles(discoveryDirectory).Count == 3,
        "图片数量仍被固定，或没有按文件夹内实际图片数量加载");
}
finally
{
    if (Directory.Exists(discoveryDirectory)) Directory.Delete(discoveryDirectory, recursive: true);
}

await using var server = new TcpMessageServer();
var received = new List<string>();
var receivedTwice = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var clientConnected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
server.ClientConnected += (_, _) => clientConnected.TrySetResult();
server.MessageReceived += (_, message) =>
{
    received.Add(message);
    if (received.Count == 2) receivedTwice.TrySetResult();
};

await server.StartAsync(IPAddress.Loopback, 0);
using var client = new TcpClient();
await client.ConnectAsync(IPAddress.Loopback, server.Port);
await clientConnected.Task.WaitAsync(TimeSpan.FromSeconds(3));
using NetworkStream clientStream = client.GetStream();

await server.SendAsync(expected);
var readBuffer = new byte[Encoding.UTF8.GetByteCount(expected)];
int totalRead = 0;
while (totalRead < readBuffer.Length)
{
    int count = await clientStream.ReadAsync(readBuffer.AsMemory(totalRead));
    if (count == 0) break;
    totalRead += count;
}
Assert(Encoding.UTF8.GetString(readBuffer, 0, totalRead) == expected, "TCP 服务器发送内容不正确");

// 把两条报文拆成三段发送，覆盖 TCP 拆包和粘包场景。
byte[] combined = Encoding.UTF8.GetBytes(expected + expected);
await clientStream.WriteAsync(combined.AsMemory(0, 7));
await clientStream.WriteAsync(combined.AsMemory(7, 23));
await clientStream.WriteAsync(combined.AsMemory(30));

await receivedTwice.Task.WaitAsync(TimeSpan.FromSeconds(3));
Assert(received.SequenceEqual([expected, expected]), "TCP 接收拆包/粘包处理不正确");

await server.StopAsync();

// 验证完整自动流程：Target 只配置一次，每张图片各发送一次 Meas，完成后再切图。
await using var batchServer = new TcpMessageServer();
var batchClientConnected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
batchServer.ClientConnected += (_, _) => batchClientConnected.TrySetResult();
await batchServer.StartAsync(IPAddress.Loopback, 0);

using var batchClient = new TcpClient();
await batchClient.ConnectAsync(IPAddress.Loopback, batchServer.Port);
await batchClientConnected.Task.WaitAsync(TimeSpan.FromSeconds(3));
using NetworkStream batchClientStream = batchClient.GetStream();

Task clientProtocol = Task.Run(async () =>
{
    using var reader = new StreamReader(batchClientStream, Encoding.UTF8, false, 1024, leaveOpen: true);
    string target = await ReadMessageAsync(reader);
    Assert(target == expected, "批量流程的 Target 参数不正确");

    byte[] targetOk = Encoding.UTF8.GetBytes(MeasurementProtocol.TargetSuccess);
    await batchClientStream.WriteAsync(targetOk.AsMemory(0, 5));
    await batchClientStream.WriteAsync(targetOk.AsMemory(5));

    for (int index = 0; index < 2; index++)
    {
        string measure = await ReadMessageAsync(reader);
        Assert(measure == MeasurementProtocol.SingleAutoMeasure, "批量流程的 Meas 命令不正确");

        // Run 与完成应答粘在同一个 TCP 包中返回。
        byte[] responses = Encoding.UTF8.GetBytes(
            MeasurementProtocol.SingleAutoRunning + MeasurementProtocol.SingleAutoCompleted);
        await batchClientStream.WriteAsync(responses);
    }
});

var fakeDesktop = new FakeDesktopDisplayService();
var runner = new BatchTestRunner(batchServer, fakeDesktop);
var completedImages = new List<string>();
runner.ProgressChanged += (_, e) =>
{
    if (e.Status == BatchImageStatus.Completed) completedImages.Add(e.ImagePath);
};

string[] testImages = ["image-1.png", "image-2.png"];
await runner.RunAsync(new BatchTestOptions
{
    ImageFiles = testImages,
    TargetMessage = expected,
    Topology = DisplayTopology.Clone,
    SwitchDelay = TimeSpan.Zero,
    TargetTimeout = TimeSpan.FromSeconds(3),
    MeasurementTimeout = TimeSpan.FromSeconds(3),
    RestoreWallpaper = false
}, CancellationToken.None);

await clientProtocol.WaitAsync(TimeSpan.FromSeconds(3));
Assert(fakeDesktop.AppliedTopology == DisplayTopology.Clone, "批量流程未应用投影模式");
Assert(fakeDesktop.AppliedTopologies.SequenceEqual(
        new[] { DisplayTopology.Clone, DisplayTopology.Clone }),
    "没有在每张图片切换后刷新桌面模式");
Assert(fakeDesktop.Wallpapers.SequenceEqual(testImages), "桌面图片切换顺序不正确");
Assert(completedImages.SequenceEqual(testImages), "未按 Meas 完成应答结束每张图片");

await batchServer.StopAsync();

// 即使勾选“单图失败后继续”，TCP 断开后也必须立即停止切图。
await using var disconnectServer = new TcpMessageServer();
var disconnectClientConnected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
disconnectServer.ClientConnected += (_, _) => disconnectClientConnected.TrySetResult();
await disconnectServer.StartAsync(IPAddress.Loopback, 0);

using var disconnectClient = new TcpClient();
await disconnectClient.ConnectAsync(IPAddress.Loopback, disconnectServer.Port);
await disconnectClientConnected.Task.WaitAsync(TimeSpan.FromSeconds(3));
NetworkStream disconnectStream = disconnectClient.GetStream();

Task disconnectProtocol = Task.Run(async () =>
{
    using var reader = new StreamReader(disconnectStream, Encoding.UTF8, false, 1024, leaveOpen: true);
    Assert(await ReadMessageAsync(reader) == expected, "断线测试的 Target 参数不正确");
    await disconnectStream.WriteAsync(Encoding.UTF8.GetBytes(MeasurementProtocol.TargetSuccess));
    Assert(await ReadMessageAsync(reader) == MeasurementProtocol.SingleAutoMeasure,
        "断线测试的 Meas 命令不正确");
    disconnectClient.Close();
});

var disconnectDesktop = new FakeDesktopDisplayService();
var disconnectRunner = new BatchTestRunner(disconnectServer, disconnectDesktop);
bool stoppedOnDisconnect = false;
try
{
    await disconnectRunner.RunAsync(new BatchTestOptions
    {
        ImageFiles = ["disconnect-1.png", "disconnect-2.png"],
        TargetMessage = expected,
        SwitchDelay = TimeSpan.Zero,
        TargetTimeout = TimeSpan.FromSeconds(3),
        MeasurementTimeout = TimeSpan.FromSeconds(3),
        ContinueAfterFailure = true,
        RestoreWallpaper = false
    }, CancellationToken.None);
}
catch (IOException)
{
    stoppedOnDisconnect = true;
}

await disconnectProtocol.WaitAsync(TimeSpan.FromSeconds(3));
Assert(stoppedOnDisconnect && disconnectDesktop.Wallpapers.Count == 1,
    "TCP 断开后仍在继续切换后续图片");
await disconnectServer.StopAsync();

// 在 STA 线程构造 Designer 窗体，不显示、不修改桌面，仅检查控件初始化。
Exception? designerException = null;
var designerThread = new Thread(() =>
{
    try
    {
        using var form = new BatchTestForm();
        Assert(form.ClientSize.Width == 1040 && form.ClientSize.Height == 730,
            "批量测试窗体尺寸不正确");
        Assert(FindControl<Button>(form, "btnTestDesktop").Text == "测试切换下一张",
            "批量窗口缺少独立桌面切换测试按钮");
        Assert(FindControl<Button>(form, "btnTimedProjection").Text == "开始定时投图" &&
               FindControl<NumericUpDown>(form, "numTimedInterval").Value == 5 &&
               FindControl<Button>(form, "btnStart").Text == "开始批量测试",
            "批量窗口缺少定时投图设置或仍使用固定 9 图文案");

        using var mainForm = new MainForm();
        var host = FindControl<TextBox>(mainForm, "txtHost");
        var port = FindControl<NumericUpDown>(mainForm, "nudPort");
        var listenButton = FindControl<Button>(mainForm, "btnConnect");
        Assert(host.Text == "127.0.0.1" && host.ReadOnly &&
               port.Value == 9527 && port.ReadOnly &&
               listenButton.Text == "启动监听",
            "TCP 服务器界面没有固定为 127.0.0.1:9527");
        var testType = FindControl<ComboBox>(mainForm, "cmbTestType");
        var autoExposure = FindControl<RadioButton>(mainForm, "rbAutoExposure");
        var manualExposure = FindControl<RadioButton>(mainForm, "rbManualExposure");
        var autoRecognition = FindControl<RadioButton>(mainForm, "rbAutoRecognition");
        var thresholdRecognition = FindControl<RadioButton>(mainForm, "rbThresholdRecognition");
        var specifiedEye = FindControl<RadioButton>(mainForm, "rbSpecifiedEye");
        var specifiedEyes = FindControl<TextBox>(mainForm, "txtSpecifiedEyes");
        var preview = FindControl<TextBox>(mainForm, "txtSendPreview");
        var timedSendInterval = FindControl<NumericUpDown>(mainForm, "numTimedSendInterval");
        var timedSendButton = FindControl<Button>(mainForm, "btnTimedSend");

        Assert(timedSendInterval.Value == 1 &&
               timedSendInterval.Minimum == 0.1m &&
               timedSendButton.Text == "开始定时发送",
            "主界面缺少按分钟循环发送的控件");

        Assert(autoExposure.Parent != autoRecognition.Parent,
            "曝光方式和识别方式仍位于同一个单选按钮容器");
        testType.SelectedIndex = 1;

        manualExposure.Checked = true;
        autoRecognition.Checked = true;
        Assert(TestMessageCodec.TryDeserialize(preview.Text, out TestMessage? manualAuto, out _) &&
               manualAuto is { ExposureMode: 1, RecognitionMode: 0 },
            "手动曝光 + 自动识别没有实时写入两个独立字段");

        autoExposure.Checked = true;
        thresholdRecognition.Checked = true;
        Assert(TestMessageCodec.TryDeserialize(preview.Text, out TestMessage? autoThreshold, out _) &&
               autoThreshold is { ExposureMode: 0, RecognitionMode: 1 },
            "自动曝光 + 阈值识别没有实时写入两个独立字段");

        specifiedEye.Checked = true;
        specifiedEyes.Text = string.Empty;
        Assert(!TestMessageCodec.TryDeserialize(preview.Text, out _, out _),
            "指定眼位模式仍允许发送空眼位");
        specifiedEyes.Text = "5,9,13";
    }
    catch (Exception ex)
    {
        designerException = ex;
    }
});
designerThread.SetApartmentState(ApartmentState.STA);
designerThread.Start();
designerThread.Join();
if (designerException is not null) throw designerException;

Console.WriteLine("Smoke tests passed: protocol + TCP framing + timed settings + automatic workflow + Designer form.");

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static async Task<string> ReadMessageAsync(StreamReader reader)
{
    var message = new StringBuilder();
    var buffer = new char[1];
    while (true)
    {
        int count = await reader.ReadAsync(buffer);
        if (count == 0) throw new EndOfStreamException("连接在完整报文到达前关闭。");
        message.Append(buffer[0]);
        if (message.ToString().EndsWith(TestMessageCodec.EndMarker, StringComparison.Ordinal))
        {
            return message.ToString();
        }
    }
}

static T FindControl<T>(Control root, string name) where T : Control
{
    if (root is T match && root.Name == name) return match;
    foreach (Control child in root.Controls)
    {
        T? found = FindControlOrDefault<T>(child, name);
        if (found is not null) return found;
    }

    throw new InvalidOperationException($"未找到控件：{name}");
}

static T? FindControlOrDefault<T>(Control root, string name) where T : Control
{
    if (root is T match && root.Name == name) return match;
    foreach (Control child in root.Controls)
    {
        T? found = FindControlOrDefault<T>(child, name);
        if (found is not null) return found;
    }

    return null;
}

sealed class FakeDesktopDisplayService : IDesktopDisplayService
{
    public List<string> Wallpapers { get; } = [];
    public List<DisplayTopology> AppliedTopologies { get; } = [];
    public DisplayTopology AppliedTopology { get; private set; }

    public string? GetCurrentWallpaper() => null;
    public void ApplyTopology(DisplayTopology topology)
    {
        AppliedTopology = topology;
        AppliedTopologies.Add(topology);
    }
    public void SetWallpaper(string imagePath) => Wallpapers.Add(imagePath);
}
