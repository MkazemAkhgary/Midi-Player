using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Utilities.Extensions
{
    public static class EnumerableExtensions
    {
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

        public static ReadOnlyCollection<TResult> ToReadOnlyCollection<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList().AsReadOnly();
        }
    }
}
