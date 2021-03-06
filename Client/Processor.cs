﻿using ChatGroupsContracts;
using Client.Models;
using EasyConsole;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    public class Processor
    {
        private readonly HubConnection _connection;
        public List<UserGroup> UserGroups;
        public UserGroup CurrentGroup;

        public Processor(ref HubConnection connection)
        {
            _connection = connection;
            UserGroups = new List<UserGroup>();
        }

        public async Task CreateGroup()
        {
            Console.WriteLine("Enter group name:");
            var groupName = Console.ReadLine();
            await _connection.InvokeAsync(GroupMethodNames.CreateGroup, groupName);
        }

        public void ChangeCurrentGroup()
        {
            Console.WriteLine("Select a group from list of your groups:");
            for (int i = 0; i < UserGroups.Count; i++)
            {
                Console.WriteLine($"{i}. {UserGroups[i].GroupName}");
                //TBD: Since it's console application for testing purposes, let's leave only name here. For real usage there could be message history or identifier, depends on business requirements.
            }
            int input = Input.ReadInt();
            if (input < 0 || input > UserGroups.Count)
            {
                Output.WriteLine(ConsoleColor.Red, "Invalid group number provided.");
                return;
            }
            CurrentGroup = UserGroups[input];
            Output.WriteLine(ConsoleColor.Green, $"You're now chatting in {UserGroups[input].GroupName} group.");
        }

        public async Task JoinGroup()
        {
            Console.WriteLine("Enter group ID:");
            var groupId = Console.ReadLine();
            await _connection.InvokeAsync(GroupMethodNames.JoinGroup, groupId);
        }

        public async Task LeaveGroup()
        {
            Console.WriteLine("Select a group to leave from list of your groups:");
            for (int i = 0; i < UserGroups.Count; i++)
            {
                Console.WriteLine($"{i}. {UserGroups[i].GroupName}");
                //TBD: Since it's console application for testing purposes, let's leave only name here. For real usage there could be message history or identifier, depends on business requirements.
            }
            int input = Input.ReadInt();
            if (input < 0 || input > UserGroups.Count)
            {
                Output.WriteLine(ConsoleColor.Red, "Invalid group number provided.");
                return;
            }
            var groupId = UserGroups[input].GroupId;
            UserGroups.Remove(UserGroups[input]);
            if (CurrentGroup.GroupId == groupId)
            {
                CurrentGroup = null;
                Output.WriteLine(ConsoleColor.Red, "You've left your current group, please select a new one in main menu (press M), so you could chat in another group.");
            }
            await _connection.InvokeAsync(GroupMethodNames.LeaveGroup, groupId);
        }

        public void OnGroupJoined(UserGroup group)
        {
            UserGroups.Add(group);
            CurrentGroup = group;
            Output.WriteLine(ConsoleColor.Green, $"You're now chatting in {group.GroupName} group.");
        }
    }
}
