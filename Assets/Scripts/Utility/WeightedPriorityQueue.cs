using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPriorityQueue<T> : MetaDataOrderedList<T, int>, IEnumerable<DataWrapper<T, int>>
{
    /*
    Description: A queue of items sorted by their priorities, items with higher
    priority will be popped first. Additionally, each item has a weight factor
    assigned to it. Popping items will cause their weight factor to drop by 1.
    The item will not be fully removed from the queue if the weight factor is
    greater than 0. Enqueuing an item returns the key to access the weight
    factor of that item. This can be used to modify its weight factor.

    === Private attributes ===
    container: Items stored in this queue. The first item in the queue will be popped from the front.
    size: Size of the queue
    pointer: Points to the current item being popped
    contents: A dictionary that contains the same items as container that provides random access functionlity
    */
    public override T Dequeue()
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
        DataWrapper<T, int> item = pointer.Value;
        item.data -= 1;
        if (item.data <= 0)
        {
            Remove(pointer.Value);

            if (item.data < 0)
            {
                return Dequeue();
            }
        }
        else { 
            pointer = pointer.Next;
        }
        return item.value;
    }

    public IEnumerator<DataWrapper<T, int>> GetEnumerator()
    {
        return container.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return container.GetEnumerator();
    }
}

