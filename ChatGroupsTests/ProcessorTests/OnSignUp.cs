using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ChatGroupsTests.ProcessorTests
{
    public class OnSignUp
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

            _processor = new Processor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }

        [Test]
        public async Task OnSignUp_ValidClientInfo_ClientCreatedInStorage()
        {
            var clientDto = DummyModels.ClientDto();
            await _processor.OnSignUp(clientDto);

            _clientRepoMock.Verify(x => x.Add(It.Is<Client>(x => x.ConnectionId == clientDto.ConnectionId)), Times.Once);
        }

        [Test]
        public void OnSignUp_NullArgumentPassed_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _processor.OnSignUp(null));
        }

        [Test]
        public void OnSignUp_ErrorInstorage_ErrorThrown()
        {
            _clientRepoMock.Setup(x => x.Add(It.IsAny<Client>())).ThrowsAsync(new Exception());

            var clientDto = DummyModels.ClientDto();
            Assert.ThrowsAsync<Exception>(() => _processor.OnSignUp(clientDto));
        }
    }
}
