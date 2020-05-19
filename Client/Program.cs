using System;
using System.Collections.Generic;
using ChatGroupsContracts;
using EasyConsole;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static HubConnection connection;
        private static string userNickName;

        public static void Main(string[] args)
        {
            ConfigureHub();

            userNickName = Input.ReadString("Welcome! Let's imitate authentication process ;) Please enter your nickname:");
            Output.WriteLine($"Hey {userNickName}, welcome to our messenger!");
            connection.StartAsync().Wait(); //TODO: this should be extracted later
            connection.InvokeAsync("Connect", userNickName).Wait();

            var processor = new GroupsProcessor(ref connection);

            var groupsMenu = new Menu()
                .Add("Create new group", () => processor.CreateGroup().Wait())
                .Add("Join existing group", () => processor.JoinGroup().Wait())
                .Add("Retrieve existing groups", () => connection.InvokeAsync("ListGroups").Wait())
                .Add("Chatting in a group", () => SendMessageToGroup())
                .Add("Exit", () => Environment.Exit(0));
            groupsMenu.Display();


            var selection = Console.ReadKey();
            while (selection.Key != ConsoleKey.Escape)
            {
                groupsMenu.Display();
                Console.ReadKey();
            }
            connection.StopAsync().Wait();
        }

        private static void SendMessageToGroup()
        {
            string input = Input.ReadString($"{userNickName}:");
            var groupMessage = new GroupMessage
            {
                Body = input,
                GroupId = "",
                //TODO: pass groupId here
                GroupName = "",
                //TODO: pass groupId here
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
                Console.WriteLine(message);
            });
            connection.On("Notify", (string message) =>
            {
                Console.WriteLine(message);
            });
        }
    }
}
