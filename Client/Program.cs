using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:55933/chat")
                .Build();

            connection.On("Receive", (string message, string connectionId) =>
            {
                Console.WriteLine($"Message received from {connectionId}: {message}");
            });
            connection.On("Notify", (string message) =>
            {
                Console.WriteLine(message);
            });
            connection.StartAsync();
            connection.InvokeAsync("Send", "Test text");

            Console.Read();
            connection.StopAsync();
        }
    }
}
