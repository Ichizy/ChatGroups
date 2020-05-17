﻿using ChatGroups.Data.Models;
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

        public async Task Add(Group group)
        {
            await _storage.Groups.AddAsync(group);
            await _storage.SaveChangesAsync();
        }

        //TODO: delete by ID?
        public async Task Delete(Group group)
        {
            _storage.Groups.Remove(group);
            await _storage.SaveChangesAsync();
        }
    }
}