using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.DTOs;
using ChatGroups.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    //TODO: add logging support
    //TODO: split the responsibilities or rename service (depends on further extentions?)
    public class GroupsProcessor : IGroupsProcessor
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IMessageRepository _messageRepo;

        public GroupsProcessor(IGroupRepository groupRepo, IClientRepository clientRepo, IMessageRepository messageRepo)
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

        public async Task OnMessageSent(MessageDto msgDto)
        {
            try
            {
                var client = await _clientRepo.Get(msgDto.SenderConnectionId);
                var msg = new Message
                {
                    Body = msgDto.Body,
                    Client = client
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

        public async Task<IList<MessageDto>> OnGroupJoin(string groupId, string clientConnectionId)
        {
            try
            {
                var client = await _clientRepo.Get(clientConnectionId);
                await _groupRepo.AddClientToGroup(groupId, client);

                var historyModel = await _messageRepo.GetGroupHistory(groupId);
                var history = new List<MessageDto>();

                //TODO: automapper?
                foreach (var item in historyModel)
                {
                    var historyItem = new MessageDto
                    {
                        GroupId = item.Group.PublicId,
                        Body = item.Body,
                        SenderConnectionId = item.Client.ConnectionId,
                        SentToGroup = true,
                        Time = item.Time
                    };
                    history.Add(historyItem);
                }
                return history;
            }
            catch (Exception ex)
            {
                //TODO: process properly
                throw ex;
            }
        }
    }
}
