using JobsityChatApp.Automapper;
using JobsityChatApp.Core;
using JobsityChatApp.Data;
using JobsityChatApp.Services;
using JobsityChatApp.Web;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobsityChatApp.Extensions;

public static class ServiceCollectionExtensions
{
    const string Queue = "Queue";

    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<ChatContext>(options => options.UseSqlServer(connectionString));
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddDefaultIdentity<ChatUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ChatContext>();
        services.AddControllersWithViews();
        services.AddSignalR((options) =>
        {
            options.EnableDetailedErrors = true;
        });
        var options = new BotOptions();
        config.GetSection("Bot").Bind(options);
        services.AddScoped<IMessageService, MessageService>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddAutoMapper(new Type[] { typeof(Mappings) });
        services.Configure<BotOptions>(config.GetSection("Bot"));
        services.Configure<QueueOptions>(config.GetSection(Queue));
        services.AddMassTransitMiddleware(config);
    }

    private static void AddMassTransitMiddleware(this IServiceCollection services, IConfiguration config)
    {
        var options = new QueueOptions();
        config.GetSection(Queue).Bind(options);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BotConsumer>();
            x.UsingRabbitMq((ctx, config) =>
            {
                config.AutoDelete = false;
                config.Exclusive = false;
                config.Durable = false;
                config.Host(options.Url);
                config.ReceiveEndpoint(options.QueueName, c =>
                {
                    c.ConfigureConsumer<BotConsumer>(ctx);
                });
            });
        });

        services.AddMassTransitHostedService();
    }
}

