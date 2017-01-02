using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MidiStream.Components.Containers
{
    using Helpers;
    using Properties;
    using static Helpers.Extension;

    #region Interface
    public interface IGrouping<in TKey, TValue> : IList<TValue> where TValue : IComparable<TValue>
    {
        IEnumerable<TValue> Including(params TKey[] keys);
        IEnumerable<TValue> Excluding(params TKey[] keys);
        void ReleaseBuffer();
    }
    #endregion Interface

    #region Grouping

    /// <summary>
    /// Represents a collection of grouped collections of items that have a common key.
    /// </summary>
    public class Grouping<TKey, TValue> : IGrouping<TKey,TValue> where TValue : IComparable<TValue>
    {
        #region Fields

        private readonly IList<TValue> _list;

        private readonly Func<TValue, TKey> _keyselector;
        private readonly IDictionary<TKey, IList<TValue>> _grouping;

        // buffers used to speed up querying Including and Excluding methods.
        private readonly IDictionary<TKey[], IList<TValue>> _includes;
        private readonly IDictionary<TKey[], IList<TValue>> _excludes;

        private static readonly IEqualityComparer<TKey> EqualityComparer = EqualityComparer<TKey>.Default;
        private static readonly IComparer<TValue> Comparer = Comparer<TValue>.Default;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// this constructor is used for making read-only version of <see cref="Grouping{TKey, TValue}"/>
        /// </summary>
        /// <param name="cloning">instance to clone as read-only.</param>
        private Grouping(Grouping<TKey, TValue> cloning)
        {
            _keyselector = cloning._keyselector;
            _list = new ReadOnlyCollection<TValue>(cloning._list);

            _includes = cloning._includes;
            _excludes = cloning._excludes;

            var rolist = ToFunc<IList<TValue>>(val => new ReadOnlyCollection<TValue>(val));

            var grouping = cloning._grouping.ToDictionary(kvp => kvp.Key, kvp => rolist(kvp.Value), EqualityComparer);
            _grouping = new ReadOnlyDictionary<TKey, IList<TValue>>(grouping);
        }

        public Grouping(Func<TValue, TKey> key)
        {
            _keyselector = key;
            _list = new List<TValue>();
            
            _includes = new Dictionary<TKey[], IList<TValue>>(ArrayComparer<TKey>.Create());
            _excludes = new Dictionary<TKey[], IList<TValue>>(ArrayComparer<TKey>.Create());

            _grouping = new Dictionary<TKey, IList<TValue>>(EqualityComparer);
        }

        #endregion Constructors

        #region Methods

        private IList<TValue> PrepareBuffer(TKey[] inkeys)
        {
            // filter, flatten, order.
            return _grouping.Where(val => inkeys.Contains(val.Key, EqualityComparer))
                .SelectMany(val => val.Value)
                .OrderBy(v => v, Comparer).ToList();
        }

        /// <summary>
        /// Creates an entirely immutable version of this collection.
        /// </summary>
        public ReadOnlyGrouping<TKey, TValue> AsReadOnly()
        {
            return new ReadOnlyGrouping<TKey, TValue>(new Grouping<TKey, TValue>(this));
        }

        #endregion Methods

        #region IGrouping Impl

        /// <summary>
        /// Queries collections with these keys included.
        /// </summary>
        /// <returns>Ordered enumerable of TValues from selected collections.</returns>
        public IEnumerable<TValue> Including([NotNull]params TKey[] keys)
        {
            IList<TValue> list;
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

        /// <summary>
        /// Queries collections with these keys excluded.
        /// </summary>
        /// <returns>Ordered enumerable of TValues from rest of collections.</returns>
        public IEnumerable<TValue> Excluding([NotNull]params TKey[] keys)
        {
            IList<TValue> list;
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

        public void Add(TValue item)
        {
            IList<TValue> list;
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
            foreach (var list in _grouping.Values)
            {
                list.Clear();
            }
            _grouping.Clear();
            _list.Clear();
        }

        public bool Contains(TValue item)
        {
            IList<TValue> list;
            return _grouping.TryGetValue(_keyselector(item), out list) && list.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            Array.Copy(_list.ToArray(), 0, array, arrayIndex, Count);
        }

        public bool Remove(TValue item)
        {
            IList<TValue> list;
            _list.Remove(item);
            return _grouping.TryGetValue(_keyselector(item), out list) && list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _grouping.IsReadOnly;

        #endregion ICollection Impl

        #region IList Impl

        public int IndexOf(TValue item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public TValue this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion IList Impl
    }

    #endregion Grouping

    #region ReadOnlyGrouping

    /// <summary>
    /// Represents a read-only collection of grouped read-only collections of items that have a common key.
    /// </summary>
    public sealed class ReadOnlyGrouping<TKey, TValue> : IGrouping<TKey, TValue>, IReadOnlyList<TValue> where TValue : IComparable<TValue>
    {
        private readonly Grouping<TKey, TValue> _source;

        public ReadOnlyGrouping(Grouping<TKey, TValue> source)
        {
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
            get { return _source[index]; }
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
