using ChatGroups.Services;
using Moq;
using ChatGroups.Data.Repositories;
using NUnit.Framework;
using ChatGroups.Data.Models;
using System.Threading.Tasks;
using ChatGroups.DTOs;

namespace ChatGroupsTests
{
    internal class GroupsProcessorTests
    {
        private Processor _processor;
        private Mock<IClientRepository> _clientRepoMock;
        private Mock<IGroupRepository> _groupRepoMock;
        private Mock<IMessageRepository> _messageRepoMock;

        [SetUp]
        public void Setup()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _clientRepoMock.Setup(x => x.Add(It.IsAny<Client>())).Returns(Task.CompletedTask);

            _groupRepoMock = new Mock<IGroupRepository>();
            _messageRepoMock = new Mock<IMessageRepository>();
            //TODO: setup mocks

            _processor = new Processor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }

        [Test]
        public async Task Login_ValidClientInfo_ClientCreatedInStorage()
        {
            var clientDto = new ClientDto
            {
                ConnectionId = "1234",
                Nickname = "bestMageEu"
            };
            await _processor.OnSignUp(clientDto);

            _clientRepoMock.Verify(x => x.Add(It.Is<Client>(x => x.ConnectionId == clientDto.ConnectionId)), Times.Once);
        }
    }
}
