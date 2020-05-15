using System;
using System.Collections.Generic;

namespace Client
{
    //TODO: implement best approach on handling Menu items
    public class JobRunner
    {
        private static List<MenuItem> MenuItems;

        private static void PopulateMenuItems()
        {
            MenuItems = new List<MenuItem>
            {
                new MenuItem { Description = "Create new group", Action = () => { Console.WriteLine("Hello!"); } }
            };
        }
    }
}
