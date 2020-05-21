using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Resources
{
    //TODO: consider extraction to resource files
    public class ErrorMessages
    {
        public static string GroupDoesNotExist(string groupName)
        {
            return $"ERROR: Group {groupName} doesn't exist. Please ensure you've entered the correct name.";
        }

        public static string GroupAlreadyExists(string groupName)
        {
            return $"ERROR: Group {groupName} can't be created since it already exists.";
        }

        public static string NotAGroupMember(string groupName)
        {
            return $"ERROR: You're not a member of {groupName} group";
        }

        //TODO: add other error messages here;

        //TODO: extract other non-error messages to static files;
    }
}
