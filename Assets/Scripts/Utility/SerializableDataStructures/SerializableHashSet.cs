using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializableHashSet<TValue> : ISet<TValue>, ISerializationCallbackReceiver
{
    protected HashSet<TValue> m_HashSet = new();
    [SerializeField] protected List<TValue> values = new();

    public int Count => ((ICollection<TValue>)m_HashSet).Count;

    public bool IsReadOnly => ((ICollection<TValue>)m_HashSet).IsReadOnly;

    public bool Add(TValue item)
    {
        return ((ISet<TValue>)m_HashSet).Add(item);
    }

    public void Clear()
    {
        ((ICollection<TValue>)m_HashSet).Clear();
    }

    public bool Contains(TValue item)
    {
        return ((ICollection<TValue>)m_HashSet).Contains(item);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        ((ICollection<TValue>)m_HashSet).CopyTo(array, arrayIndex);
    }

    public void ExceptWith(IEnumerable<TValue> other)
    {
        ((ISet<TValue>)m_HashSet).ExceptWith(other);
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return ((IEnumerable<TValue>)m_HashSet).GetEnumerator();
    }

    public void IntersectWith(IEnumerable<TValue> other)
    {
        ((ISet<TValue>)m_HashSet).IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<TValue> other)
    {
        return ((ISet<TValue>)m_HashSet).IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<TValue> other)
    {
        return ((ISet<TValue>)m_HashSet).IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<TValue> other)
    {
        return ((ISet<TValue>)m_HashSet).IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<TValue> other)
    {
        return ((ISet<TValue>)m_HashSet).IsSupersetOf(other);
    }

    public void OnAfterDeserialize()
    {
        m_HashSet.Clear();
        for (int i = 0; i < values.Count; i++)
        {
            m_HashSet.Add(values[i]);
        }
        values.Clear();
    }

    public void OnBeforeSerialize()
    {
        values.Clear();
        foreach (TValue v in m_HashSet)
        {
            values.Add(v);
        }
    }

    public bool Overlaps(IEnumerable<TValue> other)
    {
        return ((ISet<TValue>)m_HashSet).Overlaps(other);
    }

    public bool Remove(TValue item)
    {
        return ((ICollection<TValue>)m_HashSet).Remove(item);
    }

    public bool SetEquals(IEnumerable<TValue> other)
    {
        return ((ISet<TValue>)m_HashSet).SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<TValue> other)
    {
        ((ISet<TValue>)m_HashSet).SymmetricExceptWith(other);
    }

    public void UnionWith(IEnumerable<TValue> other)
    {
        ((ISet<TValue>)m_HashSet).UnionWith(other);
    }

    void ICollection<TValue>.Add(TValue item)
    {
        ((ICollection<TValue>)m_HashSet).Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)m_HashSet).GetEnumerator();
    }

    public List<TValue> GetValueInList() { 
        List<TValue> list = new List<TValue>();
        foreach (TValue item in m_HashSet) { 
            list.Add(item);
        }
        return list;
    }
}

[Serializable]
public class ActionSet : SerializableHashSet<ActionInstance> { }
