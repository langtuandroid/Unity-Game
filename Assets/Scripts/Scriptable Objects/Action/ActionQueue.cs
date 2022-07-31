using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Action/ActionQueue")]
public class ActionQueue : ScriptableObject
{
    public WeightedPriorityQueue<ActionInstance> Queue = new();
}
