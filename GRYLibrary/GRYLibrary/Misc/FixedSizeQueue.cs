using System.Collections.Generic;

namespace GRYLibrary.Core.Misc
{
    public class FixedSizeQueue<T>
    {
        private readonly Queue<T> _Queue = new();
        private readonly int _MaxSize;

        public int Count => _Queue.Count;


        public FixedSizeQueue() : this(int.MaxValue)
        {
        }
        public FixedSizeQueue(int maxSize)
        {
            GRYLibrary.Core.Misc.Utilities.AssertCondition(0 < maxSize, $"{maxSize} must be grater than 0.");
            _MaxSize = maxSize;
        }
        public T[] GetEntries() => _Queue.ToArray();

        public void Enqueue(T item)
        {
            if (0<_Queue.Count && _Queue.Count + 1 < _MaxSize)
            {
                _Queue.Dequeue();
            }
            _Queue.Enqueue(item);
        }
        public T Dequeue()
        {
            return _Queue.Dequeue();
        }
    }
}
