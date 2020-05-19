using ChatGroups.Data.Models;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public interface IGroupRepository
    {
        Task Create(Group group, Client creatorClient);

        Task Delete(Group group);

        Task<Group> Get(string publicId);
    }
}
