using System;
using System.ComponentModel.DataAnnotations;

namespace ChatGroups
{
    /// <summary>
    /// Configuration settings for this application.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// The maximum amount of clients available per chat group.
        /// </summary>
        public uint MaximumGroupSize { get; set; }

        /// <summary>
        /// Key used for writing logs.
        /// </summary>
        public string InstrumentationKey { get; set; }
    }
}
