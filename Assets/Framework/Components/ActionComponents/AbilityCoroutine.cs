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
            public string Key { get; set; }

            [HideInInspector]
            public float awakeTime = 0;
        }

        protected sealed override void OnEnqueue(AbilityConfig config, AbilityPipe pipe, string configName)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)config;
            c.Key = configName;
            c.CoroutineRunning = true;
            c.Coroutine = Coroutine(c, pipe);
            OnCoroutineEnqueue(c);
        }

        /// <summary>
        /// Callback when the ability is enqueued, replaces OnEnqueue
        /// </summary>
        /// <param name="config">The config being enqueued with</param>
        protected abstract void OnCoroutineEnqueue(AbilityCoroutineConfig config);

        protected override sealed bool Action(AbilityConfig config, AbilityPipe pipe)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)config;
            if (c.awakeTime > Time.time) {
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
                    OnCoroutineReset(c);
                    return true;
                }
                if (option.waitTime > 0) {
                    c.awakeTime = Time.time + option.waitTime;
                }
            }
            return true;
        }

        protected sealed override void OnActionFinish(AbilityConfig config)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)config;
            c.CoroutineRunning = false;
            OnCoroutineFinish(c);
        }
        /// <summary>
        /// Callback on ability finished, replaces OnActionFinish
        /// </summary>
        /// <param name="config"></param>
        protected abstract void OnCoroutineFinish(AbilityCoroutineConfig config);

        protected abstract void OnCoroutineReset(AbilityCoroutineConfig config);

        /// <summary>
        /// The body of the ability execution, replaces Action method
        /// </summary>
        /// <param name="config">The ability configuration to execute on</param>
        /// <returns></returns>
        protected abstract IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config, AbilityPipe pipe);
    }

    public class CoroutineOption {
        public bool exit;
        public bool reset;
        public float waitTime;

        public static CoroutineOption Exit = new CoroutineOption(0, true);
        public static CoroutineOption Reset = new CoroutineOption(0, false, true);

        public static CoroutineOption Wait(float time) { 
            return new CoroutineOption(time);
        }

        public CoroutineOption(float time, bool exit = false, bool reset = false) { 
            waitTime = time;
            this.reset = reset;
            this.exit = exit;
        }
    }
}
