using Microsoft.AspNetCore.Identity;

namespace JobsityChatApp.Data;

public class ChatUser : IdentityUser<Guid>
{
    public List<Message> Messages { get; set; } = new List<Message>();
}
