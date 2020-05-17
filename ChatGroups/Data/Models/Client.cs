using System;
using System.ComponentModel.DataAnnotations;

namespace ChatGroups.Data.Models
{
    public class Client
    {
        [Key]
        public long Id { get; set; }

        public string PublicName { get; set; }

        public string ConnectionId { get; set; }
    }
}
