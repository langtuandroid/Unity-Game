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
        [SerializeField] internal XList<State> allStates;
        [DisableInPlayMode]
        [SerializeField] private State initialState;
        [ReadOnly]
        [SerializeField] private State currentState;
        [SerializeField] internal string statePath;    

        internal readonly Dictionary<Type, State> states = new();


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
                state.name = s.name;
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

        /// <summary>
        /// Reset the state by replacing it with a copy with parameters set to the its initial values at the start of play mode
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal State ResetState(int index){
            try { 
                State state = allStates[index];
                Type type = state.GetType();
                bool currentlyRunning = states[type] == currentState;

                DestroyImmediate(states[type]);
                State newState = Instantiate(state);
                newState.name = state.name;
                states[type] = newState;
                newState.controller = controller;
                newState.stateMachine = this;
                newState.InitializeFields(gameObject);
                if (currentlyRunning) {
                    newState.OnEnter();
                    currentState = newState;
                }
                return newState;
            } catch(Exception e) {
                Debug.LogException(e);
            }
            return null;
        }
    }
}
