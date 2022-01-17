using Jobsity;
using JobsityChatApp.Data;
using JobsityChatApp.Services;
using MassTransit;
using System.Security.Claims;

namespace JobsityChatApp.Web
{
    internal class BotConsumer : IConsumer<BotMessage>
    {
        private readonly IMessageService messageService;

        public BotConsumer(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public async Task Consume(ConsumeContext<BotMessage> context)
        {
            var message = new Message()
            {
                Created = DateTime.UtcNow,
                Text = context.Message!.Text,
                RoomId = context.Message!.RoomId,
            };

            var principal = GetBotIdentity();

            await messageService.CreateMessageAsync(message, principal);
            await messageService.SendMessageAsync(message);
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
}