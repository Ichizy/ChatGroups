using ChatGroupsContracts.Models;
using Microsoft.AspNetCore.SignalR;
using System;

namespace ChatGroupsContracts
{
    /// <summary>
    /// Represents methods which should be implemented on client side to work properly with all possible functionality. 
    /// </summary>
    public interface IClientHubContract
    {
        /// <summary>
        /// Receive a message sent to a group.
        /// </summary>
        [HubMethodName(MessageMethodNames.ReceiveGroupMessage)]
        Action ReceiveMessage(GroupMessage message);

        /// <summary>
        /// Receive message history for a group. Triggered once client joins a new group.
        /// </summary>
        [HubMethodName(MessageMethodNames.ReceiveMessageHistory)]
        Action ReceiveGroupHistory(GroupMessageHistory history);

        /// <summary>
        /// Contains information about group, which client has just joined.
        /// </summary>
        [HubMethodName(GroupMethodNames.OnGroupJoined)]
        Action OnGroupJoined(GroupMessage message);
    }
}
