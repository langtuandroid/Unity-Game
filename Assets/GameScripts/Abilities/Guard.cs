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
        private WeaponWielder weaponWielder;
        private bool animationInterrupted;

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

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Mainhand != null)
            {
                int animation = Animator.StringToHash(weaponWielder.Mainhand.Name + "_guard");
                int index = abilityRunner.Animator.GetLayerIndex("Base Layer");
                return abilityRunner.Animator.HasState(index, animation);
            }
            return false;
        }

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            animationInterrupted = false;
            GuardConfig guardConfig = (GuardConfig)config;
            abilityRunner.StartAnimation<Guard>(configName, weaponWielder.Mainhand.Name + "_guard", weaponWielder.Mainhand.AttackSpeed);
            guardConfig.animationSignaled = false;
            guardConfig.inputSignaled = false;
        }

        protected override void OnCoroutineFinish(AbilityConfig config)
        {
            if(!animationInterrupted) {
                weaponWielder.PlayDefaultWeaponAnimation();
            }
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
            weaponWielder.Mainhand.Action(WeaponState.Guarding);
            
            while(!guardConfig.inputSignaled)
            {
                yield return null;
            }
            guardConfig.inputSignaled = false;
            abilityRunner.Animator.speed = weaponWielder.Mainhand.AttackSpeed;
            weaponWielder.Mainhand.Pause();

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
            animationInterrupted = true;
            HaltAbilities();
        }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
        }
    }
}
