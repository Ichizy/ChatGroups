using ChatGroups.Data.Models;
using ChatGroups.Data.Repositories;
using ChatGroups.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    public class GroupsOperationsProcessor
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IClientRepository _clientRepo;
        private readonly ILogger _logger;

        public GroupsOperationsProcessor(ILogger logger, IGroupRepository groupRepo, IClientRepository clientRepo)
        {
            _groupRepo = groupRepo;
            _clientRepo = clientRepo;
            _logger = logger;
        }

        public void Test()
        {
            _logger.LogInformation("TEST SABINA");
        }

        public async Task<string> OnGroupCreation(GroupDto groupDto)
        {
            try
            {
                var client = await _clientRepo.Get(groupDto.CreatorConnectionId);

                var group = new Group(groupDto.Name);
                await _groupRepo.Create(group, client);
                //create group, add client to that, return group public id?
                return group.PublicId;
            }
            catch (Exception ex)
            {
                //TODO: process properly
                return null;
            }
        }

        public void OnGroupJoin()
        {

            //TODO: return messages history here
            //TODO: add person to group in DB
        }
    }
}
