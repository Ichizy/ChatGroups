using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGroupsTests.ProcessorTests
{
    public class OnGroupJoin
    {
        private const string groupId = "1234-5678-9012";
        private const string connectionId = "1234";

        private Processor _processor;
        private Mock<IClientRepository> _clientRepoMock;
        private Mock<IGroupRepository> _groupRepoMock;
        private Mock<IMessageRepository> _messageRepoMock;

        [SetUp]
        public void Setup()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _clientRepoMock.Setup(x => x.Get(connectionId)).ReturnsAsync(DummyModels.Client);

            _groupRepoMock = new Mock<IGroupRepository>();
            _groupRepoMock.Setup(x => x.AddClientToGroup(groupId, It.IsAny<Client>())).Returns(Task.CompletedTask);

            _messageRepoMock = new Mock<IMessageRepository>();
            _messageRepoMock.Setup(x => x.GetGroupHistory(groupId)).Returns(Task.FromResult(DummyModels.GroupHistory()));

            _processor = new Processor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }

        [Test]
        public async Task OnGroupJoin_ValidParams_Success()
        {
            var result = await _processor.OnGroupJoin(groupId, connectionId);

            _clientRepoMock.Verify(x => x.Get(connectionId), Times.Once);
            _groupRepoMock.Verify(x => x.AddClientToGroup(groupId, It.IsAny<Client>()), Times.Once);
            _messageRepoMock.Verify(x => x.GetGroupHistory(groupId), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Messages.Count);
            Assert.AreEqual(connectionId, result.Client.ConnectionId);
        }

        [Test]
        public async Task OnGroupJoin_NoMessageHistory_Success()
        {
            IList<Message> defaultHistory = new List<Message>();
            _messageRepoMock.Setup(x => x.GetGroupHistory(groupId))
                .Returns(Task.FromResult(defaultHistory));

            var result = await _processor.OnGroupJoin(groupId, connectionId);

            _clientRepoMock.Verify(x => x.Get(connectionId), Times.Once);
            _groupRepoMock.Verify(x => x.AddClientToGroup(groupId, It.IsAny<Client>()), Times.Once);
            _messageRepoMock.Verify(x => x.GetGroupHistory(groupId), Times.Once);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Messages.Count);
        }

        [Test]
        public void OnGroupJoin_NoClientFound_ExceptionThrown()
        {
            _clientRepoMock.Setup(x => x.Get(connectionId)).ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(() => _processor.OnGroupJoin(groupId, connectionId));
        }

        [Test]
        public void OnGroupJoin_NullArgumentsPassed_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _processor.OnGroupJoin(null, null));
        }
    }
}
