using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Utilities.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Extracts attribute from given enumeration.
        /// </summary>
        /// <typeparam name="TAttribute">Attribute to extract</typeparam>
        /// <param name="value">enumeration value</param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>([NotNull] this Enum value) where TAttribute : Attribute
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            return value.GetType()
               .GetMember(value.ToString())[0]
               .GetCustomAttribute<TAttribute>();
        }

        public static IEnumerable<Enum> GetAllFlags([NotNull] this Enum values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));

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
