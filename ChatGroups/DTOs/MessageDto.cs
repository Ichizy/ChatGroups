using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.DTOs
{
    public class MessageDto
    {
        public string Body { get; set; }

        public string SenderConnectionId { get; set; }

        public string GroupId { get; set; }

        public bool SentToGroup { get; set; }
    }
}
