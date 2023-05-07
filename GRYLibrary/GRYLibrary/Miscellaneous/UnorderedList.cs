using System;
using System.Collections;
using System.Collections.Generic;

namespace GRYLibrary.Core.Miscellaneous
{
    public class UnorderedList<T> :IList<T>, IEquatable<UnorderedList<T>>
    {
        private readonly IList<T> _Items = new List<T>();
        public int Count => _Items.Count;

        public bool IsReadOnly => _Items.IsReadOnly;

        public T this[int index] { get => _Items[index]; set => _Items[index] = value; }

        public override int GetHashCode()
        {
            return HashCode.Combine(_Items.Count.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnorderedList<T>);
        }

        /// <returns>Returns true if and only if there is a bijection between this and <paramref name="other"/>.</returns>
        public bool Equals(UnorderedList<T> other)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            return _Items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _Items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _Items.RemoveAt(index);
        }

        public void Add(T item)
        {
            _Items.Add(item);
        }

        public void Clear()
        {
            _Items.Clear();
        }

        public bool Contains(T item)
        {
            return _Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _Items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }
}
