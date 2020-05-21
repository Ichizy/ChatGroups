using ChatGroups.Services;
using Moq;
using ChatGroups.Data.Repositories;
using NUnit.Framework;

namespace ChatGroupsTests
{
    internal class GroupsProcessorTests
    {
        private GroupsProcessor _processor;
        private Mock<IClientRepository> _clientRepoMock;
        private Mock<IGroupRepository> _groupRepoMock;
        private Mock<IMessageRepository> _messageRepoMock;

        [SetUp]
        public void Setup()
        {
            _clientRepoMock = new Mock<IClientRepository>();
            _groupRepoMock = new Mock<IGroupRepository>();
            _messageRepoMock = new Mock<IMessageRepository>();
            //TODO: setup mocks

            _processor = new GroupsProcessor(_groupRepoMock.Object, _clientRepoMock.Object, _messageRepoMock.Object);
        }
    }
}
