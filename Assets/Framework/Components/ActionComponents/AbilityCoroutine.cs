using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
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
            public IEnumerator Coroutine { get; set; }
            [HideInInspector]
            public bool CoroutineRunning { get; set; }
        }

        protected sealed override void OnEnqueue(AbilityConfig config, string configName)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)config;
            c.CoroutineRunning = true;
            c.Coroutine = Coroutine(config);
            OnCoroutineEnqueue(config, configName);
        }

        /// <summary>
        /// Callback when the ability is enqueued, replaces OnEnqueue
        /// </summary>
        /// <param name="config">The config being enqueued with</param>
        /// <param name="configName">The name of the config</param>
        protected abstract void OnCoroutineEnqueue(AbilityConfig config, string configName);

        protected override sealed bool Action(AbilityConfig config)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)config;
            return c.Coroutine.MoveNext();
        }

        protected sealed override void OnActionFinish(AbilityConfig config)
        {
            AbilityCoroutineConfig c = (AbilityCoroutineConfig)config;
            c.CoroutineRunning = false;
            OnCoroutineFinish(config);
        }
        /// <summary>
        /// Callback on ability finished, replaces OnActionFinish
        /// </summary>
        /// <param name="config"></param>
        protected abstract void OnCoroutineFinish(AbilityConfig config);

        /// <summary>
        /// The body of the ability execution, replaces Action method
        /// </summary>
        /// <param name="config">The ability configuration to execute on</param>
        /// <returns></returns>
        protected abstract IEnumerator Coroutine(AbilityConfig config);
    }
}
