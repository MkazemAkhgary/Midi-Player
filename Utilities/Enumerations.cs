using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utilities
{
    public static class Enumerations
    {
        /// <summary>
        /// Extracts attribute from given enumeration.
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to extract</typeparam>
        /// <param name="value">enumeration value</param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            return value.GetType()
               .GetMember(value.ToString())[0]
               .GetCustomAttribute<TAttribute>();
        }

        public static IEnumerable<Enum> GetAllFlags(this Enum values)
        {
            foreach (Enum value in Enum.GetValues(values.GetType()))
            {
                if (values.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}
