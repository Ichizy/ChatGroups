using System;
using System.Collections.Generic;
using ChatGroupsContracts;
using Client.Models;
using EasyConsole;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static HubConnection connection;
        private static string userNickName;
        private static GroupsProcessor _processor;

        public static void Main(string[] args)
        {
            ConfigureHub();

            userNickName = Input.ReadString("Welcome! Let's imitate authentication process ;) Please enter your nickname:");
            Output.WriteLine($"Hey {userNickName}, welcome to our messenger!");
            connection.StartAsync().Wait(); //TODO: this should be extracted later
            connection.InvokeAsync("Connect", userNickName).Wait();

            _processor = new GroupsProcessor(ref connection);

            var groupsMenu = new Menu()
                .Add("Create new group", () => _processor.CreateGroup().Wait())
                .Add("Join existing group", () => _processor.JoinGroup().Wait())
                .Add("Retrieve existing groups", () => connection.InvokeAsync("ListGroups").Wait())
                .Add("Chatting in a group", () => SendMessageToGroup())
                .Add("Exit", () => Environment.Exit(0));
            groupsMenu.Display();


            var selection = Console.ReadKey();
            while (selection.Key != ConsoleKey.Escape)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.M)
                {
                    groupsMenu.Display();
                }
                else
                {
                    SendMessageToGroup();
                }
            }
            connection.StopAsync().Wait();
        }

        private static void SendMessageToGroup()
        {
            string input = Input.ReadString($"{userNickName}:");
            var groupMessage = new GroupMessage
            {
                Body = input,
                GroupId = _processor._userGroups[0].GroupId,
                //TODO: select proper group here
                GroupName = _processor._userGroups[0].GroupName,
                //TODO: select proper group here
                SenderName = userNickName,
                Time = DateTime.UtcNow
            };
            connection.InvokeAsync("SendToGroup", groupMessage).Wait();
        }

        private static void ConfigureHub()
        {
            connection = new HubConnectionBuilder()
                   //.WithUrl("http://chatgroups.azurewebsites.net/chat")
                   .WithUrl("http://localhost:55933/chat")
                   .Build();

            connection.On("Receive", (Message message) =>
            {
                Output.WriteLine(ConsoleColor.Yellow, message.ToString());
            });
            connection.On(GroupMethodNames.OnGroupJoined, (GroupMessage groupMessage) =>
            {
                var userGroup = new UserGroup
                {
                    GroupId = groupMessage.GroupId,
                    GroupName = groupMessage.GroupName
                };
                _processor.OnGroupJoined(userGroup).Wait();
            });
        }
    }
}
