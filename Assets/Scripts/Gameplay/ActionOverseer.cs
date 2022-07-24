using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOverseer : MonoBehaviour
{
    [SerializeField] private ActionQueue queue;
    void LateUpdate()
    { 
        WeightedPriorityQueue<ActionInstance> actionQueue = queue.Queue;
        ActionInstance a = actionQueue.Dequeue();
        while (a != default)
        {
            a.Execute();
            a = actionQueue.Dequeue();
        }
        actionQueue.Reset();
    }
}
