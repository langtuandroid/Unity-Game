using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LobsterFramework.AI
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private AIController controller;
        [SerializeField] private List<State> allStates;
        [SerializeField] private State initialState;
        [SerializeField] private State currentState;
        
        private Dictionary<Type, State> states = new();

        public State CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        void Start()
        {
            ReferenceCheck();

            foreach (State s in allStates)
            {
                State state = Instantiate(s);
                states[state.GetType()] = state;
                state.controller = controller;
                state.InitializeFields(gameObject);
            }
            if (initialState == null)
            {
                initialState = allStates[0];
            }
            currentState = states[initialState.GetType()];
        }

        private void ReferenceCheck()
        {
            if (initialState == null)
            {
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

        public void OnDestroy()
        {
            foreach (State s in states.Values)
            {
                Destroy(s);
            }
        }
    }
}
