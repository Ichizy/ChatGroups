using ChatGroups.Models;
using ChatGroups.Resources;
using ChatGroups.Services;
using ChatGroupsContracts;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.HubProcessors
{
    
    public class GroupsHub : Hub
    {
        public GroupsHub(GroupsOperationsProcessor processor)
        {
            processor.Test();
        }

        private static readonly IList<GroupDto> chatGroups = new List<GroupDto>();

        //TODO: retrieve from DI
        private readonly AppConfiguration _appConfiguration = new AppConfiguration();
        private const string receiveMethodName = "Receive";

        /// <summary>
        /// Sends a message to all members of a group.
        /// </summary>
        [HubMethodName("SendToGroup")]
        public async Task Send(string groupName, string body)
        {
            var message = CreateClientMessage(body);
            await Clients.Group(groupName).SendAsync(receiveMethodName, message);
        }

        /// <summary>
        /// Retrieves collection of all groups.
        /// </summary>
        [HubMethodName("ListGroups")]
        public async Task List()
        {
            var body = JsonConvert.SerializeObject(chatGroups);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(body));
        }

        /// <summary>
        /// Creates a new group with the name specified. Creator becomes first member by default.
        /// </summary>
        [HubMethodName(GroupMethodNames.CreateGroup)]
        public async Task Create(string groupName)
        {
            if (chatGroups.FirstOrDefault(x => x.Name == groupName) != null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"Group {groupName} can't be created since it already exists."));
                return;
            }

            chatGroups.Add(new GroupDto
            {
                Name = groupName,
                CurrentClientsAmount = 1,
                MaximumClientsAmount = _appConfiguration.MaximumGroupSize,
                ClientsConnected = new List<string>
                {
                    Context.ConnectionId
                }
            });

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"Group {groupName} successfully created."));
        }

        /// <summary>
        /// Allows a client to join an existing group.
        /// </summary>
        [HubMethodName("JoinGroup")]
        public async Task Join(string groupName)
        {
            var existingGroup = chatGroups.FirstOrDefault(x => x.Name == groupName);
            if (existingGroup == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(ErrorMessages.GroupDoesNotExist(groupName)));
                return;
            }

            if (existingGroup.CurrentClientsAmount == existingGroup.MaximumClientsAmount)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"Sorry, you can't join {groupName} since there're already a limit of {existingGroup.MaximumClientsAmount} clients reached."));
                return;
            }

            if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) != null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"You're already a member of {groupName} group."));
                return;
            }

            //TODO: update default collection
            existingGroup.ClientsConnected.Add(Context.ConnectionId);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(InformationMessages.SuccessfullyJoinedGroup(groupName)));
            //TODO: upload here previous chat history;

            var context = Context.GetHttpContext();
            var exclutionList = new List<string> { Context.ConnectionId };
            await Clients.GroupExcept(groupName, exclutionList).SendAsync(receiveMethodName, CreateSystemMessage($"{context.Connection.RemoteIpAddress} has joined group."));
        }

        /// <summary>
        /// Allows a client to leave a group.
        /// </summary>
        [HubMethodName("LeaveGroup")]
        public async Task Leave(string groupName)
        {
            var existingGroup = chatGroups.FirstOrDefault(x => x.Name == groupName);
            if (existingGroup == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"ERROR: Group {groupName} doesn't exist. Please ensure you've entered the correct name."));
                return;
            }

            if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"ERROR: You're not a member of {groupName} group."));
                return;
            }

            existingGroup.ClientsConnected.Remove(Context.ConnectionId);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"You've successfully left {groupName} group."));

            var context = Context.GetHttpContext();
            await Clients.Group(groupName).SendAsync(receiveMethodName, CreateSystemMessage($"{context.Connection.RemoteIpAddress} has left the group."));
        }

        private Message CreateClientMessage(string body)
        {
            var context = Context.GetHttpContext();
            return new Message
            {
                Sender = context.Connection.RemoteIpAddress.ToString(),
                Body = body,
                Time = DateTime.UtcNow
            };
        }

        private Message CreateSystemMessage(string body)
        {
            var context = Context.GetHttpContext();
            return new Message
            {
                Sender = "SYSTEM",
                Body = body,
                Time = DateTime.UtcNow
            };
        }

        //public override async Task OnConnectedAsync()
        //{
        //    //TODO: connect to a specific group here
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
