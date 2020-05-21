using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.DTOs;
using ChatGroups.Models;
using ChatGroups.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    public class Processor : IProcessor
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IMessageRepository _messageRepo;

        public Processor(IGroupRepository groupRepo, IClientRepository clientRepo, IMessageRepository messageRepo)
        {
            _groupRepo = groupRepo;
            _clientRepo = clientRepo;
            _messageRepo = messageRepo;
        }

        public async Task OnSignUp(ClientDto clientDto)
        {
            Ensure.NotNull(nameof(clientDto), clientDto);

            try
            {
                Log.Information("Starting SignUp: {@clientDto}", clientDto);
                var client = new Client
                {
                    ConnectionId = clientDto.ConnectionId,
                    PublicName = clientDto.Nickname
                };
                await _clientRepo.Add(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OnSignUp failed for {@clientDto}", clientDto);
                throw ex;
            }
        }

        public async Task<string> OnGroupCreation(GroupDto groupDto)
        {
            Ensure.NotNull(nameof(groupDto), groupDto);

            try
            {
                var client = await _clientRepo.Get(groupDto.CreatorConnectionId);
                var group = new Group(groupDto.Name);
                await _groupRepo.Create(group, client);
                return group.PublicId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OnGroupCreation failed for {@groupDto}", groupDto);
                throw ex;
            }
        }

        public async Task OnMessageSent(MessageDto msgDto)
        {
            Ensure.NotNull(nameof(msgDto), msgDto);

            try
            {
                var client = await _clientRepo.Get(msgDto.SenderConnectionId);
                var msg = new Message
                {
                    Body = msgDto.Body,
                    Client = client,
                    Time = msgDto.Time
                };

                if (msgDto.SentToGroup)
                    msg.Group = await _groupRepo.Get(msgDto.GroupId);

                await _messageRepo.Add(msg);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OnMessageSent failed for {@msgDto}", msgDto);
                throw ex;
            }
        }

        public async Task<GroupMessagesToClientDto> OnGroupJoin(string groupId, string clientConnectionId)
        {
            Ensure.NotNull(nameof(groupId), groupId);
            Ensure.NotNull(nameof(clientConnectionId), clientConnectionId);

            try
            {
                var client = await _clientRepo.Get(clientConnectionId);
                await _groupRepo.AddClientToGroup(groupId, client);

                var historyModel = await _messageRepo.GetGroupHistory(groupId);
                var history = new List<MessageDto>();

                foreach (var item in historyModel)
                {
                    var historyItem = new MessageDto
                    {
                        GroupId = item.Group.PublicId,
                        Body = item.Body,
                        SenderName = item.Client.PublicName,
                        SenderConnectionId = item.Client.ConnectionId,
                        SentToGroup = true,
                        Time = item.Time
                    };
                    history.Add(historyItem);
                }
                return new GroupMessagesToClientDto
                {
                    Messages = history,
                    Client = client
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"OnGroupJoin failed for client: {clientConnectionId}, group: {groupId}");
                throw ex;
            }
        }

        /// <summary>
        /// Cleans up client-group relations, drops the group if no one left in there.
        /// </summary>
        /// <returns>Client public name.</returns>
        public async Task<string> OnGroupLeave(string groupId, string clientConnectionId)
        {
            Ensure.NotNull(nameof(groupId), groupId);
            Ensure.NotNull(nameof(clientConnectionId), clientConnectionId);

            try
            {
                await _groupRepo.LeaveGroup(clientConnectionId, groupId);
                var client = await _clientRepo.Get(clientConnectionId);
                return client.PublicName;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"OnGroupLeave failed for client: {clientConnectionId}, group: {groupId}");
                throw ex;
            }
        }
    }
}
