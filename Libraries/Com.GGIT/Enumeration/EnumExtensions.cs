using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.GGIT.Enumeration
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum en)
        {
            EnumDescription[] attributes = (EnumDescription[])en.GetType().GetField(en.ToString()).GetCustomAttributes(typeof(EnumDescription), false);
            return attributes.Length > 0 ? attributes[0].Value : string.Empty;
        }

        public static string ToValue(this Enum en)
        {
            EnumValue[] attributes = (EnumValue[])en.GetType().GetField(en.ToString()).GetCustomAttributes(typeof(EnumValue), false);
            return attributes.Length > 0 ? attributes[0].Value : string.Empty;
        }

        public static T GetEnumFromValue<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(EnumValue)) is EnumValue attribute)
                {
                    if (attribute.Value == value) return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "Value");
        }

        public static bool IsEnumValueExist<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(EnumValue)) is EnumValue attribute)
                {
                    if (attribute.Value == value) return true;
                }
            }
            return false;
        }

        public static T GetEnumFromDescription<T>(string value)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(EnumDescription)) is EnumDescription attribute)
                {
                    if (attribute.Value == value) return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "Description");
        }

        public static IEnumerable<Enum> ToEnumList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<Enum>();
        }
    }
}
