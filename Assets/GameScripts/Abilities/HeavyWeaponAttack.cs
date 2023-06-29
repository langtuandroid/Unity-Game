using UnityEngine;
using LobsterFramework.AbilitySystem;
using System.Collections;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(Animator))]
    public class HeavyWeaponAttack : AbilityCoroutine
    {
        protected override IEnumerator Coroutine(AbilityConfig config)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCoroutineFinish(AbilityConfig config)
        {
            throw new System.NotImplementedException();
        }

        public class HeavyWeaponAttackConfig : AbilityCoroutineConfig { }

        
    }
}
