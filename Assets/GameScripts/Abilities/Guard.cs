using UnityEngine;
using LobsterFramework.Pool;
using LobsterFramework.EntitySystem;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Utility;
using System.Collections;
using Codice.Client.BaseCommands.CheckIn.Progress;
using System;

namespace GameScripts.Abilities
{
    [ComponentRequired(typeof(WeaponWielder))]
    [AddAbilityMenu]
    public class Guard : AbilityCoroutine
    {
        [SerializeField] private string animation;
        private WeaponWielder weaponWielder;

        public class GuardConfig : AbilityCoroutineConfig
        {
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;

            protected override void Initialize()
            {
                animationSignaled = false;
                inputSignaled = false;
            }
        }

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            GuardConfig guardConfig = (GuardConfig)config;
            abilityRunner.StartAnimation<Guard>(configName, animation, weaponWielder.Weapon.AttackSpeed);
            guardConfig.animationSignaled = false;
            guardConfig.inputSignaled = false;
        }

        protected override void OnCoroutineFinish(AbilityConfig config)
        {
            
        }

        protected override IEnumerator Coroutine(AbilityConfig config)
        {
            GuardConfig guardConfig = (GuardConfig)config; 

            while(!guardConfig.animationSignaled)
            {
                yield return null;
            }
            guardConfig.animationSignaled = false;
            abilityRunner.Animator.speed = 0;
            weaponWielder.Weapon.Activate(WeaponState.Guarding);
            
            while(!guardConfig.inputSignaled)
            {
                yield return null;
            }
            guardConfig.inputSignaled = false;
            abilityRunner.Animator.speed = weaponWielder.Weapon.AttackSpeed;
            weaponWielder.Weapon.Deactivate();

            while (!guardConfig.animationSignaled)
            {
                yield return null;
            }
        }

        protected override void Signal(AbilityConfig config, bool isAnimation)
        {
            GuardConfig guardConfig = (GuardConfig)config;
            if (isAnimation)
            {
                guardConfig.animationSignaled = true;
            }
            else { 
                guardConfig.inputSignaled = true;
            }
        }

        protected override void OnAnimationInterrupt(AbilityConfig config)
        {
            HaltAbilities();
        }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
        }
    }
}
