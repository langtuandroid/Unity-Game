using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Utility
{
    /// <summary>
    /// A list that disables editing when entering playmode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class XList<T> : IList<T>, IEnumerable<T>
    {
        [SerializeField] private List<T> lst;

        public T this[int index] { get => ((IList<T>)lst)[index]; set => ((IList<T>)lst)[index] = value; }

        public int Count => ((ICollection<T>)lst).Count;

        public bool IsReadOnly => ((ICollection<T>)lst).IsReadOnly;

        public void Add(T item)
        {
            ((ICollection<T>)lst).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)lst).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)lst).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)lst).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)lst).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)lst).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)lst).Insert(index, item);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)lst).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)lst).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)lst).GetEnumerator();
        }
    }
}
