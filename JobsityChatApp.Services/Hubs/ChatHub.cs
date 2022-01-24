using Jobsity;
using JobsityChatApp.Data;
using JobsityChatApp.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JobsityChatApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService messageService;
    private readonly IPublishEndpoint publishEndpoint;

    public ChatHub(
        IMessageService messageService,
        IPublishEndpoint publishEndpoint)
    {
        this.messageService = messageService;
        this.publishEndpoint = publishEndpoint;
    }

    public async Task AddToGroup(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        await Clients.Group(roomName).SendAsync("userJoined", $"{Context.User.Identity!.Name} has joined room {roomName}.");
    }

    public async Task SendMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text) || message.RoomId <= 0)
        {
            return;
        }

        if (message.Text.StartsWith("/stock="))
        {
            var stockCode = message.Text[(message.Text.IndexOf("=") + 1)..];

            await this.publishEndpoint.Publish(new AppMessage(stockCode, message.RoomId));
        }
        else
        {
            await this.messageService.CreateMessageAsync(message);

            await this.messageService.SendMessageAsync(message);
        }
    }
}