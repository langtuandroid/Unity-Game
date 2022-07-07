using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "StateMachine/State")]
public class State : DescriptionBaseSO
{
    [SerializeField] private UnityEvent OnExit;
    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent StateAction;

    [Tooltip("The state machine used to process the state.")]
    [SerializeField] StateMachine stateMachine;

    public void Exit() {
        OnExit.Invoke();
    }

    public void Enter() { 
        OnEnter.Invoke();
    }

    public void Operate() { 
        StateAction.Invoke();
    }
}
