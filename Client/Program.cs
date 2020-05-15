using System;
using ChatGroupsContracts;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static HubConnection connection;

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to chat!");
            //TODO: show main menu here

            connection = new HubConnectionBuilder()
                //.WithUrl("http://chatgroups.azurewebsites.net/chat")
                .WithUrl("http://localhost:55933/chat")
                .Build();

            ConfigureHub();
            connection.StartAsync().Wait();
            var processor = new GroupsProcessor(ref connection);

            var selection = Console.ReadKey();
            while (selection.Key != ConsoleKey.Escape)
            {
                if (selection.Key == ConsoleKey.D1)
                {
                    Console.WriteLine("Please create a group or enter existing one:\n1.Create new group.\n2.Join existing group.\n3.Retrieve existing groups.");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D1:
                            {
                                processor.CreateGroup().Wait();
                                break;
                            }
                        case ConsoleKey.D2:
                            {
                                processor.JoinGroup().Wait();
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
                else
                {
                    var text = Console.ReadLine();
                    //TODO: pass groupName here
                    connection.InvokeAsync("SendToGroup", "sabina", selection.KeyChar + text).Wait();
                }
                selection = Console.ReadKey();
            }

            connection.StopAsync().Wait();
        }

        private static void ConfigureHub()
        {
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
