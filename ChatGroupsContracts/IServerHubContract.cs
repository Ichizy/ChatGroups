using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatGroupsContracts
{
    /// <summary>
    /// Represents available methods on server Hub, which can be called by client. 
    /// </summary>
    public interface IServerHubContract
    {
        /// <summary>
        /// Send a message to all members of a group.
        /// </summary>
        [HubMethodName(GroupMethodNames.SendToGroup)]
        Task SendToGroup(GroupMessage message);

        /// <summary>
        /// Retrieve collection of all groups.
        /// </summary>
        [HubMethodName(GroupMethodNames.ListGroups)]
        Task List();

        /// <summary>
        /// Create a new group with the name specified. Creator becomes first member by default.
        /// </summary>
        [HubMethodName(GroupMethodNames.CreateGroup)]
        Task Create(string groupName);

        /// <summary>
        /// Join existing group.
        /// </summary>
        [HubMethodName(GroupMethodNames.JoinGroup)]
        Task Join(string groupId);

        /// <summary>
        /// Leave existing group.
        /// </summary>
        [HubMethodName(GroupMethodNames.LeaveGroup)]
        Task Leave(string groupId);

        /// This method is an imitation of authentication process. Currently new client is authomatically added once he's connected. In real project, proper auth module should be called separately.
        /// </summary>
        [HubMethodName(ClientMethodNames.SignUp)]
        Task SignUp(string clientNickname);
    }
}
