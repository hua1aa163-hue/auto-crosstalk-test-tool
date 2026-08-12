namespace SimpleProtocolServer.Protocol;

/// <summary>按列表顺序取出报文，到达末尾后自动回到第一条。</summary>
internal sealed class CycleMessageSequence
{
    private string[] _messages = [];
    private int _nextIndex;

    public int Count => _messages.Length;

    public void Reset(IEnumerable<string> messages)
    {
        _messages = messages.ToArray();
        _nextIndex = 0;
    }

    public bool TryGetNext(out string message, out int position)
    {
        if (_messages.Length == 0)
        {
            message = string.Empty;
            position = 0;
            return false;
        }

        position = _nextIndex + 1;
        message = _messages[_nextIndex];
        _nextIndex = (_nextIndex + 1) % _messages.Length;
        return true;
    }
}
