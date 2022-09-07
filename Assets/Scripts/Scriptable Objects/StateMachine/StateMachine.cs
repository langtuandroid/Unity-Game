using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private List<StateSO> allStates;
    [SerializeField] private StateSO initialState;
    [SerializeField] private State currentState;
    private Dictionary<Type, State> states = new();

    public State CurrentState{
        get{ return currentState; }
        set { currentState = value; }
    }

    void Start() {
        ReferenceCheck();

        foreach (StateSO s in allStates) {
            State state = s.ResolveState();
            states[state.GetType()] = state;
            state.InitializeFields(gameObject);
        }
        if (initialState == null)
        {
            initialState = allStates[0];
        }
        currentState = states[initialState.GetStateType()];
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
