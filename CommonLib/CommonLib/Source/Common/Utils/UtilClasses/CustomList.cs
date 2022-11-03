using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class CustomList<T> : IList<T>
    {
        protected List<T> _customList { get; }

        public int Count => _customList.Count;

        public bool IsReadOnly { get; }

        public T this[int index]
        {
            get => _customList[index];
            set => _customList[index] = value;
        }

        public CustomList(bool isReadOnly = false)
        {
            _customList = new List<T>();
            IsReadOnly = isReadOnly;
        }

        public CustomList(List<T> list, bool isReadOnly = false)
        {
            _customList = list;
            IsReadOnly = isReadOnly;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CustomEnumerator<T>(_customList);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _customList.Add(item);
        }

        public void Clear()
        {
            _customList.Clear();
        }

        public bool Contains(T item)
        {
            return _customList.Any(s => Equals(s, item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _customList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _customList.Remove(item);
        }

        public int IndexOf(T item)
        {
            return _customList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _customList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _customList.RemoveAt(index);
        }

        public List<T> ToList => new(_customList);

        public override string ToString()
        {
            return _customList.ToString();
        }
    }

    public class CustomEnumerator<T> : IEnumerator<T>
    {
        private readonly List<T> _customList;
        private int _position = -1;

        public T Current
        {
            get
            {
                try
                {
                    return _customList[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public CustomEnumerator(List<T> customList)
        {
            _customList = customList;
        }

        public bool MoveNext()
        {
            _position++;
            return _position < _customList.Count;
        }

        public void Reset()
        {
            _position = -1;
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (!disposing) 
            //    return;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
