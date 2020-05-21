using System;
using ChatGroupsContracts;
using ChatGroupsContracts.Models;
using Client.Models;
using EasyConsole;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static HubConnection connection;
        private static string userNickName;
        private static Processor _processor;
        // private const string connectionUrl = "http://chatgroups.azurewebsites.net/chat";
        private const string connectionUrl = "http://localhost:55933/chat";

        public static void Main(string[] args)
        {
            ConfigureHub();

            userNickName = Input.ReadString("Welcome! Let's imitate authentication process ;) Please enter your nickname:");
            Output.WriteLine($"Hey {userNickName}, welcome to our messenger!");

            connection.StartAsync().Wait();
            connection.InvokeAsync(ClientMethodNames.SignUp, userNickName).Wait();
            _processor = new Processor(ref connection);

            var groupsMenu = new Menu()
                .Add("Create new group", () => _processor.CreateGroup().Wait())
                .Add("Join existing group", () => _processor.JoinGroup().Wait())
                .Add("Retrieve existing groups", () => connection.InvokeAsync(GroupMethodNames.ListGroups).Wait())
                .Add("Change current group (send messages to another group from now)", () => _processor.ChangeCurrentGroup())
                .Add("Leave group", () => _processor.LeaveGroup().Wait())
                .Add("Exit", () => Environment.Exit(0));
            groupsMenu.Display();
            Output.WriteLine(ConsoleColor.Cyan, "You're now in chatting mode, after sending/receiving message, press Enter to proceed.\nIf you want to call main menu, please press M.");


            var selection = Console.ReadKey();
            while (selection.Key != ConsoleKey.Escape)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.M)
                    groupsMenu.Display();
                else
                {
                    SendMessageToGroup();
                }
            }
            connection.StopAsync().Wait();
        }

        private static void SendMessageToGroup()
        {
            if (_processor.CurrentGroup == null)
            {
                Output.WriteLine(ConsoleColor.Red, "You currently have no group selected. Please create, join or select a group where you would like to chat.\nPress M for entering main menu.");
                return;
            }
            string input = Input.ReadString($"{userNickName}:");
            var groupMessage = new GroupMessage
            {
                Body = input,
                GroupId = _processor.CurrentGroup.GroupId,
                GroupName = _processor.CurrentGroup.GroupName,
                SenderName = userNickName,
                Time = DateTime.UtcNow
            };
            connection.InvokeAsync(GroupMethodNames.SendToGroup, groupMessage).Wait();
        }

        private static void ConfigureHub()
        {
            connection = new HubConnectionBuilder()
                   .WithUrl(connectionUrl)
                   .Build();

            connection.On(MessageMethodNames.ReceiveGroupMessage, (GroupMessage message) =>
            {
                Output.WriteLine(ConsoleColor.Yellow, message.ToString());
            });
            connection.On(GroupMethodNames.ReceiveGroupHistory, (GroupMessageHistory history) =>
            {
                foreach (var item in history.Messages)
                {
                    Output.WriteLine(ConsoleColor.Yellow, item.ToString());
                }
            });
            connection.On(GroupMethodNames.OnGroupJoined, (GroupMessage groupMessage) =>
            {
                var userGroup = new UserGroup
                {
                    GroupId = groupMessage.GroupId,
                    GroupName = groupMessage.GroupName
                };
                _processor.OnGroupJoined(userGroup);
            });
        }
    }
}
