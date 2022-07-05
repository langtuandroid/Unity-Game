using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "StateMachine/GameState")]
public class State : DescriptionBaseSO
{
    private string stateName;
    private HashSet<State> nextStates;
    private HashSet<State> prevStates;
    private bool isActive;

    [SerializeField] private UnityEvent OnExit;
    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityAction StateAction;

    [Tooltip("The state machine used to process the state.")]
    [SerializeField] StateMachine stateMachine;

    public bool SetName(string name) {
        foreach (State s in stateMachine) {
            if (s.stateName == name) {
                return false;
            }
        }
        this.stateName = name;
        if (!isActive) {
            isActive = true;
            stateMachine.Add(this);
        }
        return true;
    }

    public string GetName() {
        return stateName;
    }

    public void Exit() {
        OnExit.Invoke();
    }

    public void Enter() { 
        OnEnter.Invoke();
    }

    public bool AddNextState(State state) { 
        return nextStates.Add(state);
    }

    public bool RemoveNextState(State state) { 
        return nextStates.Remove(state);
    }

    public bool AddPrevState(State state)
    {
        return prevStates.Add(state);
    }

    public bool RemovePrevState(State state)
    {
        return prevStates.Remove(state);
    }

    public bool Active {
        get { return isActive; }
        set { isActive = value; }
    }
}
