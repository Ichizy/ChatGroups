using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

//NOTE: Current solution relies on Moqs, however I think better solution would be to handle repository-related operation with in-memory storage.
//NOTE: As well I haven't had enough time for that, however ChatHub should be covered with tests as well.
namespace ChatGroupsTests.ProcessorTests
{
    public class OnGroupCreation
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
            _groupRepoMock.Setup(x => x.Create(It.IsAny<Group>(), It.IsAny<Client>())).Returns(Task.CompletedTask);

            _messageRepoMock = new Mock<IMessageRepository>();

            _processor = new Processor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }

        [Test]
        public async Task OnGroupCreation_ValidParams_Success()
        {
            var dto = DummyModels.GroupDto();
            var result = await _processor.OnGroupCreation(dto);

            _clientRepoMock.Verify(x => x.Get(It.IsAny<string>()), Times.Once);
            _groupRepoMock.Verify(x => x.Create(It.IsAny<Group>(), It.IsAny<Client>()), Times.Once);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void OnGroupCreation_NullArgumentPassed_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _processor.OnGroupCreation(null));
        }

        [Test]
        public void OnGroupCreation_ErrorInstorage_ErrorThrown()
        {
            _clientRepoMock.Setup(x => x.Get(It.IsAny<string>())).ThrowsAsync(new Exception());
            var dto = DummyModels.GroupDto();
            Assert.ThrowsAsync<Exception>(() => _processor.OnGroupCreation(dto));
        }
    }
}
