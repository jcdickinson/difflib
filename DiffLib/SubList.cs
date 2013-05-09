using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    static class SubList
    {
        public static IList<T> Sub<T>(this IList<T> list, int low = 0, int high = -1)
        {
            if (high < 0) high = list.Count - low;
            return new SubList<T>(list, low, high - low);
        }
    }

    class SubList<T> : IList<T>
    {
        private readonly IList<T> _target;
        private readonly int _offset;
        private readonly int _count;

        public SubList(IList<T> target, int low, int count)
        {
            _target = target;
            _offset = low;
            _count = count;
        }

        public int IndexOf(T item)
        {
            for (var i = 0; i < _count; i++)
                if (EqualityComparer<T>.Default.Equals(_target[i + _offset], item))
                    return i;
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                return _target[index + _offset];
            }
            set
            {
                _target[index + _offset] = value;
            }
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        private IEnumerable<T> Enumerable
        {
            get
            {
                for (var i = 0; i < _count; i++)
                    yield return _target[i + _offset];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }
    }
}
