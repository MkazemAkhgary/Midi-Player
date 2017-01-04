using System.Collections.Generic;

namespace Utilities.Helpers
{
    /// <summary>
    /// Provides <see cref="IEqualityComparer{T}"/> for an array of <see cref="T"/>.
    /// Must call <see cref="ArrayComparer{T}.Create"/> to get singleton instance.
    /// </summary>
    /// <typeparam name="T">the type of the elements of the array.</typeparam>
    public sealed class ArrayComparer<T> : IEqualityComparer<T[]>
    {
        private static readonly ArrayComparer<T> Singleton = new ArrayComparer<T>();

        private readonly IEqualityComparer<T> _comparer;

        private ArrayComparer()
        {
            _comparer = EqualityComparer<T>.Default;
        }

        public static ArrayComparer<T> Create()
        {
            return Singleton;
        }

        #region Implemented Methods

        public bool Equals(T[] x, T[] y)
        {
            if (x == null || y == null) return false;
            if (ReferenceEquals(x, y)) return true;
            if (x.Length != y.Length) return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (!_comparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(T[] obj)
        {
            if (obj == null) return 0;
            unchecked
            {
                int hash = 13;
                for (int i = 0; i < obj.Length; i++)
                {
                    hash *= 9 + _comparer.GetHashCode(obj[i]);
                }
                return hash;
            }
        }

        #endregion Implemented Methods
    }
}
