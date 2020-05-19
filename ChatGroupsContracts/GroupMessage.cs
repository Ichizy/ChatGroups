using System;
using System.Collections.Generic;
using System.Text;

namespace ChatGroupsContracts
{
    public class GroupMessage : Message
    {
        public string GroupName { get; set; }

        public string GroupId { get; set; }
    }
}
