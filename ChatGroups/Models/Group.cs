using System.Collections.Generic;

namespace ChatGroups.Models
{
    public class Group
    {
        public string Name { get; set; }
        public uint CurrentClientsAmount { get; set; }
        public uint MaximumClientsAmount { get; set; }
        public IList<string> ClientsConnected { get; set; }

        public Group()
        {
            //TODO: extract this somewhere
            MaximumClientsAmount = 20;
            ClientsConnected = new List<string>();
        }
    }
}
