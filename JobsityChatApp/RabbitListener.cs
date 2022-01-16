using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using JobsityChatApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using JobsityChatApp.Core;

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
    private readonly IHubContext<ChatHub> hubContext;
    private readonly QueueOptions options;

    public RabbitListener(IOptions<QueueOptions> options, IHubContext<ChatHub> hubContext)
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
        this.hubContext = hubContext;
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
            var msg = new MessageDto()
            {
                Created = DateTime.UtcNow,
                Text = Encoding.UTF8.GetString(ea.Body.ToArray()),
                UserName = "jobsity bot",
            };
            await this.hubContext.Clients.All.SendAsync(Constants.ReceiveMessage, msg);
        };
        channel.BasicConsume(queue: this.options.QueueName,
                             autoAck: true,
                             consumer: consumer);
    }

    public void Deregister()
    {
        this.connection.Close();
    }
}

