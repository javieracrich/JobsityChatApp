using JobsityChatApp.Core;
using JobsityChatApp.Data;
using JobsityChatApp.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace JobsityChatApp;

public interface IRabbitListener
{
    void Register();
    void Deregister();
}

public class RabbitListener : IRabbitListener
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly IServiceProvider serviceProvider;
    private readonly QueueOptions options;

    public RabbitListener(IOptions<QueueOptions> options, IServiceProvider serviceProvider)
    {
        this.options = options.Value;
        var factory = new ConnectionFactory()
        {
            HostName = this.options.Url,
            UserName = this.options.UserName,
            Password = this.options.Password,
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        this.serviceProvider = serviceProvider;
    }

    public void Register()
    {
        channel.QueueDeclare(queue: this.options.QueueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var queueMessage = JsonSerializer.Deserialize<QueueMessage>(Encoding.UTF8.GetString(ea.Body.ToArray()));

            var message = new Message()
            {
                Created = DateTime.UtcNow,
                Text = queueMessage!.Text,
                RoomId = queueMessage!.RoomId,
            };

            var principal = GetBotIdentity();

            using var scope = serviceProvider.CreateScope();
            var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
            await messageService.CreateMessageAsync(message, principal);
            await messageService.SendMessageAsync(message);
        };
        channel.BasicConsume(queue: this.options.QueueName,
                             autoAck: true,
                             consumer: consumer);
    }

    public void Deregister()
    {
        this.connection.Close();
    }

    private static ClaimsPrincipal GetBotIdentity()
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Constants.BotUserName),
                new Claim(ClaimTypes.NameIdentifier,Constants.BotUserId.ToString())
            };

        var identity = new ClaimsIdentity(claims, "Bearer");
        return new ClaimsPrincipal(identity);
    }
}
