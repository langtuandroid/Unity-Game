using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    public class TestCoroutine : AbilityCoroutine
    {
        public class TestCoroutineConfig : AbilityCoroutineConfig { }

        protected override IEnumerator Coroutine(AbilityConfig config)
        {
            Debug.Log("1: " + Time.time);
            yield return null;

            Debug.Log("2: " + Time.time);
            yield return null;

            Debug.Log("3: " + Time.time);
            yield return null;

            Debug.Log("4: " + Time.time);
            yield return null;

            Debug.Log("5: " + Time.time);
        }

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            Debug.Log("Coroutining!");
        }

        protected override void OnCoroutineFinish(AbilityConfig config)
        {
            Debug.Log("Coroutined!");
        }
    }
}
