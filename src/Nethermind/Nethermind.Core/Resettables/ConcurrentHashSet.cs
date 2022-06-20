using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections;

namespace Nethermind.Core.Resettables
{
    internal class ConcurrentHashSet<T>: IEnumerable<T>, ICollection<T>
    {
        private ConcurrentDictionary<T, byte> _dict = new();
        public ConcurrentHashSet(int startCapacity)
        {
            _dict = new ConcurrentDictionary<T, byte>(127, startCapacity);
        }

        public int Count => _dict.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            _dict.TryAdd(item, 0);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(T item)
        {
            return _dict.TryGetValue(item, out _);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dict.Keys.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return _dict.TryRemove(item, out _);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }
    }
}
