using JobsityChatApp.Data;

namespace JobsityChatApp;

public static class DatabaseSeeder
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        var bot = new ChatUser()
        {
            UserName = Constants.BotUserName,
            Id = Constants.BotUserId,
        };
        using var scope = serviceProvider.CreateScope();
        var chatContext = scope.ServiceProvider.GetRequiredService<ChatContext>();

        if (!chatContext.Users.Any(x => x.Id == bot.Id))
        {
            await chatContext.Users.AddAsync(bot);
            await chatContext.SaveChangesAsync();
        }
    }
}
