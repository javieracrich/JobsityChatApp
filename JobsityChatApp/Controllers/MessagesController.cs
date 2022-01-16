using AutoMapper;
using JobsityChatApp.Core;
using JobsityChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService messageService;
    private readonly IMapper mapper;

    public MessagesController(IMessageService messageService, IMapper mapper)
    {
        this.messageService = messageService;
        this.mapper = mapper;
    }


    [HttpGet("{roomId}")]
    public async Task<List<MessageDto>> GetMessages(int roomId)
    {
        var messages = await this.messageService.GetMessages()
                          .Where(x => x.RoomId == roomId)
                          .OrderBy(x => x.Created)
                          .Take(50)
                          .ToListAsync();

        return this.mapper.Map<List<MessageDto>>(messages);
    }
}

