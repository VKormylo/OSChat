namespace logic;

public class Message
{
    public string Text { get; set; } = string.Empty;

    public bool IsReceived { get; set; }
    
    public DateTime TimeStamp { get; set; }
    
    public Message()
    {}

    public Message(string text, bool isReceived = false)
    {
        Text = text;
        IsReceived = isReceived;
        TimeStamp = DateTime.Now;
    }
}