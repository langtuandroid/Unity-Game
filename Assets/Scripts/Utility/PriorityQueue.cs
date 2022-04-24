using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> { 
    /*
     Description: A queue of items sorted by their priorities, the ones with lower priorities will be popped out first
     */

    private LinkedList<PriorityWrapper<T>> container = new LinkedList<PriorityWrapper<T>>();
    private int size = 0;

    public void Enqueue(T value, int priority)  {
        PriorityWrapper<T> item = new PriorityWrapper<T>(value, priority);
        LinkedListNode<PriorityWrapper<T>> node = container.First;
        LinkedListNode<PriorityWrapper<T>> new_node = new LinkedListNode<PriorityWrapper<T>>(item);
        size++;
        while (node != null)
        {
            PriorityWrapper<T> current = node.Value;
            if (Compare(item, current) <= 0)
            {
                container.AddBefore(node, new_node);
                return;
            }
            node = node.Next;
        }
        container.AddLast(new_node);
    }

    public void Enqueue(PriorityWrapper<T> item)
    {
        LinkedListNode<PriorityWrapper<T>> node = container.First;
        LinkedListNode<PriorityWrapper<T>> new_node = new LinkedListNode<PriorityWrapper<T>>(item);
        size++;
        while (node != null)
        {
            PriorityWrapper<T> current = node.Value;
            if (Compare(item, current) <= 0) {
                container.AddBefore(node, new_node);
                return;
            }
            node = node.Next;
        }
        container.AddLast(new_node);
    }

    public T Dequeue() {
        if (size == 0) {
            return default(T);
        }
        LinkedListNode<PriorityWrapper<T>> item = container.First;
        T i = item.Value.obj;
        container.RemoveFirst();
        size--;
        return i;
    }

    public int Size() {
        return size;
    }

    public int Compare(PriorityWrapper<T> x, PriorityWrapper<T> y)
    {
        return x.priority - y.priority;
    }
}

public struct PriorityWrapper<T> {
    public T obj;
    public int priority;

    public PriorityWrapper(T o, int p) {
        obj = o;
        priority = p;
    }
}
