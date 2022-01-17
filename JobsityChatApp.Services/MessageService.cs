using AutoMapper;
using JobsityChatApp.Core;
using JobsityChatApp.Data;
using JobsityChatApp.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Web;

namespace JobsityChatApp.Services;

public interface IMessageService
{
    Task CreateMessageAsync(Message message, ClaimsPrincipal? principal = null);
    Task SendMessageAsync(Message message);
    IQueryable<Message> GetMessages();
}

public class MessageService : IMessageService
{
    private readonly ChatContext chatContext;
    private readonly IHubContext<ChatHub> hubContext;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;

    public MessageService(ChatContext context,
        IHubContext<ChatHub> hubContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        this.chatContext = context;
        this.hubContext = hubContext;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateMessageAsync(Message message, ClaimsPrincipal? principal = null)
    {
        ClaimsPrincipal user = principal == null ?
            this.httpContextAccessor.HttpContext.User :
            principal;

        message.UserName = user.Identity!.Name!;
        var userId = user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        message.UserId = Guid.Parse(userId);

        message.Created = DateTime.UtcNow;
        message.Text = HttpUtility.HtmlEncode(message.Text);
        await chatContext.Messages!.AddAsync(message);
        await chatContext.SaveChangesAsync();
    }

    public Task SendMessageAsync(Message message)
    {
        var dto = this.mapper.Map<MessageDto>(message);
        return this.hubContext.Clients.Group(dto.RoomId.ToString()).SendAsync(Constants.ReceiveMessage, dto);
    }

    public IQueryable<Message> GetMessages()
    {
        return chatContext.Messages!.AsQueryable();
    }
}