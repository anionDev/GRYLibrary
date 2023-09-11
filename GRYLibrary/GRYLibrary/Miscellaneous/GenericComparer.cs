using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous
{
    public class GenericComparer<T> : IComparer<T>, IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _Comparer;
        private readonly Func<T, int> _GetHashCode;
        public GenericComparer(Func<T, T, bool> comparer, Func<T, int> getHashCode)
        {
            this._Comparer = comparer;
            this._GetHashCode = getHashCode;
        }
        public static IComparer<T> CreateComparer(Func<T, T, bool> comparer, Func<T, int> getHashCode)
        {
            return new GenericComparer<T>(comparer, getHashCode);
        }
        public static IEqualityComparer<T> CreateEqualityComparer(Func<T, T, bool> comparer, Func<T, int> getHashCode)
        {
            return new GenericComparer<T>(comparer, getHashCode);
        }
        public int Compare(T x, T y)
        {
            return this._Comparer(x, y) ? 1 : -1;
        }

        public bool Equals(T x, T y)
        {
            return this._Comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return this._GetHashCode(obj);
        }
    }
}