using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AI
{
    [System.Serializable]
    public struct Transition
    {
        [SerializeField] private State targetState;
        [SerializeField] private List<Condition> transitionConditions;

        public State Eval()
        {
            foreach (Condition condition in transitionConditions)
            {
                if (condition.Eval())
                {
                    return targetState;
                }
            }
            return null;
        }

        public State TargetState
        {
            get { return targetState; }
        }
    }
}
