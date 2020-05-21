using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ChatGroupsTests.ProcessorTests
{
    public class OnMessageSent
    {
        private Processor _processor;
        private Mock<IClientRepository> _clientRepoMock;
        private Mock<IGroupRepository> _groupRepoMock;
        private Mock<IMessageRepository> _messageRepoMock;

        [SetUp]
        public void Setup()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _clientRepoMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(DummyModels.Client);

            _groupRepoMock = new Mock<IGroupRepository>();
            _groupRepoMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(DummyModels.Group);

            _messageRepoMock = new Mock<IMessageRepository>();
            _messageRepoMock.Setup(x => x.Add(It.IsAny<Message>())).Returns(Task.CompletedTask);

            _processor = new Processor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }

        [Test]
        public async Task OnMessageSent_ValidParams_Success()
        {
            var dto = DummyModels.MessageDto();
            await _processor.OnMessageSent(dto);

            _clientRepoMock.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
            _groupRepoMock.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
            _messageRepoMock.Verify(x => x.Add(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public async Task OnMessageSent_SentNotToGroup_Success()
        {
            var dto = DummyModels.MessageDto();
            dto.SentToGroup = false;
            dto.GroupId = null;
            await _processor.OnMessageSent(dto);

            _clientRepoMock.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
            _groupRepoMock.Verify(x => x.Get(It.IsAny<string>()), Times.Never);
            _messageRepoMock.Verify(x => x.Add(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public void OnMessageSent_NoGroupFound_ExceptionThrown()
        {
            _groupRepoMock.Setup(x => x.Get(It.IsAny<string>())).ThrowsAsync(new Exception());

            var dto = DummyModels.MessageDto();
            Assert.ThrowsAsync<Exception>(() => _processor.OnMessageSent(dto));
        }

        [Test]
        public void OnMessageSent_NullArgumentPassed_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _processor.OnMessageSent(null));
        }
    }
}
