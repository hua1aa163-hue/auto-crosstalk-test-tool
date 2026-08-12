using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleProtocolServer;
using SimpleProtocolServer.Networking;
using SimpleProtocolServer.Protocol;

var protocolCases = new (CommandType Command, string Parameter, string Expected)[]
{
    (CommandType.Stop, "", "&|Stop|@"),
    (CommandType.SwitchRecipe, "qwerty", "&|Elems|C|qwerty|@"),
    (CommandType.SaveRecipe, "asdfgh", "&|Elems|S|asdfgh|@"),
    (CommandType.GetFocalLength, "7500", "&|FF|7500|@"),
    (CommandType.SingleAutomatic, "", "&|Meas|A|A|@"),
    (CommandType.SingleManual, "", "&|Meas|A|M|@"),
    (CommandType.ContinuousAutomatic, "", "&|Meas|S|A|@"),
    (CommandType.ContinuousManual, "", "&|Meas|S|M|@")
};

foreach ((CommandType command, string parameter, string expected) in protocolCases)
{
    Assert(SimpleMessageProtocol.TryBuildMessage(command, parameter, out string message, out _) &&
           message == expected,
        $"报文生成不正确：{command}");
}

Assert(!SimpleMessageProtocol.TryBuildMessage(CommandType.SwitchRecipe, "", out _, out _),
    "切换配方时仍允许空配方名");
Assert(!SimpleMessageProtocol.TryBuildMessage(CommandType.SaveRecipe, "bad|name", out _, out _),
    "配方名称仍允许使用协议保留字符");
Assert(!SimpleMessageProtocol.TryBuildMessage(CommandType.GetFocalLength, "0", out _, out _),
    "VID 仍允许非正整数");
Assert(SimpleMessageProtocol.TryValidateMessage("&|Custom|123|@", out string editable, out _) &&
       editable == "&|Custom|123|@",
    "手工编辑的完整报文无法发送");
Assert(!SimpleMessageProtocol.TryValidateMessage("&|Stop|@&|FF|7500|@", out _, out _),
    "发送框仍允许同时填入多条报文");

var cycle = new CycleMessageSequence();
cycle.Reset(["&|Stop|@", "&|FF|7500|@"]);
Assert(cycle.TryGetNext(out string cycle1, out int position1) &&
       cycle.TryGetNext(out string cycle2, out int position2) &&
       cycle.TryGetNext(out string cycle3, out int position3) &&
       (cycle1, position1) == ("&|Stop|@", 1) &&
       (cycle2, position2) == ("&|FF|7500|@", 2) &&
       (cycle3, position3) == ("&|Stop|@", 1),
    "不同报文没有按列表顺序循环");

await using var server = new TcpMessageServer();
var connected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var received = new List<string>();
var receivedTwice = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
server.ClientConnected += (_, _) => connected.TrySetResult();
server.MessageReceived += (_, message) =>
{
    received.Add(message);
    if (received.Count == 2) receivedTwice.TrySetResult();
};

await server.StartAsync(IPAddress.Loopback, 0);
using var client = new TcpClient();
await client.ConnectAsync(IPAddress.Loopback, server.Port);
await connected.Task.WaitAsync(TimeSpan.FromSeconds(3));
using NetworkStream stream = client.GetStream();

string outgoing = "&|Stop|@";
await server.SendAsync(outgoing);
byte[] readBuffer = new byte[Encoding.UTF8.GetByteCount(outgoing)];
int totalRead = 0;
while (totalRead < readBuffer.Length)
{
    int count = await stream.ReadAsync(readBuffer.AsMemory(totalRead));
    if (count == 0) break;
    totalRead += count;
}
Assert(Encoding.UTF8.GetString(readBuffer, 0, totalRead) == outgoing,
    "TCP 服务器发送的报文不正确");

// 两条返回报文分三段发送，同时验证拆包与粘包。
string response1 = "&|Stop|OK|@";
string response2 = "&|Meas|A|A|Run|@";
byte[] combined = Encoding.UTF8.GetBytes(response1 + response2);
await stream.WriteAsync(combined.AsMemory(0, 5));
await stream.WriteAsync(combined.AsMemory(5, 11));
await stream.WriteAsync(combined.AsMemory(16));
await receivedTwice.Task.WaitAsync(TimeSpan.FromSeconds(3));
Assert(received.SequenceEqual([response1, response2]), "TCP 拆包或粘包处理不正确");
await server.StopAsync();

Exception? designerException = null;
var designerThread = new Thread(() =>
{
    try
    {
        using var form = new MainForm();
        var host = FindControl<TextBox>(form, "txtHost");
        var port = FindControl<NumericUpDown>(form, "numPort");
        var commands = FindControl<ComboBox>(form, "cmbCommand");
        var parameter = FindControl<TextBox>(form, "txtParameter");
        var preview = FindControl<TextBox>(form, "txtSendPreview");
        var cycleMessages = FindControl<ListBox>(form, "lstCycleMessages");
        var addCycle = FindControl<Button>(form, "btnAddCycle");
        var interval = FindControl<NumericUpDown>(form, "numIntervalMinutes");
        var timedButton = FindControl<Button>(form, "btnTimedSend");

        Assert(host.Text == "127.0.0.1" && host.ReadOnly && port.Value == 9527 && port.ReadOnly,
            "服务器界面没有固定为 127.0.0.1:9527");
        Assert(commands.Items.Count == 8 && !preview.ReadOnly &&
               cycleMessages.Items.Count == 0 && addCycle.Text == "加入循环列表" &&
               interval.Value == 1 &&
               timedButton.Text == "开始定时发送",
            "界面缺少可编辑发送框、循环列表或定时发送控件");

        commands.SelectedIndex = (int)CommandType.SwitchRecipe;
        Assert(parameter.Text == "qwerty" && preview.Text == "&|Elems|C|qwerty|@",
            "选择切换配方后没有实时生成报文");
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

Console.WriteLine("SimpleProtocolServer smoke tests passed: editable messages + cyclic queue + TCP framing + Designer form.");

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static T FindControl<T>(Control root, string name) where T : Control
{
    if (root is T match && root.Name == name) return match;
    foreach (Control child in root.Controls)
    {
        try
        {
            return FindControl<T>(child, name);
        }
        catch (InvalidOperationException)
        {
            // 继续查找其他子控件。
        }
    }

    throw new InvalidOperationException($"未找到控件：{name}");
}
