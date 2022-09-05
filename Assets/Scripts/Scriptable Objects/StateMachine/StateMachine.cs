using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private List<State> allStates;
    [SerializeField] private State initialState;
    [SerializeField] private State currentState;
    private Dictionary<Type, State> states = new();

    public State CurrentState{
        get{ return currentState; }
        set { currentState = value; }
    }

    void Awake() {
        ReferenceCheck();
        if (initialState == null) {
            initialState = allStates[0];
        }
        currentState = initialState;

        foreach (State s in allStates) {
            states[s.GetType()] = s;
            s.InitializeFields(gameObject);
        }
    }

    private void ReferenceCheck() {
        if (initialState == null) {
            Debug.LogWarning("Initial state of the state machine is not set!");
        }
    }

    public void Update()
    {
        Type target = currentState.Tick();
        if (target != null)
        {
            currentState.OnExit();
            currentState = states[target];
            currentState.OnEnter();
        }
    }
}
