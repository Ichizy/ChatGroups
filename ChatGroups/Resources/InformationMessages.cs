using System;

namespace ChatGroups.Resources
{
    //TBD: I would consider moving this data into some .txt or any other file.
    public class InformationMessages
    {
        public static string SuccessfullyJoinedGroup(string groupName)
        {
            return $"You've successfully joined {groupName} group. Now you may start chating.";
        }

        public static string ClientHasJoinedGroup(string clientName)
        {
            return $"{clientName} has joined group";
        }

        public static string SuccessfullyLeftGroup(string groupName)
        {
            return $"You've successfully left {groupName} group.";
        }

        public static string ClientHasLeftGroup(string clientName)
        {
            return $"{clientName} has left the group.";
        }

        public static string GroupSuccessfullyCreated(string groupName)
        {
            return $"Group {groupName} successfully created.";
        }

        public static string ClientsInGroupLimitReached(string groupName, uint maximumClientsAllowed)
        {
            return $"Sorry, you can't join {groupName} since there're already a limit of {maximumClientsAllowed} clients reached.";
        }
    }
}
