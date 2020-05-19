using ChatGroups.DTOs;
using ChatGroups.Models;
using System.Threading.Tasks;

namespace ChatGroups.Services
{
    public interface IGroupsProcessor
    {
        Task<string> OnGroupCreation(GroupDto groupDto);

        Task OnClientRegistered(ClientDto clientDto);

        Task OnMessageSent(MessageDto msgDto);
    }
}
