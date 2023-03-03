using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SerializableDictionary<TKey, TValue> :
    IDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] protected List<TKey> keys = new();
    [SerializeField] protected List<TValue> values = new();
    protected Dictionary<TKey, TValue> dictionary = new();

    public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)dictionary)[key]; set => ((IDictionary<TKey, TValue>)dictionary)[key] = value; }

    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)dictionary).Keys;

    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)dictionary).Values;

    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

    public void Add(TKey key, TValue value)
    {
        ((IDictionary<TKey, TValue>)dictionary).Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)dictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        for (int i = 0;i < keys.Count;i++) {
            dictionary[keys[i]] = values[i];
        }
        keys.Clear();
        values.Clear();

    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public bool Remove(TKey key)
    {
        return ((IDictionary<TKey, TValue>)dictionary).Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return ((IDictionary<TKey, TValue>)dictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)dictionary).GetEnumerator();
    }
}

[Serializable]
public class TypeActionComponentDictionary : SerializableDictionary<string, ActionComponent> { }
[Serializable]
public class TypeActionInstanceDictionary : SerializableDictionary<string, ActionInstance> { }

[Serializable]
public class StringActionConfigDictionary : SerializableDictionary<string, ActionInstance.ActionConfig> { }