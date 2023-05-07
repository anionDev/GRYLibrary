using System;
using System.Collections;
using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous
{
    public class UnorderedList<T> :IEnumerable<T>, IEquatable<UnorderedList<T>>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object other)
        {
            return Equals(other as UnorderedList<T>);
        }

        public override int GetHashCode()
        {
            return 42;//TODO
        }
        /// <returns>Returns true if and only if there is a bujection between this and <paramref name="other"/>.</returns>
        public bool Equals(UnorderedList<T> other)
        {
            throw new NotImplementedException();
        }
    }
}
