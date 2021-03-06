﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Utilities.Threading
{
    /// <summary>
    /// Encapsulates an object. can block or release access to containing object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BlockingContainer<T> where T : class
    {
        private volatile bool _isFree = true;
        private readonly T _item;

        /// <summary>
        /// blocks access to item for specified amount of time.
        /// </summary>
        /// <param name="item">encapsulating item.</param>
        public BlockingContainer(T item)
        {
            _item = item;
        }
        
        /// <summary>
        /// returns encapsulated item if container is free.
        /// </summary>
        [CanBeNull]
        public T RequestItem()
        {
            return _isFree ? _item : null;
        }

        /// <summary>
        /// asks for encapsulated item. will return item after the specified duration.
        /// </summary>
        [ItemCanBeNull]
        public async Task<T> Reserve(int wait)
        {
            if (wait < 0)
                throw new ArgumentOutOfRangeException(nameof(wait));

            return await Block(wait);
        }
        
        private async Task<T> Block(int wait)
        {
            if (!_isFree) return null;
            _isFree = false;
            await Task.Delay(wait).ConfigureAwait(continueOnCapturedContext: false);
            _isFree = true;
            return _item;
        }
    }
}
