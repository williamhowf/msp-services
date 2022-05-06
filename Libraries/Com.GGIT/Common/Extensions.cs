using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Com.GGIT.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Check the object whether is NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Check the object whether is NOT NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(this T obj)
        {
            return !obj.IsNull();
        }

        /// <summary>
        /// Return TRUE if, and only if, length() is 0.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            return str.Length == 0;
        }

        /// <summary>
        /// Get the value with default data type
        /// </summary>
        /// <typeparam name="TKey">Dictionary Key</typeparam>
        /// <typeparam name="TValue">Dictionary Value</typeparam>
        /// <typeparam name="TActual">Value Data Type</typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetTypedValue<TKey, TValue, TActual>(this IDictionary<TKey, TValue> data, TKey key, out TActual value) where TActual : TValue
        {
            if (data.TryGetValue(key, out TValue tmp))
            {
                value = (TActual)tmp;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Tells whether or not this string matches the given regular expression.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool Matches(this string str, string pattern)
        {
            return new Regex(pattern).IsMatch(str);
        }

        /// <summary>
        /// Compares this String to another String, ignoring case considerations.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="anotherString"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str, string anotherString)
        {
            return str.Equals(anotherString, StringComparison.OrdinalIgnoreCase);
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
        where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static string ExceptionToString(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.Message);
            sb.AppendLine();
            sb.Append("Exception : ");
            sb.Append(JsonConvert.SerializeObject(ex.InnerException, Formatting.Indented));
            return sb.ToString();
        }
    }
}
