using ChatGroups.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly StorageContext _storage;

        public MessageRepository(StorageContext storage)
        {
            //TBD: I would prefer to add null-verification in each ctor.
            _storage = storage;
        }

        public async Task Add(Message message)
        {
            await _storage.Messages.AddAsync(message);
            await _storage.SaveChangesAsync();
        }

        //TODO: Here should be functionality to modify, mark as deleted message, etc.
    }
}
