using System;

namespace ChatGroups.Resources
{
    //TBD: I would consider moving this data into some .txt or any other file.
    public class ErrorMessages
    {
        public static string GroupDoesNotExist(string groupId)
        {
            return $"ERROR: Group {groupId} doesn't exist. Please ensure you've entered the correct name.";
        }

        public static string GroupAlreadyExists(string groupName)
        {
            return $"ERROR: Group {groupName} can't be created since it already exists.";
        }

        public static string NotAGroupMember(string groupName)
        {
            return $"ERROR: You're not a member of {groupName} group";
        }

        public static string AlreadyAGroupMember(string groupName)
        {
            return $"ERROR: You're already a member of {groupName} group.";
        }
    }
}