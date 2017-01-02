using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Linq;

namespace MidiStream.Helpers
{
    using Properties;

    public static class Extension
    {
        #region Extension Methods
        /// <summary>
        /// Filters collection of base items into collection of child items and enumerates the rest.
        /// </summary>
        /// <param name="enumerable">Master collection</param>
        /// <param name="collection">Target collection</param>
        /// <returns>rest of the items that does not match with <see cref="TChild"/></returns>
        public static IEnumerable<TBase> Sieve<TBase, TChild>(this IEnumerable<TBase> enumerable, ICollection<TChild> collection) where TChild : TBase
        {
            foreach (var val in enumerable)
            {
                if (val is TChild)
                {
                    collection.Add((TChild)val);
                }
                else yield return val;
            }
        }
        
        /// <summary>
        /// Infers function from anonymously typed lambda or delegate.
        /// </summary>
        public static Func<T, T> ToFunc<T>(Func<T, T> f) { return f; }

        public static ReadOnlyCollection<TResult> ToReadOnlyCollection<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList().AsReadOnly();
        }

        #endregion Extension Methods

        #region Extension Classes

        public static class Enumerations
        {
            /// <summary>
            /// Extracts attribute from given enumeration.
            /// </summary>
            /// <typeparam name="TAttribute">Attribute to extract</typeparam>
            /// <param name="value">enumeration value</param>
            /// <returns></returns>
            public static TAttribute GetAttribute<TAttribute>(Enum value) where TAttribute : Attribute
            {
                return value.GetType()
                   .GetMember(value.ToString())[0]
                   .GetCustomAttribute<TAttribute>();
            }

            public static IEnumerable<Enum> GetAllFlags(Enum values)
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
        
        public static class IntConverter
        {
            public static byte Reverse(byte input)
            {
                return input;
            }

            public static short Reverse(short input)
            {
                var uinput = (ushort)input;
                return (short)(((uinput & 0xFF) << 8) |
                             ((uinput & 0xFF00) >> 8));
            }

            public static int Reverse(int input)
            {
                var uinput = (uint)input;
                return (int)(((uinput & 0xFF) << 24) |
                           ((uinput & 0xFF00) << 8) |
                         ((uinput & 0xFF0000) >> 8) |
                       ((uinput & 0xFF000000) >> 24));
            }

            public static long Reverse(long input)
            {
                var uinput = (ulong)input;
                return (long)(((uinput & 0xFF) << 56) |
                           ((uinput & 0xFF00) << 40) |
                         ((uinput & 0xFF0000) << 24) |
                       ((uinput & 0xFF000000) << 8) |
                     ((uinput & 0xFF00000000) >> 8) |
                   ((uinput & 0xFF0000000000) >> 24) |
                 ((uinput & 0xFF000000000000) >> 40) |
               ((uinput & 0xFF00000000000000) >> 56));
            }

            public static int ByteArrayToInt([NotNull]byte[] input)
            {
                var val = 0;

                for (int i = 0; i < input.Length; i++)
                {
                    val <<= 8;
                    val |= input[i];
                }

                return val;
            }
        }

        #endregion Extension Classes
    }
}
