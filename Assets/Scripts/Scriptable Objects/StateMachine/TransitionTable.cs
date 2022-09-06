using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "StateMachine/Transition Table")]
public class TransitionTable : ScriptableObject
{
    private Dictionary<Type, SortedList<int, Transition>> transitions;

    public StateSO Execute(StateSO currentState) {
        Type type = currentState.GetType();
        if (transitions.ContainsKey(type)) {
            foreach (Transition t in transitions[type].Values) {
                return t.Eval();
            }
        }
        return null;
    }

    public bool AddTransition(StateSO fromState, Transition t, int priority)
    {
        Type type = fromState.GetType();
        if (!transitions.ContainsKey(type))
        {
            transitions[type] = new SortedList<int, Transition>();
        }
        else if (transitions[type].Values.Contains(t))
        {
            return false;
        }
        transitions[type].Add(priority, t);
        return true;
    }

    public bool RemoveTransition(StateSO fromState, int key)
    {
        Type fromType = fromState.GetType();
        if (transitions.ContainsKey(fromType))
        {
            return transitions[fromType].Remove(key);
        }
        return false;
    }
}
