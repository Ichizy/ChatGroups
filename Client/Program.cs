using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
               .WithUrl("http://chatgroups.azurewebsites.net/chat")
                //.WithUrl("http://localhost:55933/chat")
                .Build();

            //TODO: who sent this message?
            connection.On("Receive", (string message) =>
            {
                Console.WriteLine($"Message received: {message}");
            });
            connection.On("Notify", (string message) =>
            {
                Console.WriteLine(message);
            });
            connection.StartAsync().Wait();

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                var text = Console.ReadLine();
                connection.InvokeAsync("Send", text).Wait();
                Console.Read();
            }

            connection.StopAsync().Wait();
        }
    }
}
