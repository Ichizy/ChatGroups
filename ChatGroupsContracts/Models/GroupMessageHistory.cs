using System.Collections.Generic;

namespace ChatGroupsContracts.Models
{
    /// <summary>
    /// Contains collection of messages (message history) sent into a specific group.
    /// </summary>
    public class GroupMessageHistory
    {
        public IList<GroupMessage> Messages { get; set; }
    }
}
