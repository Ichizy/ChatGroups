using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Resources
{
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
