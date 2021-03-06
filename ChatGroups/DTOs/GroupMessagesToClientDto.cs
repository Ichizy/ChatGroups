﻿using ChatGroups.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.DTOs
{
    public class GroupMessagesToClientDto
    {
        public IList<MessageDto> Messages { get; set; }

        public Client Client { get; set; }

        public string GroupId { get; set; }

        public string GroupName { get; set; }

        public bool SentToGroup { get; set; }
    }
}
