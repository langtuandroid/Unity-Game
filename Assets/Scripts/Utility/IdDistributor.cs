using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Provides utility to generate new ids
    
 */
public class IdDistributor {
    
    // Class used for generating new ids
    private static Dictionary<string, IdGenerator> table = new Dictionary<string, IdGenerator>();
    public static int GetId(string key) {
        if (!table.ContainsKey(key)) {
            table.Add(key, new IdGenerator());
        }
        return table[key].NewID();
    }

    public static bool RecycleId(string key, int num) {
        if (!table.ContainsKey(key)) {
            return false;
        }
        return table[key].DeleteID(num);
    }


    private class IdGenerator
    {
        private HashSet<int> availableIds = new HashSet<int>();
        private int counter = 0;

        public int NewID()
        {
            int returnVal = counter;
            counter += 1;

            return returnVal;
        }
        public bool DeleteID(int id)
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
}
