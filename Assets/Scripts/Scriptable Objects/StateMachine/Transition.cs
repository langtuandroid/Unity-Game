using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Transition")]
public class Transition : DescriptionBaseSO
{
    [SerializeField] private State targetState;
    [SerializeField] private HashSet<Condition<GameObject>> transitionConditions;

    public bool Execute(GameObject obj)
    {
        foreach (Condition<GameObject> condition in transitionConditions)
        {
            if (condition.Eval(obj))
            {
                return true;
            }
        }
        return false;
    }

    public State TargetState {
        get { return targetState; }
    } 
}
