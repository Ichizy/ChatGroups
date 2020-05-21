using ChatGroups.Data.Models;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public interface IGroupRepository
    {
        Task Create(Group group, Client creatorClient);

        Task AddClientToGroup(string publicId, Client client);

        Task Delete(string publicId);

        Task<Group> Get(string publicId);

        Task LeaveGroup(string clientConnectionId, string publicId);
    }
}
