using ChatGroups.DTOs;
using ChatGroups.Models;
using ChatGroups.Resources;
using ChatGroups.Services;
using ChatGroups.Util;
using ChatGroupsContracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.HubProcessors
{
    /// <summary>
    /// Communnication hub, entry point for processing requests from clients. 
    /// </summary>
    public class ChatHub : Hub, IServerHubContract
    {
        private readonly IProcessor _processor;
        private readonly AppConfiguration _appConfig;

        private static IList<GroupDto> chatGroups = new List<GroupDto>();
        private const string receiveMethodName = MessageMethodNames.ReceiveGroupMessage;

        public ChatHub(IProcessor groupsProcessor, IOptions<AppConfiguration> options)
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
                Time = message.Time
            };
            await _processor.OnMessageSent(msgDto);
            await Clients.Group(message.GroupName).SendAsync(receiveMethodName, message);
        }

        /// <summary>
        /// Retrieves collection of all groups.
        /// </summary>
        [HubMethodName(GroupMethodNames.ListGroups)]
        public async Task List()
        {
            var body = JsonConvert.SerializeObject(chatGroups);
            await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(body));
        }

        /// <summary>
        /// Creates a new group with the name specified. Creator becomes first member by default.
        /// </summary>
        [HubMethodName(GroupMethodNames.CreateGroup)]
        public async Task Create(string groupName)
        {
            if (chatGroups.FirstOrDefault(x => x.Name == groupName) != null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(ErrorMessages.GroupAlreadyExists(groupName)));
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
            var message = MessageConstructor.SystemMessage(InformationMessages.GroupSuccessfullyCreated(groupName));
            var groupMessage = new GroupMessage
            {
                GroupName = groupName,
                GroupId = groupDto.PublicId,
                Body = message.Body,
                SenderName = message.SenderName,
                Time = message.Time
            };

            await Clients.Caller.SendAsync(receiveMethodName, message);
            await Clients.Caller.SendAsync(GroupMethodNames.OnGroupJoined, groupMessage);
        }

        /// <summary>
        /// Allows a client to join an existing group.
        /// </summary>
        [HubMethodName(GroupMethodNames.JoinGroup)]
        public async Task Join(string groupId)
        {
            var existingGroup = chatGroups.FirstOrDefault(x => x.PublicId == groupId);
            if (existingGroup == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(ErrorMessages.GroupDoesNotExist(groupId)));
                return;
            }

            var groupName = existingGroup.Name;
            if (existingGroup.CurrentClientsAmount == existingGroup.MaximumClientsAmount)
            {
                await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(InformationMessages.ClientsInGroupLimitReached(groupName, _appConfig.MaximumGroupSize)));
                return;
            }

            if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) != null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(ErrorMessages.AlreadyAGroupMember(groupName)));
                return;
            }
            var groupHistoryToClientDto = await _processor.OnGroupJoin(existingGroup.PublicId, Context.ConnectionId);

            var groupMessage = new GroupMessage
            {
                GroupName = groupName,
                GroupId = existingGroup.PublicId
            };
            existingGroup.ClientsConnected.Add(Context.ConnectionId);
            var exclutionList = new List<string> { Context.ConnectionId };

            Task.WaitAll(
            Groups.AddToGroupAsync(Context.ConnectionId, groupName),
            Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(InformationMessages.SuccessfullyJoinedGroup(groupName))),
            Clients.Caller.SendAsync(GroupMethodNames.OnGroupJoined, groupMessage),
            Clients.Caller.SendAsync(GroupMethodNames.ReceiveGroupHistory, MessageConstructor.GroupMessageHistory(groupHistoryToClientDto)),
            Clients.GroupExcept(groupName, exclutionList)
                .SendAsync(receiveMethodName, MessageConstructor.SystemMessage(InformationMessages.ClientHasJoinedGroup(groupHistoryToClientDto.Client.PublicName))));
        }

        /// <summary>
        /// Allows a client to leave a group.
        /// </summary>
        [HubMethodName(GroupMethodNames.LeaveGroup)]
        public async Task Leave(string groupId)
        {
            var existingGroup = chatGroups.FirstOrDefault(x => x.PublicId == groupId);
            if (existingGroup == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(ErrorMessages.GroupDoesNotExist(groupId)));
                return;
            }

            if (existingGroup.ClientsConnected.FirstOrDefault(x => x == Context.ConnectionId) == null)
            {
                await Clients.Caller.SendAsync(receiveMethodName, MessageConstructor.SystemMessage(ErrorMessages.NotAGroupMember(existingGroup.Name)));
                return;
            }

            existingGroup.ClientsConnected.Remove(Context.ConnectionId);
            if (existingGroup.ClientsConnected.Count == 0)
                chatGroups.Remove(existingGroup);

            var clientName = await _processor.OnGroupLeave(groupId, Context.ConnectionId);

            Task.WaitAll(
            Groups.RemoveFromGroupAsync(Context.ConnectionId, existingGroup.Name),
            Clients.Caller
                .SendAsync(receiveMethodName, MessageConstructor.SystemMessage(InformationMessages.SuccessfullyLeftGroup(existingGroup.Name))),
            Clients.Group(existingGroup.Name)
                .SendAsync(receiveMethodName, MessageConstructor.SystemMessage(InformationMessages.ClientHasLeftGroup(clientName))));
        }

        /// <summary>
        /// This method is an imitation of sign up process. Currently new client is authomatically added once he's connected. In real project, proper auth module should be called separately.
        /// </summary>
        [HubMethodName(ClientMethodNames.SignUp)]
        public async Task SignUp(string clientNickname)
        {
            var clientDto = new ClientDto
            {
                ConnectionId = Context.ConnectionId,
                Nickname = clientNickname
            };
            await _processor.OnSignUp(clientDto);
        }
    }
}
