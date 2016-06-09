using System;
using System.Collections.Generic;

namespace Chronos.Extensions
{
    /// <summary>
    /// Extension methods for all kinds of (typed) enumerable data (Array, List, ...)
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts all items of a list and returns them as enumerable.
        /// </summary>
        /// <typeparam name="TSource">The source data type</typeparam>
        /// <typeparam name="TTarget">The target data type</typeparam>
        /// <param name="source">The source data.</param>
        /// <returns>The converted data</returns>
        /// <example>
        /// 
        /// var values = new[] { "1", "2", "3" };
        /// values.ConvertList&lt;string, int&gt;().ForEach(Console.WriteLine);
        /// 
        /// </example>
        public static IEnumerable<TTarget> ConvertList<TSource, TTarget>(this IEnumerable<TSource> source)
        {
            foreach (var value in source)
            {
                yield return value.ConvertTo<TTarget>();
            }
        }

        /// <summary>
        /// Performs an action for each item in the enumerable
        /// </summary>
        /// <typeparam name="T">The enumerable data type</typeparam>
        /// <param name="values">The data values.</param>
        /// <param name="action">The action to be performed.</param>
        /// <example>
        /// 
        /// var values = new[] { "1", "2", "3" };
        /// values.ConvertList&lt;string, int&gt;().ForEach(Console.WriteLine);
        /// 
        /// </example>
        /// <remarks>This method was intended to return the passed values to provide method chaining. Howver due to defered execution the compiler would actually never run the entire code at all.</remarks>
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
            {
                action(value);
            }
        }
    }
}
