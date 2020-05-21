using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ChatGroupsTests.ProcessorTests
{
    public class OnGroupLeave
    {
        private const string groupId = "1234-5678-9012";
        private const string connectionId = "1234";

        private readonly Client _client = DummyModels.Client();
        private Processor _processor;
        private Mock<IClientRepository> _clientRepoMock;
        private Mock<IGroupRepository> _groupRepoMock;
        private Mock<IMessageRepository> _messageRepoMock;

        [SetUp]
        public void Setup()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _clientRepoMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(_client);

            _groupRepoMock = new Mock<IGroupRepository>();
            _groupRepoMock.Setup(x => x.LeaveGroup(connectionId, groupId)).Returns(Task.CompletedTask);

            _messageRepoMock = new Mock<IMessageRepository>();

            _processor = new Processor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }

        [Test]
        public async Task OnGroupLeave_ValidParams_Success()
        {
            var result = await _processor.OnGroupLeave(groupId, connectionId);

            _clientRepoMock.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
            _groupRepoMock.Verify(x => x.LeaveGroup(connectionId, groupId), Times.Once);
            Assert.AreEqual(_client.PublicName, result);
        }

        [Test]
        public void OnGroupLeave_ErrorInstorage_ErrorThrown()
        {
            _clientRepoMock.Setup(x => x.Get(It.IsAny<string>())).ThrowsAsync(new Exception());
            Assert.ThrowsAsync<Exception>(() => _processor.OnGroupLeave(groupId, connectionId));
        }

        [Test]
        public void OnGroupLeave_NullArgumentsPassed_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _processor.OnGroupLeave(null, null));
        }
    }
}
