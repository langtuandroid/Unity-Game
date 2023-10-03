using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Utility
{
    /// <summary>
    /// Manages coroutine running operations
    /// </summary>
    public class CoroutineRunner
    {
        private List<Coroutine> coroutines = new();
        private List<Coroutine> removed = new();


        /// <summary>
        /// Add a coroutine to the running queue
        /// </summary>
        /// <param name="coroutine">The coroutine to be runned</param>
        /// <returns> The Coroutine Object that represents the state of coroutine </returns>
        public Coroutine AddCoroutine(IEnumerator<CoroutineOption> coroutine) {
            Coroutine corout = new(this, coroutine);
            coroutines.Add(corout);
            return corout;
        }

        /// <summary>
        /// Run through coroutines added to the running queue in a FIFO order. Child coroutines can be added and runned dynamically.
        /// </summary>
        public void Run() {
            int i = 0;
            while (i < coroutines.Count) {
                if (!coroutines[i].Advance()) {
                    removed.Add(coroutines[i]);
                }
                i++;
            }
            foreach (Coroutine c in removed) {
                coroutines.Remove(c);
            }
        }

        /// <summary>
        /// The number of Coroutines currently active
        /// </summary>
        public int Size { get { return coroutines.Count; } }
    }
}
