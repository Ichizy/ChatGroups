using ChatGroups.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Data.Repositories
{
    public interface IMessageRepository
    {
        Task Add(Message message);
    }
}
