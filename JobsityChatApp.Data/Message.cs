namespace JobsityChatApp.Data;

public class Message
{
    public Message(string text, int roomId)
    {
        this.Text = text;
        this.RoomId = roomId;
        this.Created = DateTime.UtcNow;
    }
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime Created { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public virtual ChatUser? User { get; set; }
    public int RoomId { get; set; }
}
