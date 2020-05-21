using ChatGroups.DTOs;
using ChatGroups.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    public interface IProcessor
    {
        Task<string> OnGroupCreation(GroupDto groupDto);

        Task OnClientRegistered(ClientDto clientDto);

        /// <summary>
        /// Triggered by Send Message operation, responsible for processing all side-related operations (managing storage for example).
        /// </summary>
        Task OnMessageSent(MessageDto msgDto);

        Task<GroupMessagesToClientDto> OnGroupJoin(string groupId, string clientConnectionId);
    }
}
