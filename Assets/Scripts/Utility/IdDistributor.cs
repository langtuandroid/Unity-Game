using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Provides utility to generate new ids
    
 */
public class IdDistributor
{
    private LinkedList<int> availableIds = new();
    private int counter = 0;

    public int GetID()
    {
        if (availableIds.Count == 0)
        {
            return NewID();
        }
        int r = availableIds.First.Value;
        availableIds.RemoveFirst();
        return r;
    }

    private int NewID()
    {
        int returnVal = counter;
        counter += 1;

        return returnVal;
    }
    public bool RecycleID(int id)
    {
        if (id >= counter || availableIds.Contains(id))
        {
            return false;
        }
        if (id == counter - 1)
        {
            counter -= 1;
        }
        else
        {
            availableIds.AddLast(id);
        }
        return true;
    }
}
