using JobsityChatApp.Data;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatApp;

public static class DatabaseMigrator
{
    public static async Task MigrateDatabase(IServiceProvider serviceProvider)
    {
        var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        using var serviceScope = serviceScopeFactory.CreateScope();

        await using var context = serviceScope.ServiceProvider.GetService<ChatContext>();

        await context?.Database.MigrateAsync()!;
    }
}
