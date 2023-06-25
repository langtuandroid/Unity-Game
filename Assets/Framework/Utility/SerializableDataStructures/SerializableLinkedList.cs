using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableLinkedList<T> : ISerializationCallbackReceiver, ICollection<T>, IEnumerable<T>
{
    private LinkedList<T> lst = new();
    private List<T> m_lst = new();

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

    public void OnAfterDeserialize()
    {
        foreach (T item in m_lst) {
            lst.AddLast(item);
        }
        m_lst.Clear();
    }

    public void OnBeforeSerialize()
    {
        foreach (T item in lst) {
            m_lst.Add(item);
        }
        lst.Clear();
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)lst).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)lst).GetEnumerator();
    }
}
