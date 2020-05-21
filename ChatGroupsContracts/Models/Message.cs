using System;

namespace ChatGroupsContracts
{
    /// <summary>
    /// Represents basic message sent by client/system.
    /// </summary>
    public class Message
    {
        public string SenderName { get; set; }

        public string Body { get; set; }

        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"{Time} {SenderName}: {Body}";
        }
    }
}