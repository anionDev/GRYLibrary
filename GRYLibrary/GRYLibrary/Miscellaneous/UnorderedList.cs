﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            return ToTuples(GetItemsWithCount(this)).Equals(ToTuples(GetItemsWithCount(other)));
        }

        private ISet<WriteableTuple<T, ulong>> ToTuples(IDictionary<T, ulong> dictionary)
        {
            var result = new HashSet<WriteableTuple<T, ulong>>();
            foreach(var kvp in dictionary)
            {
                result.Add(new WriteableTuple<T, ulong>(kvp.Key, kvp.Value));
            }
            return result;
        }

        private IDictionary<T, ulong> GetItemsWithCount(UnorderedList<T> items)
        {
            Dictionary<T, ulong> result = new Dictionary<T, ulong>();
            foreach(var item in items)
            {
                if(result.ContainsKey(item))
                {
                    result.Add(item, 0);
                }
                else
                {
                    result[item] = result[item] + 1;
                }
            }
            return result;
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
