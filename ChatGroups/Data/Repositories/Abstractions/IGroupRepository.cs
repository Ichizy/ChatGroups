using ChatGroups.Data.Models;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public interface IGroupRepository
    {
        Task Add(Group group);

        Task Delete(Group group);
    }
}
