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

        private readonly Dictionary<Type, State> states = new();

        #region Coroutine
        [HideInInspector]
        public IEnumerator<CoroutineOption> Coroutine { get; set; }
        [HideInInspector]
        public float awakeTime = 0; // Coroutine will only execute if Time.time >= awakeTime

        private Type switchingTo = null;

        /// <summary>
        /// Set the coroutine to be executed starting next frame, normal ticking and state switching will not occur until the coroutine has finished executing
        /// </summary>
        /// <param name="coroutine">The coroutine to be executed</param>
        public void RunCoroutine(IEnumerator<CoroutineOption> coroutine) {
            Coroutine = coroutine;
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
            if (Coroutine != default) {
                if (Time.time < awakeTime) // CoroutineOption.Wait(x) is called by coroutine to halt execution until x seconds has passed
                {
                    return;
                }
                bool hasNext = Coroutine.MoveNext();
                if (hasNext)
                {
                    CoroutineOption option = Coroutine.Current;
                    if (option != null)
                    {
                        if (option.exit)
                        {
                            Coroutine = default;
                            if (switchingTo != null)
                            {
                                currentState.OnExit();
                                currentState = states[switchingTo];
                                currentState.OnEnter();
                                switchingTo = null;
                            }
                            return;
                        }
                        if (option.reset)
                        {
                            Coroutine.Reset();
                            return;
                        }
                        if (option.waitTime > 0)
                        {
                            awakeTime = Time.time + option.waitTime;
                        }
                        return;
                    }
                }
                // Return to normal state ticking when coroutine is finished
                Coroutine = default;
                if (switchingTo != null) {
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
                if (Coroutine == default)
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
