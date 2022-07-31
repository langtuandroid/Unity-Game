using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collections/List")]
public class ListSO<T> : DescriptionBaseSO, IEnumerable<T>, IList<T>
{
    private List<T> container = new();

    public T this[int index] { get => ((IList<T>)container)[index]; set => ((IList<T>)container)[index] = value; }

    public int Count => ((ICollection<T>)container).Count;

    public bool IsReadOnly => ((ICollection<T>)container).IsReadOnly;

    public void Add(T item)
    {
        ((ICollection<T>)container).Add(item);
    }

    public void Clear()
    {
        ((ICollection<T>)container).Clear();
    }

    public bool Contains(T item)
    {
        return ((ICollection<T>)container).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)container).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)container).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return ((IList<T>)container).IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ((IList<T>)container).Insert(index, item);
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)container).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<T>)container).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)container).GetEnumerator();
    }
}
