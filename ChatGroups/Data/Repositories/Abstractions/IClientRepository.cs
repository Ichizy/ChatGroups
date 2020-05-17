using ChatGroups.Data.Models;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public interface IClientRepository
    {
        Task Add(Client client);
    }
}
