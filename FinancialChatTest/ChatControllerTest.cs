using FinancialChat.Controllers;
using FinancialChat.Data;
using FinancialChat.Hubs;
using FinancialChat.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using static FinancialChat.Controllers.ChatController;

namespace FinancialChatTest
{
    public class ChatControllerTest
    {
        private Mock<UserManager<IdentityUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<IHubContext<ChatHub>> GetMockHubContext()
        {
            var mockHubContext = new Mock<IHubContext<ChatHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockHubContext.Setup(context => context.Clients).Returns(mockClients.Object);

            return mockHubContext;
        }

        private Mock<IMessageQueueService> GetMockQueueService()
        {
            return new Mock<IMessageQueueService>();
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task SendMessage_Should_Save_Message_When_Not_StockCommand()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userManager = GetMockUserManager();
            var hubContext = GetMockHubContext();
            var queueService = GetMockQueueService();

            var user = new IdentityUser { UserName = "TestUser", Id = "1" };
            userManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var controller = new ChatController(context, userManager.Object, hubContext.Object, queueService.Object);
            var message = new SendMessageRequest { Message = "Hello" };

            // Act
            var result = await controller.SendMessage(message);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Single(context.ChatMessages);
            Assert.Equal("Hello", context.ChatMessages.First().Message);
        }

        [Fact]
        public async Task SendMessage_Should_Publish_Message_When_StockCommand()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userManager = GetMockUserManager();
            var hubContext = GetMockHubContext();
            var queueService = GetMockQueueService();

            var user = new IdentityUser { UserName = "TestUser", Id = "1" };
            userManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var controller = new ChatController(context, userManager.Object, hubContext.Object, queueService.Object);
            var message = new SendMessageRequest { Message = "/stock=aapl" };

            // Act
            var result = await controller.SendMessage(message);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Empty(context.ChatMessages);
            queueService.Verify(q => q.Publish(It.IsAny<string>()), Times.Once);
        }
    }
}