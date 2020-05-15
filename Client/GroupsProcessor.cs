using ChatGroupsContracts;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Client
{
    public class GroupsProcessor
    {
        private readonly HubConnection _connection;

        public GroupsProcessor(ref HubConnection connection)
        {
            _connection = connection;
        }

        public async Task CreateGroup()
        {
            Console.WriteLine("Enter group name:");
            var groupName = Console.ReadLine();

            // await connection.StartAsync();
            await _connection.InvokeAsync(GroupMethodNames.CreateGroup, groupName);
        }

        public async Task JoinGroup()
        {
            Console.WriteLine("Enter group name:");
            var groupName = Console.ReadLine();

            // await connection.StartAsync();
            _connection.InvokeAsync(GroupMethodNames.JoinGroup, groupName).Wait();
        }
    }
}
