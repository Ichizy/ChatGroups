using ChatGroups.DTOs;
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
        private readonly IGroupsProcessor _processor;
        private static IList<GroupDto> chatGroups = new List<GroupDto>();

        //TODO: retrieve from DI
        private readonly AppConfiguration _appConfiguration = new AppConfiguration();
        private const string receiveMethodName = "Receive";

        public GroupsHub(IGroupsProcessor groupsProcessor)
        {
            _processor = groupsProcessor;
        }

        /// <summary>
        /// Sends a message to all members of a group.
        /// </summary>
        [HubMethodName("SendToGroup")]
        public async Task SendToGroup(GroupMessage message)
        {
            var msgDto = new MessageDto
            {
                Body = message.Body,
                GroupId = message.GroupId,
                SenderConnectionId = Context.ConnectionId,
                SentToGroup = true,
                Time = DateTime.UtcNow //TODO: ensure time
            };
            await _processor.OnMessageSent(msgDto);

            var broadcastMessage = CreateClientMessage(message.Body, message.SenderName);
            await Clients.Group(message.GroupName).SendAsync(receiveMethodName, broadcastMessage);
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
            var groupDto = new GroupDto
            {
                Name = groupName,
                CurrentClientsAmount = 1,
                MaximumClientsAmount = _appConfiguration.MaximumGroupSize,
                CreatorConnectionId = Context.ConnectionId,
                ClientsConnected = new List<string>
                {
                    Context.ConnectionId
                }
            };
            var publicId = await _processor.OnGroupCreation(groupDto);
            groupDto.PublicId = publicId;
            chatGroups.Add(groupDto);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var message = CreateSystemMessage($"Group {groupName} successfully created.");
            var groupMessage = new GroupMessage
            {
                GroupName = groupName,
                GroupId = groupDto.PublicId
                //TODO: construct from basic message?
            };

            await Clients.Caller.SendAsync(receiveMethodName, message);
            await Clients.Caller.SendAsync(GroupMethodNames.OnGroupJoined, groupMessage);
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

            //TODO: return, commented for testing now.
            //if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) != null)
            //{
            //    await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"You're already a member of {groupName} group."));
            //    return;
            //}
            var messageHistory = await _processor.OnGroupJoin(existingGroup.PublicId, Context.ConnectionId);

            //TODO: update default collection
            existingGroup.ClientsConnected.Add(Context.ConnectionId);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(InformationMessages.SuccessfullyJoinedGroup(groupName)));
            //TODO: return history here;

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

        //TODO: consider moving this to OnConnected
        [HubMethodName("Connect")]
        public async Task Connect(string clientNickname)
        {
            var clientDto = new ClientDto
            {
                ConnectionId = Context.ConnectionId,
                nickname = clientNickname
            };
            await _processor.OnClientRegistered(clientDto);
        }

        private Message CreateClientMessage(string body, string senderName)
        {
            var context = Context.GetHttpContext();
            return new Message
            {
                SenderName = senderName,
                Body = body,
                Time = DateTime.UtcNow
            };
        }

        private Message CreateSystemMessage(string body)
        {
            var context = Context.GetHttpContext();
            return new Message
            {
                SenderName = "SYSTEM",
                Body = body,
                Time = DateTime.UtcNow
            };
        }
    }
}
