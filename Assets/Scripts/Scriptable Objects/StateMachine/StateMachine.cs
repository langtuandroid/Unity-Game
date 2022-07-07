using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/StateMachine")]
public class StateMachine : DescriptionBaseSO
{
    [SerializeField] private State initialState;
    [SerializeField] private State currentState;
    private Dictionary<State, SortedList<int, Transition>> transitions;

    public State CurrentState{
        get{ return currentState; }
        set { currentState = value; }
    }

    void Awake() {
        currentState = initialState;
    }

    public void Run()
    {
         currentState.Operate();
        if (transitions.ContainsKey(currentState)) {
            SortedList<int, Transition> list = transitions[currentState];
            foreach (Transition t in list.Values) {
                if (t.Execute(this)) {
                    return;
                }
            }
        }
    }

    public bool AddTransition(Transition t, int priority) {
        if (!transitions.ContainsKey(t.FromState))
        {
            transitions[t.FromState] = new SortedList<int, Transition>();
        }
        else if(transitions[t.FromState].Values.Contains(t)){
            return false;
        }
        transitions[t.FromState].Add(priority, t);
        return true;
    }

    public bool RemoveTransition(Transition t, int key) {
        if (transitions.ContainsKey(t.FromState)) {
            return transitions[t.FromState].Remove(key);
        }
        return false;
    }
}
