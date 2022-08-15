using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Action/ActionDataSO")]

public class ActionDataSO : ScriptableObject {
    [SerializeField] private ActionData data;

    public ActionData Data
    {
        get { return data; }
    }

    public int Priority
    {
        get { return data.Priority; }
    }

    public float Cooldown
    {
        get { return data.Cooldown; }
    }

    public int Duration
    {
        get { return data.Duration; }
    }
}

[System.Serializable]
public struct ActionData
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
