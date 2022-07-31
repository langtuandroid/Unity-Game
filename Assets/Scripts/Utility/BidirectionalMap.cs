using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BidirectionalMap<T, K> : IEnumerable<KeyValuePair<T, K>>
{
    private Dictionary<T, K> f_Dictionary = new();
    private Dictionary<K, T> r_Dictionary = new();

    public K this[T key] {
        get { return f_Dictionary[key]; }
        set { f_Dictionary[key] = value;
            r_Dictionary[value] = key;
        }
    }

    public T Key(K value) {
        return r_Dictionary[value];
    }

    public bool ContainsKey(T key) {
        return f_Dictionary.ContainsKey(key);
    }

    public bool ContainsValue(K value) {
        return r_Dictionary.ContainsKey(value);
    }

    public bool Remove(T key) {
        if (f_Dictionary.ContainsKey(key)) {
            r_Dictionary.Remove(f_Dictionary[key]);
            f_Dictionary.Remove(key);
            return true;
        }
        return false;
    }

    public bool RemoveValue(K value) {
        if (r_Dictionary.ContainsKey(value))
        {
            f_Dictionary.Remove(r_Dictionary[value]);
            r_Dictionary.Remove(value);
            return true;
        }
        return false;
    }

    public IEnumerator<KeyValuePair<T, K>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T, K>>)f_Dictionary).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)f_Dictionary).GetEnumerator();
    }
}
