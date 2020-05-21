using ChatGroups.DTOs;
using ChatGroupsContracts;
using ChatGroupsContracts.Models;
using System;
using System.Collections.Generic;

namespace ChatGroups.Util
{
    /// <summary>
    /// Helper class for constructing types of messages.
    /// </summary>
    public class MessageConstructor
    {
        private const string system = "SYSTEM";

        public static Message SystemMessage(string body)
        {
            return new Message
            {
                SenderName = system,
                Body = body,
                Time = DateTime.UtcNow
            };
        }

        public static GroupMessageHistory GroupMessageHistory(GroupMessagesToClientDto dto)
        {
            var messages = new List<GroupMessage>();

            foreach (var item in dto.Messages)
            {
                messages.Add(new GroupMessage
                {
                    Body = item.Body,
                    GroupId = dto.GroupId,
                    GroupName = dto.GroupName,
                    SenderName = item.SenderName,
                    Time = item.Time
                });
            }

            return new GroupMessageHistory
            {
                Messages = messages
            };
        }
    }
}
