﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Utilities.Collections
{
    using Comparers;

    #region Interface
    public interface IGrouping<in TKey, TValue> : IList<TValue> where TValue : IComparable<TValue>
    {
        /// <summary>
        /// Queries collections with these keys included.
        /// </summary>
        /// <returns>Ordered enumerable of TValues from selected collections.</returns>
        [NotNull]
        IEnumerable<TValue> Including([NotNull] params TKey[] keys);

        /// <summary>
        /// Queries collections with these keys excluded.
        /// </summary>
        /// <returns>Ordered enumerable of TValues from rest of collections.</returns>
        [NotNull]
        IEnumerable<TValue> Excluding([NotNull] params TKey[] keys);

        /// <summary>
        /// performs binary search to find closest item in collection.
        /// </summary>
        /// <param name="item">item to find.</param>
        /// <param name="comparer">default comparer will be used if null is passed.</param>
        int BinarySearch([NotNull] TValue item, [CanBeNull] IComparer<TValue> comparer = null);

        /// <summary>
        /// cleans up buffers used to speed up queries.
        /// </summary>
        void ReleaseBuffer();
    }
    #endregion Interface

    #region Grouping

    /// <summary>
    /// Represents a collection of grouped collections of items that have a common key.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class Grouping<TKey, TValue> : IGrouping<TKey,TValue>
        where TValue : IComparable<TValue>
        where TKey : struct, IConvertible
    {
        #region Fields

        private readonly List<TValue> _list;

        private readonly Func<TValue, TKey> _keyselector;
        private readonly Dictionary<TKey, List<TValue>> _grouping;

        // buffers used to speed up querying Including and Excluding methods.
        private readonly Dictionary<TKey[], ReadOnlyCollection<TValue>> _includes;
        private readonly Dictionary<TKey[], ReadOnlyCollection<TValue>> _excludes;

        private static readonly IEqualityComparer<TKey> EqualityComparer = EqualityComparer<TKey>.Default;
        private static readonly IComparer<TValue> Comparer = Comparer<TValue>.Default;

        #endregion Fields

        #region Constructors

        public Grouping([NotNull] Func<TValue, TKey> key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            _keyselector = key;
            _list = new List<TValue>();
            
            _includes = new Dictionary<TKey[], ReadOnlyCollection<TValue>>(ArrayComparer<TKey>.Create());
            _excludes = new Dictionary<TKey[], ReadOnlyCollection<TValue>>(ArrayComparer<TKey>.Create());

            _grouping = new Dictionary<TKey, List<TValue>>(EqualityComparer);
        }

        #endregion Constructors

        #region Methods

        private ReadOnlyCollection<TValue> PrepareBuffer(TKey[] inkeys)
        {
            // filter, flatten, order.
            return _grouping.Where(val => inkeys.Contains(val.Key, EqualityComparer))
                .SelectMany(val => val.Value)
                .OrderBy(v => v, Comparer).ToList().AsReadOnly();
        }

        /// <summary>
        /// Creates an entirely immutable version of this collection.
        /// </summary>
        public ReadOnlyGrouping<TKey, TValue> AsReadOnly()
        {
            return new ReadOnlyGrouping<TKey, TValue>(this);
        }

        #endregion Methods

        #region IGrouping Impl
        
        public IEnumerable<TValue> Including(params TKey[] keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            ReadOnlyCollection<TValue> list;
            if (!_includes.TryGetValue(keys, out list))
            {
                var exkey = _grouping.Keys.Except(keys).ToArray();
                if (!_excludes.TryGetValue(exkey, out list))
                {
                    list = PrepareBuffer(keys);
                    _excludes.Add(exkey, list);
                }
                _includes.Add(keys, list);
            }
            return list;
        }
        
        public IEnumerable<TValue> Excluding(params TKey[] keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            ReadOnlyCollection<TValue> list;
            if (!_excludes.TryGetValue(keys, out list))
            {
                var inkey = _grouping.Keys.Except(keys).ToArray();
                if (!_includes.TryGetValue(inkey, out list))
                {
                    list = PrepareBuffer(inkey);
                    _includes.Add(inkey, list);
                }
                _excludes.Add(keys, list);
            }
            return list;
        }

        public int BinarySearch(TValue item, IComparer<TValue> comparer = null)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return _list.BinarySearch(item, comparer ?? Comparer);
        }

        public void ReleaseBuffer()
        {
            _includes.Clear();
            _excludes.Clear();
        }

        #endregion IGrouping Impl

        #region IEnumerable Impl

        public IEnumerator<TValue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Impl

        #region ICollection Impl

        public void Add([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            List<TValue> list;
            var key = _keyselector(item);
            if(!_grouping.TryGetValue(key, out list))
            {
                _grouping[key] = list = new List<TValue>();
            }
            list.Add(item);
            _list.Add(item);
        }

        public void Clear()
        {
            ReleaseBuffer();
            _grouping.Clear();
            _list.Clear();
        }

        public bool Contains([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            List<TValue> list;
            return _grouping.TryGetValue(_keyselector(item), out list) && list.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            Array.Copy(_list.ToArray(), 0, array, arrayIndex, Count);
        }

        public bool Remove([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            List<TValue> list;
            _list.Remove(item);
            return _grouping.TryGetValue(_keyselector(item), out list) && list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        #endregion ICollection Impl

        #region IList Impl

        public int IndexOf([NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return _list.IndexOf(item);
        }

        public void Insert(int index, [NotNull] TValue item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            _list.RemoveAt(index);
        }

        [NotNull]
        public TValue this[int index]
        {
            get
            {
                if (index < 0 || index >= _list.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _list[index];
            }
            set
            {
                if (index < 0 || index >= _list.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                _list[index] = value;
            }
        }

        #endregion IList Impl
    }

    #endregion Grouping

    #region ReadOnlyGrouping

    /// <summary>
    /// Represents a read-only collection of grouped read-only collections of items that have a common key.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ReadOnlyGrouping<TKey, TValue> : IGrouping<TKey, TValue>, IReadOnlyList<TValue> 
        where TValue : IComparable<TValue>
        where TKey : struct, IConvertible
    {
        private readonly Grouping<TKey, TValue> _source;

        public ReadOnlyGrouping([NotNull] Grouping<TKey, TValue> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            _source = source;
        }

        #region Implemented Methods

        public IEnumerable<TValue> Including(params TKey[] keys)
        {
            return _source.Including(keys);
        }

        public IEnumerable<TValue> Excluding(params TKey[] keys)
        {
            return _source.Excluding(keys);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int BinarySearch(TValue item, IComparer<TValue> comparer = null)
        {
            return _source.BinarySearch(item, comparer);
        }

        public void ReleaseBuffer()
        {
            _source.ReleaseBuffer();
        }

        public bool Contains(TValue item)
        {
            return _source.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _source.CopyTo(array, arrayIndex);
        }

        public int Count => _source.Count;

        public bool IsReadOnly => true;

        public int IndexOf(TValue item)
        {
            return _source.IndexOf(item);
        }

        public TValue this[int index] => _source[index];

        #endregion Implemented Methods

        #region Not Supported

        TValue IList<TValue>.this[int index]
        {
            get
            {
                return _source[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        void ICollection<TValue>.Add(TValue item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TValue>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            throw new NotSupportedException();
        }
        
        void IList<TValue>.Insert(int index, TValue item)
        {
            throw new NotSupportedException();
        }

        void IList<TValue>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion Not Supported
    }

    #endregion ReadOnlyGrouping
}
