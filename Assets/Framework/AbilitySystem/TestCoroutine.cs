using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    public class TestCoroutine : AbilityCoroutine
    {
        public class TestCoroutineConfig : AbilityCoroutineConfig { }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
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

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            Debug.Log("Coroutining!");
        }

        protected override void OnCoroutineFinish()
        {
            Debug.Log("Coroutined!");
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }
    }
}
