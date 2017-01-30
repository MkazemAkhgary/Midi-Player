using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
        public static IEnumerable<TBase> Sieve<TBase, TChild>(
            [NotNull] this IEnumerable<TBase> enumerable,
            [NotNull] ICollection<TChild> collection) where TChild : TBase
        {
            if(enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if(collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var val in enumerable)
            {
                if (val is TChild)
                {
                    collection.Add((TChild)val);
                }
                else yield return val;
            }
        }

        public static async Task AwaitForeach<TResult>(
            [NotNull] this IEnumerable<Task<TResult>> awaitableEnumerable, 
            [NotNull] Action<TResult> callback)
        {
            if(awaitableEnumerable == null)
                throw new ArgumentNullException(nameof(awaitableEnumerable));
            if(callback == null)
                throw new ArgumentNullException(nameof(callback));

            foreach (var task in awaitableEnumerable)
            {
                callback(await task);
            }
        }

        public static ReadOnlyCollection<TResult> ToReadOnlyCollection<TSource, TResult>(
            [NotNull] this IEnumerable<TSource> source,
            [NotNull] Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return source.Select(selector).ToList().AsReadOnly();
        }
    }
}
