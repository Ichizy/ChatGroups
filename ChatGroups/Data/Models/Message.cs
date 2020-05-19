using System;
using System.ComponentModel.DataAnnotations;

namespace ChatGroups.Data.Models
{
    public class Message
    {
        [Key]
        public long Id { get; set; }

        public string Body { get; set; }

        public DateTime Time { get; set; }

        public Client Client { get; set; }

        /// <summary>
        /// Message can belong to a group, however it might be also possible to send a message just to another user (so group would be empty).
        /// </summary>
        public Group? Group { get; set; }
    }
}