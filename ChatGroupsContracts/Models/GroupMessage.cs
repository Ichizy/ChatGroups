using System;

namespace ChatGroupsContracts
{
    /// <summary>
    /// Represents basic message sent into a specific group of clients.
    /// </summary>
    public class GroupMessage : Message
    {
        public string GroupName { get; set; }

        public string GroupId { get; set; }

        public override string ToString()
        {
            return $"{GroupName} {Time} {SenderName}: {Body}";
        }
    }
}
