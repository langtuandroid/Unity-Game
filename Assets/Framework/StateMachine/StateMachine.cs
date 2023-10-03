using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LobsterFramework.Utility;

namespace LobsterFramework.AI
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private AIController controller;
        [SerializeField] private List<State> allStates;
        [SerializeField] private State initialState;
        [SerializeField] private State currentState;

        private readonly Dictionary<Type, State> states = new();
        

        #region Coroutine
        public readonly CoroutineRunner coroutineRunner = new(); 
        private Type switchingTo = null;

        public Utility.Coroutine RunCoroutine(IEnumerator<CoroutineOption> coroutine) {
            return coroutineRunner.AddCoroutine(coroutine);
        }
        #endregion

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
                state.stateMachine = this;
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
            // Execute Coroutine
            if (coroutineRunner.Size > 0) {
                coroutineRunner.Run();
                if (switchingTo != null && coroutineRunner.Size == 0) {
                    currentState.OnExit();
                    currentState = states[switchingTo];
                    currentState.OnEnter();
                    switchingTo = null;
                }
                return;
            }

            // Normal state ticking behavior
            Type target = currentState.Tick();

            if (target != null)
            {
                if (coroutineRunner.Size == 0)
                {
                    currentState.OnExit();
                    currentState = states[target];
                    currentState.OnEnter();
                }
                else { // Coroutine is called by a state, postpone state switch until coroutine is finished
                    switchingTo = target;
                }
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
