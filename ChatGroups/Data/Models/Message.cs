using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatGroups.Data.Models
{
    public class Message
    {
        [Key]
        public long Id { get; set; }

        public string Body { get; set; }

        public DateTime Time { get; set; }

        [ForeignKey("FK_Messages_Clients")]
        public long ClientId { get; set; }

        // public string PublicId { get; set; }
    }
}