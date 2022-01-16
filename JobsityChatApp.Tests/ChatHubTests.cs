using JobsityChatApp.Data;
using JobsityChatApp.Hubs;
using JobsityChatApp.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace JobsityChatApp.Tests;

public class ChatHubTests
{
    [Fact]
    public async Task SendMessage_NullText()
    {
        //arrange
        var botService = new Mock<IBotService>();
        var messageService = new Mock<IMessageService>();

        var sut = new ChatHub(botService.Object, messageService.Object);
        var msg = new Message()
        {
            Created = DateTime.UtcNow,
            Text = null,
            UserId = Guid.NewGuid(),
            UserName = Guid.NewGuid().ToString()
        };


        //act
        await sut.SendMessage(msg);

        //assert
        botService.Verify(x => x.RequestQuote(It.IsAny<string>(), 1), Times.Never);
        messageService.Verify(x => x.SendMessageAsync(It.IsAny<Message>()), Times.Never);
        messageService.Verify(x => x.CreateMessageAsync(It.IsAny<Message>(), null), Times.Never);
    }

    [Fact]
    public async Task SendMessage_NotNullStockText()
    {
        //arrange
        var botService = new Mock<IBotService>();
        var messageService = new Mock<IMessageService>();

        var sut = new ChatHub(botService.Object, messageService.Object);
        var msg = new Message()
        {
            Created = DateTime.UtcNow,
            Text = "/stock=aapl.us",
            UserId = Guid.NewGuid(),
            UserName = Guid.NewGuid().ToString(),
            RoomId = 1
        };

        //act
        await sut.SendMessage(msg);

        //assert
        botService.Verify(x => x.RequestQuote(It.IsAny<string>(), 1), Times.Once);
        messageService.Verify(x => x.SendMessageAsync(It.IsAny<Message>()), Times.Never);
        messageService.Verify(x => x.CreateMessageAsync(It.IsAny<Message>(), null), Times.Never);
    }

    [Fact]
    public async Task SendMessage_NotNullText()
    {
        //arrange
        var botService = new Mock<IBotService>();
        var messageService = new Mock<IMessageService>();

        var sut = new ChatHub(botService.Object, messageService.Object);
        var msg = new Message()
        {
            Created = DateTime.UtcNow,
            Text = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid(),
            UserName = Guid.NewGuid().ToString()
        };

        //act
        await sut.SendMessage(msg);

        //assert
        botService.Verify(x => x.RequestQuote(It.IsAny<string>(), 1), Times.Never);
        messageService.Verify(x => x.SendMessageAsync(It.IsAny<Message>()), Times.Once);
        messageService.Verify(x => x.CreateMessageAsync(It.IsAny<Message>(), null), Times.Once);
    }
}
