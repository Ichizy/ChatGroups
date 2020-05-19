using ChatGroups.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly StorageContext _storage;

        public ClientRepository(StorageContext storage)
        {
            //TBD: I would prefer to add null-verification in each ctor.
            _storage = storage;
        }

        public async Task Add(Client client)
        {
            await _storage.Clients.AddAsync(client);
            await _storage.SaveChangesAsync();
        }

        public Task<Client> Get(string connectionId)
        {
            return _storage.Clients.FirstAsync(x => x.ConnectionId == connectionId);
        }
    }
}