using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ChatGroups
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }

        public override async Task OnConnectedAsync()
        {
            //TODO: upload here previous chat history;
            var context = Context.GetHttpContext();
            await Clients.All.SendAsync("Notify", $"{context.Connection.RemoteIpAddress} entered the room.");
            await base.OnConnectedAsync();

            //var context = this.Context.GetHttpContext();
            //if (context.Request.Cookies.ContainsKey("name"))
            //{
            //    string userName;
            //    if (context.Request.Cookies.TryGetValue("name", out userName))
            //    {
            //        Debug.WriteLine($"name = {userName}");
            //    }
            //}
            //Debug.WriteLine($"UserAgent = {context.Request.Headers["User-Agent"]}");
            //Debug.WriteLine($"RemoteIpAddress = {context.Connection.RemoteIpAddress.ToString()}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} left the room.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}