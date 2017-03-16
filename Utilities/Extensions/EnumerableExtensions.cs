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

        /// <summary>
        /// Awaits for each <see cref="Task"/> of given IEnumerable&lt;Task&lt;T out>>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="awaitableEnumerable">IEnumerable&lt;Task&lt;T out>> to perform foreach on.</param>
        /// <param name="callback">the action to perform on each result of <see cref="Task{T}"/>.</param>
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

        /// <summary>
        /// performs select and converts <see cref="IEnumerable{T}"/> To <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
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
