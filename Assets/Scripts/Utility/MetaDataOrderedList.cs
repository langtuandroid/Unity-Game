using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaDataOrderedList<T, V>
{
    /*
    Description: An ordered list that allows for meta data association

    === Private attributes ===
    container: Items stored in this queue. The first item in the queue will be popped from the front.
    size: Size of the queue
    pointer: Points to the current item being popped
    contents: A dictionary that contains the same items as container that provides random access functionlity
    */

    protected IdDistributor distributor = new();
    protected LinkedList<DataWrapper<T, V>> container = new LinkedList<DataWrapper<T, V>>();
    protected Dictionary<int, LinkedListNode<DataWrapper<T, V>>> contents = new();

    protected int size = 0;
    protected LinkedListNode<DataWrapper<T, V>> pointer = null;

    public override string ToString()
    {
        if (size == 0)
        {
            return "[]";
        }
        string result = "[";
        foreach (DataWrapper<T, V> w in container)
        {
            result = result + w.ToString() + ", ";
        }
        result = result.Remove(result.Length - 2) + "]";
        return result;
    }

    public bool ContainsKey(int key)
    {
        return contents.ContainsKey(key);
    }

    public DataWrapper<T, V> GetWrappedItem(int key)
    {
        if (contents.ContainsKey(key))
        {
            return contents[key].Value;
        }
        throw new InvalidKeyException(key + "");
    }
    public virtual int Enqueue(T element, V data, int priority)
    {
        int key = distributor.GetID();
        DataWrapper<T, V> item = new DataWrapper<T, V>(element, data, priority);

        LinkedListNode<DataWrapper<T, V>> node = container.First;
        LinkedListNode<DataWrapper<T, V>> new_node = new LinkedListNode<DataWrapper<T, V>>(item); 
        item.key = key;
        contents[key] = new_node;

        size++;
        while (node != null)
        {
            DataWrapper<T, V> current = node.Value;
            if (Compare(item, current) <= 0)
            {
                container.AddBefore(node, new_node);
                pointer = container.First;
                return key;
            }
            node = node.Next;
        }
        container.AddLast(new_node);
        pointer = container.First;
        return key;
    }

    public virtual T Dequeue()
    {
        if (size <= 0)
        {
            return default(T);
        }
        if (pointer == null)
        {
            Reset();
            return Dequeue();
        }
        DataWrapper<T, V> item = pointer.Value;
        pointer = pointer.Next;
        return item.value;
    }

    public int Size
    {
        get { return size; }
    }

    public bool Remove(int key)
    {
        if (!contents.ContainsKey(key))
        {
            return false;
        }
        LinkedListNode<DataWrapper<T, V>> node = contents[key];
        if (pointer != null && pointer.Equals(node))
        {
            pointer = pointer.Next;
        }
        container.Remove(contents[key]);
        contents.Remove(key);
        size--;
        distributor.RecycleID(key);
        return true;
    }

    public bool Remove(DataWrapper<T, V> data) {
        return Remove(data.key);
    }

    public void SetElementData(int key, V data)
    {
        if (contents.ContainsKey(key))
        {
            contents[key].Value.data = data;
        }
        else
        {
            throw new InvalidOperationException(key + "");
        }

    }

    public V GetElementData(int key)
    {
        if (contents.ContainsKey(key))
        {
            return contents[key].Value.data;
        }
        throw new InvalidKeyException(key + "");
    }

    public void SetElementValue(int key, T value)
    {
        if (contents.ContainsKey(key))
        {
            contents[key].Value.value = value;
        }
        else
        {
            throw new InvalidKeyException(key + "");
        }
    }

    public void Reset()
    {
        if (size == 0)
        {
            return;
        }
        pointer = container.First;
    }

    public bool HasNext
    {
        get { return pointer.Next != null; }
    }

    private int Compare(DataWrapper<T, V> x, DataWrapper<T, V> y)
    {
        return x.priority - y.priority;
    }
}

public class DataWrapper<T, V>
{
    public T value;
    public int priority;
    public int key;
    public V data;
    public DataWrapper(T o, V v, int p)
    {
        priority = p;
        value = o;
        data = v;
        key = -1;
    }

    public override string ToString()
    {
        return string.Format("{{Value: {0}, Priority: {1}, Data: {2}}}", value.ToString(), priority, data.ToString());
    }
}
