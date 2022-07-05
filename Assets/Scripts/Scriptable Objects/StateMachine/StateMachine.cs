using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/StateMachine")]
public class StateMachine : DescriptionBaseSO, IEnumerable<State>
{
    [SerializeField] private readonly HashSet<State> State = new();
    [SerializeField] private State initialState;

    public bool Add(State state) { 
        return State.Add(state);
    }

    public bool Remove(State state)
    {
        return State.Remove(state);
    }

    public bool Contains(State state) { 
        return State.Contains(state);
    }

    public IEnumerator<State> GetEnumerator()
    {
        return State.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return State.GetEnumerator();
    }
}
