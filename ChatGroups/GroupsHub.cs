using ChatGroups.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.HubProcessors
{
    public class GroupsHub : Hub
    {
        private static IList<Group> chatGroups = new List<Group>();

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }

        [HubMethodName("ListGroups")]
        public async Task List()
        {
            await Clients.Caller.SendAsync("Receive", JsonConvert.SerializeObject(chatGroups));
        }

        [HubMethodName("CreateGroup")]
        public async Task Create(string groupName)
        {
            if (chatGroups.FirstOrDefault(x => x.Name == groupName) != null)
            {
                await Clients.Caller.SendAsync("Receive", $"Group {groupName} can't be created since it already exists.");
                return;
            }

            chatGroups.Add(new Group
            {
                Name = groupName,
                CurrentClientsAmount = 1,
                ClientsConnected = new List<string>
                {
                    Context.ConnectionId
                }
            });

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("Receive", $"Group {groupName} successfully created.");
        }

        [HubMethodName("JoinGroup")]
        public async Task Join(string groupName)
        {
            var existingGroup = chatGroups.FirstOrDefault(x => x.Name == groupName);
            if (existingGroup == null)
            {
                await Clients.Caller.SendAsync("Receive", $"ERROR: Group {groupName} doesn't exist. Please ensure you've entered the correct name.");
                return;
            }

            if (existingGroup.CurrentClientsAmount == existingGroup.MaximumClientsAmount)
            {
                await Clients.Caller.SendAsync("Receive", $"Sorry, you can't join {groupName} since there're already a limit of {existingGroup.MaximumClientsAmount} clients reached.");
                return;
            }

            if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) != null)
            {
                await Clients.Caller.SendAsync("Receive", $"You're already a member of {groupName} group.");
                return;
            }

            //TODO: update default collection
            existingGroup.ClientsConnected.Add(Context.ConnectionId);
            await Clients.Caller.SendAsync("Receive", $"You've successfully joined {groupName} group. Now you may start chating.");
        }

        [HubMethodName("LeaveGroup")]
        public void Leave()
        {

        }

        //public override async Task OnConnectedAsync()
        //{
        //    //TODO: connect to a specific group here
        //    //TODO: upload here previous chat history;
        //    var context = Context.GetHttpContext();
        //    await Clients.All.SendAsync("Notify", $"{context.Connection.RemoteIpAddress} entered the room.");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var context = Context.GetHttpContext();
        //    await Clients.All.SendAsync("Notify", $"{context.Connection.RemoteIpAddress} left the room.");
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}
