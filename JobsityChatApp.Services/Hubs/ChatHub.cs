using JobsityChatApp.Data;
using JobsityChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JobsityChatApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IBotApi botApi;
    private readonly IMessageService messageService;

    public ChatHub(IBotApi botApi, IMessageService messageService)
    {
        this.botApi = botApi;
        this.messageService = messageService;
    }

    public async Task AddToGroup(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

        await Clients.Group(roomName).SendAsync("userJoined", $"{Context.User.Identity!.Name} has joined room {roomName}.");
    }

    public async Task SendMessage(Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            return;
        }

        if (message.Text.StartsWith("/stock="))
        {
            var stockCode = message.Text.Substring(message.Text.IndexOf("=") + 1);

            await this.botApi.RequestQuote(stockCode, message.RoomId);
        }
        else
        {
            await this.messageService.CreateMessageAsync(message);

            await this.messageService.SendMessageAsync(message);
        }
    }
}