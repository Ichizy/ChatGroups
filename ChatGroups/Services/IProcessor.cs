using ChatGroups.DTOs;
using ChatGroups.Models;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    public interface IProcessor
    {
        /// <summary>
        /// This method is an imitation of sign up process. Currently new client is authomatically added once he's connected. 
        /// </summary>
        Task OnSignUp(ClientDto clientDto);

        Task<string> OnGroupCreation(GroupDto groupDto);

        Task OnMessageSent(MessageDto msgDto);

        Task<GroupMessagesToClientDto> OnGroupJoin(string groupId, string clientConnectionId);

        /// <summary>
        /// Cleans up client-group relations, drops the group if no one left in there.
        /// </summary>
        /// <returns>Client public name.</returns>
        Task<string> OnGroupLeave(string groupId, string clientConnectionId)
    }
}
