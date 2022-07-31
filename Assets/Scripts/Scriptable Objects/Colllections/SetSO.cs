using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Set")]
public class SetSO<T> : DescriptionBaseSO, IEnumerable<T>, ISet<T>
{
    private HashSet<T> container = new();

    public int Count => ((ICollection<T>)container).Count;

    public bool IsReadOnly => ((ICollection<T>)container).IsReadOnly;

    public bool Add(T item)
    {
        return ((ISet<T>)container).Add(item);
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

    public void ExceptWith(IEnumerable<T> other)
    {
        ((ISet<T>)container).ExceptWith(other);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)container).GetEnumerator();
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        ((ISet<T>)container).IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)container).IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)container).IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)container).IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return ((ISet<T>)container).IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        return ((ISet<T>)container).Overlaps(other);
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)container).Remove(item);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        return ((ISet<T>)container).SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        ((ISet<T>)container).SymmetricExceptWith(other);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        ((ISet<T>)container).UnionWith(other);
    }

    void ICollection<T>.Add(T item)
    {
        ((ICollection<T>)container).Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)container).GetEnumerator();
    }
}
