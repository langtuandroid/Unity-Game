using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Transition")]
public class Transition : DescriptionBaseSO
{
    [SerializeField] private State fromState;
    [SerializeField] private State toState;
    [SerializeField] private Condition transitionCondition;

    public bool Execute(StateMachine machine) {
        if (!fromState.Equals(machine.CurrentState)) {
            return false;
        }
        if (transitionCondition.Eval()) {
            fromState.Exit();
            toState.Enter();
            machine.CurrentState = toState;
            return true;
        }
        return false;
    }

    public State FromState {
        get { return fromState; }
    }

    public State ToState {
        get { return toState; }
    } 
}
