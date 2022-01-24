using JobsityChatApp.Data;
using JobsityChatApp.Hubs;
using JobsityChatApp.Services;
using MassTransit;
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
        var messageService = new Mock<IMessageService>();
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var sut = new ChatHub(messageService.Object, publishEndpoint.Object);
        var msg = new Message(null!, 0);

        //act
        await sut.SendMessage(msg);

        //assert
        messageService.Verify(x => x.SendMessageAsync(It.IsAny<Message>()), Times.Never);
        messageService.Verify(x => x.CreateMessageAsync(It.IsAny<Message>(), null), Times.Never);
    }

    [Fact]
    public async Task SendMessage_NotNullStockText()
    {
        //arrange
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var messageService = new Mock<IMessageService>();

        var sut = new ChatHub(messageService.Object, publishEndpoint.Object);
        var msg = new Message("/stock=aapl.us", 1);

        //act
        await sut.SendMessage(msg);

        //assert
        messageService.Verify(x => x.SendMessageAsync(It.IsAny<Message>()), Times.Never);
        messageService.Verify(x => x.CreateMessageAsync(It.IsAny<Message>(), null), Times.Never);
    }

    [Fact]
    public async Task SendMessage_NotNullText()
    {
        //arrange
        var messageService = new Mock<IMessageService>();
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var sut = new ChatHub(messageService.Object, publishEndpoint.Object);
        var msg = new Message(Guid.NewGuid().ToString(), 1);


        //act
        await sut.SendMessage(msg);

        //assert
        messageService.Verify(x => x.SendMessageAsync(It.IsAny<Message>()), Times.Once);
        messageService.Verify(x => x.CreateMessageAsync(It.IsAny<Message>(), null), Times.Once);
    }
}
