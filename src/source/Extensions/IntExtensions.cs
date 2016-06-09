using System;

namespace Chronos.Extensions
{
    /// <summary>
    /// Extension methods for the string data type
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Performs the specified action n times based on the underlying int value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        public static void Times(this int value, Action action)
        {
            for (var i = 0; i < value; i++)
            {
                action();
            }
        }

        /// <summary>
        /// Performs the specified action n times based on the underlying int value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        public static void Times(this int value, Action<int> action)
        {
            for (var i = 0; i < value; i++)
            {
                action(i);
            }
        }
    }
}
