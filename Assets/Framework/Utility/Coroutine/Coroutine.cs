using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Utility
{
    /// <summary>
    /// Represents the state of Coroutine, can be used to query if the coroutine has finished.
    /// </summary>
    public class Coroutine
    {
        private CoroutineRunner runner;
        private IEnumerator<CoroutineOption> coroutine;
        private Coroutine waitFor; // The child coroutine to wait for
        private float awakeTime; // The time this coroutine should continue executing

        /// <summary>
        /// True if the coroutine is finished, false otherwise
        /// </summary>
        public bool IsFinished { get; private set; }

        public Coroutine(CoroutineRunner runner, IEnumerator<CoroutineOption> coroutine) { 
            this.coroutine = coroutine;
            IsFinished = false;
            this.runner = runner;
        }

        /// <summary>
        /// Execute Coroutine until it yields
        /// </summary>
        /// <returns> False if coroutine has finished executing, otherwise true </returns>
        internal bool Advance() {
            if (Time.time < awakeTime)
            {
                return true;
            }
            if (waitFor != null)
            {
                if (!waitFor.IsFinished)
                {
                    return true;
                }
                else { 
                    waitFor = null;
                }
            }

            bool next = coroutine.MoveNext();
            CoroutineOption option = coroutine.Current;
            if (!next)
            {
                IsFinished = true;
                return false;
            }
            if (option != null)
            {
                if (option.exit)
                {
                    IsFinished = true;
                    return false;
                }
                if (option.reset)
                {
                    coroutine.Reset();
                    return true;
                }
                if (option.waitTime > 0)
                {
                    awakeTime = Time.time + option.waitTime;
                }
                else if (option.waitFor != null) {
                    waitFor = runner.AddCoroutine(coroutine);
                }
            }
            return true;
        }
    }
}
