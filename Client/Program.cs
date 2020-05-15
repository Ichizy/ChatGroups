using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static HubConnection connection;

        public static void Main(string[] args)
        {
            connection = new HubConnectionBuilder()
               //.WithUrl("http://chatgroups.azurewebsites.net/chat")
                .WithUrl("http://localhost:55933/chat")
                .Build();

            ConfigureHub();

            connection.StartAsync().Wait();
            StartMenu();

            var selection = Console.ReadKey();
            while (selection.Key != ConsoleKey.Escape)
            {
                if (selection.Key == ConsoleKey.D1)
                {
                    StartMenu();
                }
                else
                {
                    var text = Console.ReadLine();
                    connection.InvokeAsync("Send", selection.KeyChar + text).Wait();
                }
                selection = Console.ReadKey();
            }

            connection.StopAsync().Wait();
        }

        private static void StartMenu()
        {
            Console.WriteLine("Welcome to chat! Please create a group or enter existing one:\n1.Create new group.\n2.Enter existing group.\n3.Retrieve existing groups.");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    {
                        Console.WriteLine("Enter group name:");
                        var groupName = Console.ReadLine();
                        connection.InvokeAsync("CreateGroup", groupName).Wait();
                        break;
                    }
                case ConsoleKey.D2:
                    {
                        connection.StartAsync().Wait();
                        break;
                    }
                case ConsoleKey.D3:
                    {
                        connection.InvokeAsync("ListGroups").Wait();
                        break;
                    }
                default:
                    {
                        Console.WriteLine("No proper value chosen. Please enter your choice:");
                        break;
                    }
            }
        }

        private static void ConfigureHub()
        {
            //TODO: who sent this message?
            connection.On("Receive", (string message) =>
            {
                Console.WriteLine($"Message received: {message}");
            });
            connection.On("Notify", (string message) =>
            {
                Console.WriteLine(message);
            });
        }
    }
}
