using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatApp.Data;

public class ChatContext : IdentityDbContext<ChatUser, ChatRole, Guid>
{
    public ChatContext(DbContextOptions<ChatContext> options)
        : base(options)
    {
    }

    public DbSet<Message>? Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Message>()
            .HasOne(a => a.User)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.UserId);
    }
}
