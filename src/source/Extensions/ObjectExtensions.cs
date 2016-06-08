using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Chronos.Extensions
{
    /// <summary>
    /// Extension methods for the root data type object
    /// </summary>
    public static class ObjectExtensions
    {
        #region · Extensions ·

        /// <summary>
        /// Determines whether the object is equal to any of the provided values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to be compared.</param>
        /// <param name="values">The values to compare with the object.</param>
        /// <returns></returns>
        public static bool EqualsAny<T>(this T obj, params T[] values)
        {
            return (Array.IndexOf(values, obj) != -1);
        }

        /// <summary>
        /// Determines whether the object is equal to none of the provided values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to be compared.</param>
        /// <param name="values">The values to compare with the object.</param>
        /// <returns></returns>
        public static bool EqualsNone<T>(this T obj, params T[] values)
        {
            return (obj.EqualsAny(values) == false);
        }

        /// <summary>
        /// Converts an object to the specified target type or returns the default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo<T>(this object value)
        {
            return value.ConvertTo(default(T));
        }

        /// <summary>
        /// Converts an object to the specified target type or returns the default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo<T>(this object value, T defaultValue)
        {
            if (value != null)
            {
                var targetType = typeof(T);

                var converter = TypeDescriptor.GetConverter(value);
                
                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType))
                    {
                        return (T)converter.ConvertTo(value, targetType);
                    }
                }

                converter = TypeDescriptor.GetConverter(targetType);
            
                if (converter != null)
                {
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        return (T)converter.ConvertFrom(value);
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Converts an object to the specified target type or returns the default value. Any exceptions are optionally ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="ignoreException">if set to <c>true</c> ignore any exception.</param>
        /// <returns>The target type</returns>
        public static T ConvertTo<T>(this object value, T defaultValue, bool ignoreException)
        {
            if (ignoreException)
            {
                try
                {
                    return value.ConvertTo<T>();
                }
                catch
                {
                    return defaultValue;
                }
            }

            return value.ConvertTo<T>();
        }

        /// <summary>
        /// Determines whether the value can (in theory) be converted to the specified target type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can be convert to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanConvertTo<T>(this object value)
        {
            if (value != null)
            {
                var targetType = typeof(T);

                var converter = TypeDescriptor.GetConverter(value);

                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType))
                    {
                        return true;
                    }
                }

                converter = TypeDescriptor.GetConverter(targetType);

                if (converter != null)
                {
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Converts the specified value to a different type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An universal converter suppliying additional target conversion methods</returns>
        /// <example><code>
        /// var value = "123";
        /// var numeric = value.Convert().ToInt32();
        /// </code></example>
        public static IConverter<T> Convert<T>(this T value)
        {
            return new Converter<T>(value);
        }

        /// <summary>
        /// Dynamically invokes a method using reflection
        /// </summary>
        /// <param name="obj">The object to perform on.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The parameters passed to the method.</param>
        /// <returns>The return value</returns>
        /// <example>
        /// <code>
        /// var type = Type.GetType("System.IO.FileInfo, mscorlib");
        /// var file = type.CreateInstance(@"c:\autoexec.bat");
        /// if(file.GetPropertyValue&lt;bool&gt;("Exists")) {
        ///  var reader = file.InvokeMethod&lt;StreamReader&gt;("OpenText");
        ///  Console.WriteLine(reader.ReadToEnd());
        ///  reader.Close();
        /// }
        /// </code>
        /// </example>
        public static object InvokeMethod(this object obj, string methodName, params object[] parameters)
        {
            return InvokeMethod<object>(obj, methodName, parameters);
        }

        /// <summary>
        /// Dynamically invokes a method using reflection and returns its value in a typed manner
        /// </summary>
        /// <typeparam name="T">The expected return data types</typeparam>
        /// <param name="obj">The object to perform on.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The parameters passed to the method.</param>
        /// <returns>The return value</returns>
        /// <example>
        /// <code>
        /// var type = Type.GetType("System.IO.FileInfo, mscorlib");
        /// var file = type.CreateInstance(@"c:\autoexec.bat");
        /// if(file.GetPropertyValue&lt;bool&gt;("Exists")) {
        ///  var reader = file.InvokeMethod&lt;StreamReader&gt;("OpenText");
        ///  Console.WriteLine(reader.ReadToEnd());
        ///  reader.Close();
        /// }
        /// </code>
        /// </example>
        public static T InvokeMethod<T>(this object obj, string methodName, params object[] parameters)
        {
            var type    = obj.GetType();
            var method  = type.GetMethod(methodName);

            if (method == null)
            {
                throw new ArgumentException(string.Format("Method '{0}' not found.", methodName), methodName);
            }

            var value = method.Invoke(obj, parameters);

            return (value is T ? (T)value : default(T));
        }

        /// <summary>
        /// Dynamically retrieves a property value.
        /// </summary>
        /// <param name="obj">The object to perform on.</param>
        /// <param name="propertyName">The Name of the property.</param>
        /// <returns>The property value.</returns>
        /// <example>
        /// <code>
        /// var type = Type.GetType("System.IO.FileInfo, mscorlib");
        /// var file = type.CreateInstance(@"c:\autoexec.bat");
        /// if(file.GetPropertyValue&lt;bool&gt;("Exists")) {
        ///  var reader = file.InvokeMethod&lt;StreamReader&gt;("OpenText");
        ///  Console.WriteLine(reader.ReadToEnd());
        ///  reader.Close();
        /// }
        /// </code>
        /// </example>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return GetPropertyValue<object>(obj, propertyName, null);
        }

        /// <summary>
        /// Dynamically retrieves a property value.
        /// </summary>
        /// <typeparam name="T">The expected return data type</typeparam>
        /// <param name="obj">The object to perform on.</param>
        /// <param name="propertyName">The Name of the property.</param>
        /// <returns>The property value.</returns>
        /// <example>
        /// <code>
        /// var type = Type.GetType("System.IO.FileInfo, mscorlib");
        /// var file = type.CreateInstance(@"c:\autoexec.bat");
        /// if(file.GetPropertyValue&lt;bool&gt;("Exists")) {
        ///  var reader = file.InvokeMethod&lt;StreamReader&gt;("OpenText");
        ///  Console.WriteLine(reader.ReadToEnd());
        ///  reader.Close();
        /// }
        /// </code>
        /// </example>
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return GetPropertyValue<T>(obj, propertyName, default(T));
        }

        /// <summary>
        /// Dynamically retrieves a property value.
        /// </summary>
        /// <typeparam name="T">The expected return data type</typeparam>
        /// <param name="obj">The object to perform on.</param>
        /// <param name="propertyName">The Name of the property.</param>
        /// <param name="defaultValue">The default value to return.</param>
        /// <returns>The property value.</returns>
        /// <example>
        /// <code>
        /// var type = Type.GetType("System.IO.FileInfo, mscorlib");
        /// var file = type.CreateInstance(@"c:\autoexec.bat");
        /// if(file.GetPropertyValue&lt;bool&gt;("Exists")) {
        ///  var reader = file.InvokeMethod&lt;StreamReader&gt;("OpenText");
        ///  Console.WriteLine(reader.ReadToEnd());
        ///  reader.Close();
        /// }
        /// </code>
        /// </example>
        public static T GetPropertyValue<T>(this object obj, string propertyName, T defaultValue)
        {
            var type        = obj.GetType();
            var property    = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException(string.Format("Property '{0}' not found.", propertyName), propertyName);
            }

            var value = property.GetValue(obj, null);

            return (value is T ? (T)value : defaultValue);
        }

        /// <summary>
        /// Dynamically sets a property value.
        /// </summary>
        /// <param name="obj">The object to perform on.</param>
        /// <param name="propertyName">The Name of the property.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            var type        = obj.GetType();
            var property    = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException(string.Format("Property '{0}' not found.", propertyName), propertyName);
            }

            property.SetValue(obj, value, null);
        }

        /// <summary>
        /// Gets the first matching attribute defined on the data type.
        /// </summary>
        /// <typeparam name="T">The attribute type tp look for.</typeparam>
        /// <param name="obj">The object to look on.</param>
        /// <returns>The found attribute</returns>
        public static T GetAttribute<T>(this object obj) where T : Attribute
        {
            return GetAttribute<T>(obj, true);
        }

        /// <summary>
        /// Gets the first matching attribute defined on the data type.
        /// </summary>
        /// <typeparam name="T">The attribute type tp look for.</typeparam>
        /// <param name="obj">The object to look on.</param>
        /// <param name="includeInherited">if set to <c>true</c> includes inherited attributes.</param>
        /// <returns>The found attribute</returns>
        public static T GetAttribute<T>(this object obj, bool includeInherited) where T : Attribute
        {
            var type        = (obj as Type ?? obj.GetType());
            var attributes  = type.GetCustomAttributes(typeof(T), includeInherited);

            if ((attributes != null) && (attributes.Length > 0))
            {
                return (attributes[0] as T);
            }

            return null;
        }

        /// <summary>
        /// Gets all matching attribute defined on the data type.
        /// </summary>
        /// <typeparam name="T">The attribute type tp look for.</typeparam>
        /// <param name="obj">The object to look on.</param>
        /// <returns>The found attributes</returns>
        public static IEnumerable<T> GetAttributes<T>(this object obj) where T : Attribute
        {
            return GetAttributes<T>(obj);
        }

        /// <summary>
        /// Gets all matching attribute defined on the data type.
        /// </summary>
        /// <typeparam name="T">The attribute type tp look for.</typeparam>
        /// <param name="obj">The object to look on.</param>
        /// <param name="includeInherited">if set to <c>true</c> includes inherited attributes.</param>
        /// <returns>The found attributes</returns>
        public static IEnumerable<T> GetAttributes<T>(this object obj, bool includeInherited) where T : Attribute
        {
            var type = (obj as Type ?? obj.GetType());
            
            foreach (var attribute in type.GetCustomAttributes(typeof(T), includeInherited))
            {
                if (attribute is T)
                {
                    yield return (T)attribute;
                }
            }
        }

        #endregion
    }
}