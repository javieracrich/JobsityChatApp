using JobsityChatApp.Automapper;
using JobsityChatApp.Core;
using JobsityChatApp.Data;
using JobsityChatApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobsityChatApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ChatContext>(options => options.UseSqlServer(connectionString));
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddDefaultIdentity<ChatUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ChatContext>();
        services.AddControllersWithViews();
        services.AddSignalR((options) =>
        {
            options.EnableDetailedErrors = true;
        });

        services.AddScoped<IBotService, BotService>();
        services.AddScoped<IMessageService, MessageService>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddAutoMapper(new Type[] { typeof(Mappings) });
        services.Configure<BotOptions>(configuration.GetSection("Bot"));
        services.Configure<QueueOptions>(configuration.GetSection("Queue"));
        services.AddSingleton<IRabbitListener, RabbitListener>();
    }
}

