using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State initialState;
    [SerializeField] private State currentState;
    [SerializeField] private TransitionTable transitionTable;

    public State CurrentState{
        get{ return currentState; }
        set { currentState = value; }
    }

    void Awake() {
        ReferenceCheck();
        currentState = initialState;
    }

    private void ReferenceCheck() {
        if (initialState == null) {
            Debug.LogWarning("Initial state of the state machine is not set!");
        }
    }

    public void Update()
    {
        currentState.Operate(gameObject);
        State target = transitionTable.Execute(currentState, gameObject);
        if (target != null)
        {
            currentState.Exit(gameObject);
            currentState = target;
            currentState.Enter(gameObject);
        }
    }
}
