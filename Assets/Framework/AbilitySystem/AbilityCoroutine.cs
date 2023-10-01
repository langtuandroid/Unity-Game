using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// Abilities that requires running across multiple frames can use this as a replacement for Unity Coroutines
    /// </summary>
    public abstract class AbilityCoroutine : Ability
    {
        /// <summary>
        /// AbilityConfig of coroutines, subclass configs need to inherit from this to properly work
        /// </summary>
        public class AbilityCoroutineConfig : AbilityConfig
        {
            [HideInInspector]
            public IEnumerator<CoroutineOption> Coroutine { get; set; }
            [HideInInspector]
            public bool CoroutineRunning { get; set; }
            [HideInInspector]
            public float awakeTime = 0;
        }

        protected sealed override void OnEnqueue(AbilityPipe pipe)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)CurrentConfig;
            c.CoroutineRunning = true;
            c.Coroutine = Coroutine(pipe);
            OnCoroutineEnqueue(pipe);
        }

        /// <summary>
        /// Callback when the ability is enqueued, replaces OnEnqueue
        /// </summary>
        /// <param name="config">The config being enqueued with</param>
        protected abstract void OnCoroutineEnqueue(AbilityPipe pipe);

        protected override sealed bool Action(AbilityPipe pipe)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)CurrentConfig;
            if (Time.time < c.awakeTime) {
                return true;
            }

            bool next = c.Coroutine.MoveNext();
            CoroutineOption option = c.Coroutine.Current;
            if (!next)
            {
                return false;
            }
            if (option != null)
            {
                if (option.exit) {
                    return false;
                }
                if (option.reset) {
                    c.Coroutine.Reset();
                    OnCoroutineReset();
                    return true;
                }
                if (option.waitTime > 0) {
                    c.awakeTime = Time.time + option.waitTime;
                }
            }
            return true;
        }

        protected sealed override void OnActionFinish()
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)CurrentConfig;
            c.CoroutineRunning = false;
            OnCoroutineFinish();
        }
        /// <summary>
        /// Callback on ability finished, replaces OnActionFinish
        /// </summary>
        /// <param name="config"></param>
        protected abstract void OnCoroutineFinish();

        protected abstract void OnCoroutineReset();

        /// <summary>
        /// The body of the ability execution, replaces Action method
        /// </summary>
        /// <param name="config">The ability configuration to execute on</param>
        /// <returns></returns>
        protected abstract IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe);
    }
}
