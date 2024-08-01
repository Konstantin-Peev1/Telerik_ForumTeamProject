using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Hubs;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Tests.Hubs
{
    [TestClass]
    public class ChatHubTests
    {
        private Mock<IChatService> _chatServiceMock;
        private ChatHub _hub;
        private Mock<HubCallerContext> _mockContext;
        private Mock<IGroupManager> _mockGroups;
        private Mock<IClientProxy> _mockClients;
        private Mock<IHubCallerClients> _mockHubClients;

        [TestInitialize]
        public void SetUp()
        {
            _chatServiceMock = new Mock<IChatService>();
            _hub = new ChatHub(_chatServiceMock.Object);

            _mockContext = new Mock<HubCallerContext>();
            _mockGroups = new Mock<IGroupManager>();
            _mockClients = new Mock<IClientProxy>();
            _mockHubClients = new Mock<IHubCallerClients>();

            _hub.Context = _mockContext.Object;
            _hub.Groups = _mockGroups.Object;
            _hub.Clients = _mockHubClients.Object;
        }

       
        [TestMethod]
        public async Task SendMessage_ShouldSaveMessageAndNotifyClients()
        {
            // Arrange
            var chatRoomId = 1;
            var userId = 1;
            var userName = "TestUser";
            var message = "Hello, world!";

            _mockHubClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClients.Object);

            // Act
            await _hub.SendMessage(chatRoomId, userId, userName, message);

            // Assert
            _chatServiceMock.Verify(cs => cs.AddMessage(chatRoomId, userId, userName, message), Times.Once);
            _mockClients.Verify(client => client.SendCoreAsync("ReceiveMessage",
                It.Is<object[]>(o => o.Length == 2 &&
                                    o[0].ToString() == userName &&
                                    o[1].ToString() == message),
                default), Times.Once);
        }
    }
}
