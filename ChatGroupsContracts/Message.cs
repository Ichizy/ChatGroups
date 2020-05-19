using System;

namespace ChatGroupsContracts
{
    public class Message
    {
        //TODO: think of properties that should be displayed here
        public string SenderName { get; set; }

        public string Body { get; set; }

        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"{Time} {SenderName}: {Body}";
        }
    }
}