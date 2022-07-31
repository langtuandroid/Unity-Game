using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Action/ActionData")]
public class ActionData : ScriptableObject
{
    [SerializeField] private RefInt priority;
    [SerializeField] private RefFloat cooldown;
    [SerializeField] private RefInt duration;

    public int Priority { 
        get { return priority.Value; }
    }

    public float Cooldown { 
        get { return cooldown.Value; }
    }

    public int Duration { 
        get { return duration.Value; }
    }
}
