using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Utility
{
    public class CoroutineRunner
    {
        private List<Coroutine> coroutines = new();

        public Coroutine AddCoroutine(IEnumerator<CoroutineOption> coroutine) {
            Coroutine corout = new(this, coroutine);
            coroutines.Add(corout);
            return corout;
        }

        public void Run() {
            for (int i = coroutines.Count - 1;i >= 0;i--) {
                if (!coroutines[i].Advance()) {
                    coroutines.RemoveAt(i);
                }
            }
        }

        public int Size { get { return coroutines.Count; } }
    }
}
