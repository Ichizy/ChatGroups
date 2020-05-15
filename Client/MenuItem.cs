using System;

namespace Client
{
    /// <summary>
    /// Represents an item of UI menu.
    /// </summary>
    public class MenuItem
    {
        public string Description { get; set; }

        public Action Action { get; set; }
    }
}
