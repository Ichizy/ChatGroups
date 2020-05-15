using System;
using System.Collections.Generic;
using System.Text;

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
                //new MenuItem {Description = "View balance", Execute = ViewBalance}
            };
        }
    }

    public class MenuItem
    {
        public string Description { get; set; }
        public Action Execute { get; set; }
    }
}
