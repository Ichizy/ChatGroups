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
    }
}
