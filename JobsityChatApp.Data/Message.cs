namespace JobsityChatApp.Data;

public class Message
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime Created { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public virtual ChatUser? User { get; set; }
    public int RoomId { get; set; }
}
