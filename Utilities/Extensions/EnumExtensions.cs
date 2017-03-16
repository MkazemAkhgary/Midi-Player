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

        /// <summary>
        /// retrieves all flags that single <see cref="Enum"/> value has.
        /// </summary>
        /// <param name="value"><see cref="Enum"/> value to retrive its flags.</param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetAllFlags([NotNull] this Enum value)
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            foreach (Enum flag in Enum.GetValues(value.GetType()))
            {
                if (value.HasFlag(flag))
                {
                    yield return flag;
                }
            }
        }
    }
}
