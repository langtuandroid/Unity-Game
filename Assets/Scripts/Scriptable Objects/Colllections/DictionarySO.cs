using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collections/Dictionary")]
public class DictionarySO<T, E> : DescriptionBaseSO, IDictionary<T, E>, IEnumerable<KeyValuePair<T, E>>
{
    private Dictionary<T, E> container = new();

    public E this[T key] { get => ((IDictionary<T, E>)container)[key]; set => ((IDictionary<T, E>)container)[key] = value; }

    public ICollection<T> Keys => ((IDictionary<T, E>)container).Keys;

    public ICollection<E> Values => ((IDictionary<T, E>)container).Values;

    public int Count => ((ICollection<KeyValuePair<T, E>>)container).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<T, E>>)container).IsReadOnly;

    public void Add(T key, E value)
    {
        ((IDictionary<T, E>)container).Add(key, value);
    }

    public void Add(KeyValuePair<T, E> item)
    {
        ((ICollection<KeyValuePair<T, E>>)container).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<T, E>>)container).Clear();
    }

    public bool Contains(KeyValuePair<T, E> item)
    {
        return ((ICollection<KeyValuePair<T, E>>)container).Contains(item);
    }

    public bool ContainsKey(T key)
    {
        return ((IDictionary<T, E>)container).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<T, E>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T, E>>)container).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<T, E>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T, E>>)container).GetEnumerator();
    }

    public bool Remove(T key)
    {
        return ((IDictionary<T, E>)container).Remove(key);
    }

    public bool Remove(KeyValuePair<T, E> item)
    {
        return ((ICollection<KeyValuePair<T, E>>)container).Remove(item);
    }

    public bool TryGetValue(T key, out E value)
    {
        return ((IDictionary<T, E>)container).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)container).GetEnumerator();
    }
}
