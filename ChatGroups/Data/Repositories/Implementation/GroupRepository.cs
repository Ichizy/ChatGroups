using ChatGroups.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly StorageContext _storage;

        public GroupRepository(StorageContext storage)
        {
            //TBD: I would prefer to add null-verification in each ctor.
            _storage = storage;
        }

        public async Task Create(Group group, Client creator)
        {
            await _storage.Groups.AddAsync(group);

            var clientGroup = new ClientGroup
            {
                Client = creator,
                Group = group
            };
            await _storage.ClientGroups.AddAsync(clientGroup);
            await _storage.SaveChangesAsync();
        }

        public async Task AddClientToGroup(string publicId, Client client)
        {
            var group = _storage.Groups.First(x => x.PublicId == publicId);

            var clientGroup = new ClientGroup
            {
                Client = client,
                Group = group
            };
            await _storage.ClientGroups.AddAsync(clientGroup);
            await _storage.SaveChangesAsync();
        }

        public async Task<Group> Get(string publicId)
        {
            return await _storage.Groups.FirstAsync(x => x.PublicId == publicId);
        }

        public async Task Delete(string publicId)
        {
            var group = await _storage.Groups.FirstAsync(x => x.PublicId == publicId);
            _storage.Groups.Remove(group);
            await _storage.SaveChangesAsync();
        }

        public async Task LeaveGroup(string clientConnectionId, string publicId)
        {
            var clientGroups = await _storage.ClientGroups.Where(x => x.Group.PublicId == publicId).ToListAsync();
            var clientGroup = clientGroups.First(x => x.Client.ConnectionId == clientConnectionId);

            clientGroups.Remove(clientGroup);
            _storage.ClientGroups.Remove(clientGroup);

            if (clientGroups.Count == 0)
            {
                //If no one left in group now - remove the group.
                var group = await _storage.Groups.FirstAsync(x => x.PublicId == publicId);
                _storage.Groups.Remove(group);
            }
            await _storage.SaveChangesAsync();
        }
    }
}
