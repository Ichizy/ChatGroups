using ChatGroups.DTOs;
using ChatGroups.Models;
using ChatGroups.Resources;
using ChatGroups.Services;
using ChatGroupsContracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
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
        private readonly AppConfiguration _appConfig;

        private static IList<GroupDto> chatGroups = new List<GroupDto>();
        private const string receiveMethodName = MessageMethodNames.Receive;

        public GroupsHub(IGroupsProcessor groupsProcessor, IOptions<AppConfiguration> options)
        {
            _processor = groupsProcessor;
            _appConfig = options.Value;
        }

        /// <summary>
        /// Sends a message to all members of a group.
        /// </summary>
        [HubMethodName(GroupMethodNames.SendToGroup)]
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
        [HubMethodName(GroupMethodNames.ListGroups)]
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
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(ErrorMessages.GroupAlreadyExists(groupName)));
                return;
            }
            var groupDto = new GroupDto
            {
                Name = groupName,
                CurrentClientsAmount = 1,
                MaximumClientsAmount = _appConfig.MaximumGroupSize,
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
        [HubMethodName(GroupMethodNames.JoinGroup)]
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
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(InformationMessages.ClientsInGroupLimitReached(groupName, _appConfig.MaximumGroupSize)));
                return;
            }

            //TODO: return, commented for testing now.
            //if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) != null)
            //{
            //    await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage($"You're already a member of {groupName} group."));
            //    return;
            //}
            var groupHistoryToClientDto = await _processor.OnGroupJoin(existingGroup.PublicId, Context.ConnectionId);

            //TODO: update default collection
            existingGroup.ClientsConnected.Add(Context.ConnectionId);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(InformationMessages.SuccessfullyJoinedGroup(groupName)));
            //TODO: return history here;

            var context = Context.GetHttpContext();
            var exclutionList = new List<string> { Context.ConnectionId };
            await Clients.GroupExcept(groupName, exclutionList).SendAsync(receiveMethodName, CreateSystemMessage($"{groupHistoryToClientDto.Client.PublicName} has joined group."));
        }

        /// <summary>
        /// Allows a client to leave a group.
        /// </summary>
        [HubMethodName(GroupMethodNames.LeaveGroup)]
        public async Task Leave(string groupName)
        {
            var existingGroup = chatGroups.FirstOrDefault(x => x.Name == groupName);
            if (existingGroup == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(ErrorMessages.GroupDoesNotExist(groupName)));
                return;
            }

            if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(ErrorMessages.NotAGroupMember(groupName)));
                return;
            }

            existingGroup.ClientsConnected.Remove(Context.ConnectionId);
            await Clients.Caller.SendAsync(receiveMethodName, CreateSystemMessage(InformationMessages.SuccessfullyLeftGroup(groupName)));

            //TODO: remove in DB as well
            if (existingGroup.ClientsConnected.Count == 0)
            {
                //TODO: remove the group here
            }

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
