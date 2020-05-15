using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatGroups.Resources
{
    public class ErrorMessages
    {
        public static string GroupDoesNotExist(string groupName)
        {
            return $"ERROR: Group {groupName} doesn't exist. Please ensure you've entered the correct name.";
        }

        //TODO: add other error messages here;

        //TODO: extract other non-error messages to static files;
    }
}
