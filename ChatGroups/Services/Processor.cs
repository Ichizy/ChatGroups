using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.DTOs;
using ChatGroups.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    //TODO: add logging support
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

        public async Task OnClientRegistered(ClientDto clientDto)
        {
            var client = new Client
            {
                ConnectionId = clientDto.ConnectionId,
                PublicName = clientDto.nickname
            };
            await _clientRepo.Add(client);
        }

        public async Task<string> OnGroupCreation(GroupDto groupDto)
        {
            try
            {
                var client = await _clientRepo.Get(groupDto.CreatorConnectionId);

                var group = new Group(groupDto.Name);
                await _groupRepo.Create(group, client);
                return group.PublicId;
            }
            catch (Exception ex)
            {
                //TODO: process properly
                throw ex;
            }
        }

        /// <summary>
        /// Triggered by Send Message operation, responsible for processing all side-related operations (managing storage for example).
        /// </summary>
        public async Task OnMessageSent(MessageDto msgDto)
        {
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
                //TODO: process properly
                throw ex;
            }
        }

        public async Task<GroupMessagesToClientDto> OnGroupJoin(string groupId, string clientConnectionId)
        {
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
                //TODO: process properly
                throw ex;
            }
        }

        public async Task OnGroupLeave(string clientConnectionId, string groupId)
        {
            try
            {
                await _groupRepo.LeaveGroup(clientConnectionId, groupId);
            }
            catch (Exception ex)
            {
                //TODO: process properly
                throw ex;
            }
        }
    }
}
