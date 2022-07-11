using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTable : MonoBehaviour
{
    [SerializeField] private Dictionary<State, SortedList<int, Transition>> transitions;

    public State Execute(State currentState, GameObject obj) {
        if (transitions.ContainsKey(currentState)) {
            foreach (Transition t in transitions[currentState].Values) {
                if (t.Execute(obj))
                {
                    return t.TargetState;
                }
            }
        }
        return null;
    }

    public bool AddTransition(State fromState, Transition t, int priority)
    {
        if (!transitions.ContainsKey(fromState))
        {
            transitions[fromState] = new SortedList<int, Transition>();
        }
        else if (transitions[fromState].Values.Contains(t))
        {
            return false;
        }
        transitions[fromState].Add(priority, t);
        return true;
    }

    public bool RemoveTransition(State fromState, Transition t, int key)
    {
        if (transitions.ContainsKey(fromState))
        {
            return transitions[fromState].Remove(key);
        }
        return false;
    }
}
