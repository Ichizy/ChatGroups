using ChatGroupsContracts;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    public class GroupsProcessor
    {
        private readonly HubConnection _connection;
        private List<UserGroup> _userGroups;

        public GroupsProcessor(ref HubConnection connection)
        {
            _connection = connection;
            _userGroups = new List<UserGroup>();
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
            await _connection.InvokeAsync(GroupMethodNames.JoinGroup, groupName);
        }

        public async Task OnGroupJoined(UserGroup group)
        {
            //TODO: add proper group here
            _userGroups.Add(group);
        }
    }
}
