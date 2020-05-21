using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatGroups.Data.Models
{
    public class ClientGroup
    {
        [Key]
        public long Id { get; set; }

        public long ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public long GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }
    }
}
