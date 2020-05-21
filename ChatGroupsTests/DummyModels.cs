using ChatGroups.Data.Models;
using ChatGroups.DTOs;
using ChatGroups.Models;
using System;
using System.Collections.Generic;

namespace ChatGroupsTests
{
    public class DummyModels
    {
        public static GroupDto GroupDto()
        {
            return new GroupDto
            {
                MaximumClientsAmount = 20,
                ClientsConnected = new List<string> { "1234" },
                CreatorConnectionId = "1234",
                Name = "testGroup",
                PublicId = Guid.NewGuid().ToString()
            };
        }

        public static Group Group()
        {
            return new Group("best group")
            {
                Id = 1
            };
        }

        public static ClientDto ClientDto()
        {
            return new ClientDto
            {
                ConnectionId = "1234",
                Nickname = "bestMageEu"
            };
        }

        public static Client Client()
        {
            return new Client
            {
                Id = 1,
                ConnectionId = "1234",
                PublicName = "bestMageEu"
            };
        }

        public static MessageDto MessageDto()
        {
            return new MessageDto
            {
                Body = "Hello World",
                GroupId = "1",
                SenderConnectionId = "1234",
                SenderName = "bestMageEu",
                SentToGroup = true,
                Time = DateTime.UtcNow
            };
        }

        public static Message Message()
        {
            return new Message
            {
                Body = "Hello World",
                Client = Client(),
                Group = Group(),
                Id = 1
            };
        }

        public static IList<Message> GroupHistory()
        {
            return new List<Message>
            {
                Message(),
                Message(),
                Message()
            };
        }
    }
}
