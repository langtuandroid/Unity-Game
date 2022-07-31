using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "StateMachine/State")]
public abstract class State : DescriptionBaseSO
{
    [Tooltip("The state machine used to process the state.")]
    [SerializeField] StateMachine stateMachine;

    public abstract void Exit(GameObject obj);

    public abstract void Enter(GameObject obj);

    public abstract void Operate(GameObject obj);
}
