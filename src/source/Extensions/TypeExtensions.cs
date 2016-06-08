using System;
using System.Reflection;

namespace Chronos.Extensions
{
    /// <summary>
    /// Extension methods for the reflection meta data type "Type"
    /// </summary>
    public static class TypeExtensions
    {
        #region · Extensions ·

        /// <summary>
        /// Creates and returns an instance of the desired type
        /// </summary>
        /// <param name="type">The type to be instanciated.</param>
        /// <param name="constructorParameters">Optional constructor parameters</param>
        /// <returns>The instanciated object</returns>
        /// <example>
        /// 	<code>
        /// var type = Type.GetType(".NET full qualified class Type")
        /// var instance = type.CreateInstance();
        /// </code>
        /// </example>
        public static object CreateInstance(this Type type, params object[] constructorParameters)
        {
            return CreateInstance<object>(type, constructorParameters);
        }

        /// <summary>
        /// Creates and returns an instance of the desired type casted to the generic parameter type T
        /// </summary>
        /// <typeparam name="T">The data type the instance is casted to.</typeparam>
        /// <param name="type">The type to be instanciated.</param>
        /// <param name="constructorParameters">Optional constructor parameters</param>
        /// <returns>The instanciated object</returns>
        /// <example>
        /// 	<code>
        /// var type = Type.GetType(".NET full qualified class Type")
        /// var instance = type.CreateInstance&lt;IDataType&gt;();
        /// </code>
        /// </example>
        public static T CreateInstance<T>(this Type type, params object[] constructorParameters)
        {
            var instance = Activator.CreateInstance(type, constructorParameters);
            return (T)instance;
        }

        #endregion
    }
}