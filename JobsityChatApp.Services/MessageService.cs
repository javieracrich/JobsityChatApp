using AutoMapper;
using JobsityChatApp.Core;
using JobsityChatApp.Data;
using JobsityChatApp.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Web;

namespace JobsityChatApp.Services;

public interface IMessageService
{
    Task CreateMessage(Message message);
    Task SendMessage(Message message);
    IQueryable<Message> GetMessages();
}

public class MessageService : IMessageService
{
    private readonly ChatContext chatContext;
    private readonly UserManager<ChatUser> userManager;
    private readonly IHubContext<ChatHub> hubContext;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    public MessageService(ChatContext context,
        UserManager<ChatUser> userManager,
        IHubContext<ChatHub> hubContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        this.chatContext = context;
        this.userManager = userManager;
        this.hubContext = hubContext;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateMessage(Message message)
    {
        var user = this.httpContextAccessor.HttpContext.User;
        message.UserName = user.Identity!.Name!;
        var sender = await this.userManager.GetUserAsync(user);
        message.UserId = sender.Id;
        message.Created = DateTime.UtcNow;
        message.Text = HttpUtility.HtmlEncode(message.Text);
        await chatContext.Messages!.AddAsync(message);
        await chatContext.SaveChangesAsync();
    }

    public Task SendMessage(Message message)
    {
        var dto = this.mapper.Map<MessageDto>(message);
        return this.hubContext.Clients.All.SendAsync(Constants.ReceiveMessage, dto);
    }

    public IQueryable<Message> GetMessages()
    {
        return chatContext.Messages!.AsQueryable();
    }
}
