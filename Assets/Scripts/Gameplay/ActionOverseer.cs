using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOverseer : MonoBehaviour
{
    [SerializeField] private ActionQueue queue;
    void LateUpdate()
    { 
        WeightedPriorityQueue<ActionInstance> actionQueue = queue.Queue;
        int size = actionQueue.Size;
        for(int i = 0;i < size;i++)
        {
            ActionInstance instance = actionQueue.Dequeue();
            instance.Execute();
        }
        actionQueue.Reset();
    }
}
