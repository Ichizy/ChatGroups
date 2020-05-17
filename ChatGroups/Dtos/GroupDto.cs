using System.Collections.Generic;

namespace ChatGroups.Models
{
    public class GroupDto
    {
        public string PublicId { get; set; }

        public string Name { get; set; }

        public uint CurrentClientsAmount { get; set; }

        public uint MaximumClientsAmount { get; set; }

        public string CreatorConnectionId { get; set; }

        public IList<string> ClientsConnected { get; set; }
    }
}
