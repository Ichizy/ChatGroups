using ChatGroups.HubProcessors;
using ChatGroups.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //TODO: connect to a specific group here
            //TODO: upload here previous chat history;
            var context = Context.GetHttpContext();
            await Clients.All.SendAsync("Notify", $"{context.Connection.RemoteIpAddress} entered the room.");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            await Clients.All.SendAsync("Notify", $"{context.Connection.RemoteIpAddress} left the room.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}