using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Data.Models
{
    public class ClientGroup
    {
        [Key]
        public long Id { get; set; }

        public Client Client { get; set; }

        public Group Group { get; set; }
    }
}
