using ChatGroups.DTOs;
using ChatGroups.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    public interface IGroupsProcessor
    {
        Task<string> OnGroupCreation(GroupDto groupDto);

        Task OnClientRegistered(ClientDto clientDto);

        Task OnMessageSent(MessageDto msgDto);

        Task<IList<MessageDto>> OnGroupJoin(string groupId, string clientConnectionId);
    }
}
