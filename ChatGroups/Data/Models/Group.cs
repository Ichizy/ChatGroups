using System;
using System.ComponentModel.DataAnnotations;

namespace ChatGroups.Data.Models
{
    public class Group
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Exposed ID used by clients.
        /// </summary>
        public string PublicId { get; set; }

        public Group(string name)
        {
            //TBD: Check here for NULLable name;
            Name = name;
            PublicId = Guid.NewGuid().ToString();
        }
    }
}
