using System.Collections;
using System.Collections.Specialized;

namespace MidiStream.Helpers
{
    using Properties;
    /// <summary>
    /// Generic wrapper for <see cref="ListDictionary"/>
    /// </summary>
    public class ListDictionary<TKey, TValue> : ListDictionary
    {
        #region Constructors

        public ListDictionary()
        {
        }

        public ListDictionary(IComparer comparer) : base(comparer)
        {
        }

        #endregion Constructors

        #region Public Methods

        public TValue this[[NotNull]TKey key]
        {
            get { return (TValue)base[key]; }
            set { base[key] = value; }
        }

        public void Add(TKey key, TValue value)
        {
            base.Add(key, value);
        }

        #endregion Public Methods
    }
}
