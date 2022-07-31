using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Provides utility to generate new ids
    
 */
public class IdDistributor
{
    private List<int> availableIds = new List<int>();
    private int counter = 0;

    public int GetID()
    {
        if (availableIds.Count == 0)
        {
            return NewID();
        }
        int r = availableIds[0];
        availableIds.RemoveAt(0);
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
            availableIds.Add(id);
        }
        return true;
    }
}
