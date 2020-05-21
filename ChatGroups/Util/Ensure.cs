using System;

namespace ChatGroups.Util
{
    /// <summary>
    /// Provides static methods to verify the input parameters and throws exception if an argument is wrong.
    /// </summary>
    public class Ensure
    {
        /// <summary>
        /// Verifies that object is not NULL. If a string is null the method will throw ArgumentNullException.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void NotNull(string valueName, object value)
        {
            if (ReferenceEquals(valueName, null))
            {
                throw new ArgumentNullException(nameof(valueName));
            }

            if (string.IsNullOrEmpty(valueName))
            {
                throw new ArgumentException($"The {nameof(valueName)} can not be empty", nameof(valueName));
            }

            if (value is null)
            {
                throw new ArgumentNullException(valueName);
            }
        }
    }
}
