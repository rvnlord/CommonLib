using System;
using System.Collections.Generic;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class PriorityQueue<T>
    {
        private T[] _heap;
        private readonly IComparer<T> _comparer;
        private bool _isHeap;
        private const int DefaultCapacity = 6;

        public int Count { get; private set; }

        public T Top
        {
            get
            {
                if (!_isHeap) Heapify();
                return _heap[0];
            }
        }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            _heap = new T[capacity > 0 ? capacity : DefaultCapacity];
            Count = 0;
            _comparer = comparer;
        }

        public void Push(T value)
        {
            if (Count == _heap.Length)
                Array.Resize(ref _heap, Count * 2);

            if (_isHeap)
                SiftUp(Count, ref value, 0);
            else
                _heap[Count] = value;

            Count++;
        }

        public void Pop()
        {
            if (Count <= 0) return;

            --Count;
            var ix = 0;
            while (ix < Count / 2)
            {
                var smallestChild = HeapLeftChild(ix);
                var rightChild = HeapRightFromLeft(smallestChild);

                if (rightChild < Count && _comparer.Compare(_heap[rightChild], _heap[smallestChild]) < 0)
                    smallestChild = rightChild;

                if (_comparer.Compare(_heap[Count], _heap[smallestChild]) <= 0)
                    break;

                _heap[ix] = _heap[smallestChild];
                ix = smallestChild;
            }

            _heap[ix] = _heap[Count];
            _heap[Count] = default;
        }

        private int SiftDown(int index)
        {
            var parent = index;
            var leftChild = HeapLeftChild(parent);

            while (leftChild < Count)
            {
                var rightChild = HeapRightFromLeft(leftChild);
                var bestChild = rightChild < Count && _comparer.Compare(_heap[rightChild], _heap[leftChild]) < 0 ? rightChild : leftChild;

                _heap[parent] = _heap[bestChild];

                parent = bestChild;
                leftChild = HeapLeftChild(parent);
            }

            return parent;
        }

        private void SiftUp(int index, ref T x, int boundary)
        {
            while (index > boundary)
            {
                var parent = HeapParent(index);
                if (_comparer.Compare(_heap[parent], x) > 0)
                {
                    _heap[index] = _heap[parent];
                    index = parent;
                }
                else
                    break;
            }
            _heap[index] = x;
        }

        private void Heapify()
        {
            if (_isHeap) 
                return;

            for (var i = Count / 2 - 1; i >= 0; --i)
            {
                var x = _heap[i];
                var index = SiftDown(i);
                SiftUp(index, ref x, i);
            }

            _isHeap = true;
        }

        private static int HeapParent(int i) => (i - 1) / 2;
        private static int HeapLeftChild(int i) => i * 2 + 1;
        private static int HeapRightFromLeft(int i) => i + 1;
    }
}
