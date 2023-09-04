using UnityEngine;
using System.Collections.Generic;
using static LobsterFramework.AbilitySystem.Guard;
using Animancer;

namespace LobsterFramework.AbilitySystem
{
    [ComponentRequired(typeof(WeaponWielder))]
    [AddAbilityMenu]
    public class Guard : AbilityCoroutine
    {
        private WeaponWielder weaponWielder;
        private MovementController moveControl;

        public class GuardConfig : AbilityCoroutineConfig
        {
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
            [HideInInspector] public int m_key;
            [HideInInspector] public int r_key;
            [HideInInspector] public Weapon currentWeapon;
            [HideInInspector] public AnimancerState animancerState;

            protected internal override void Initialize()
            {
                animationSignaled = false;
                inputSignaled = false;
            }
        }
        public class GuardPipe : AbilityPipe{ }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            moveControl = weaponWielder.Wielder.GetComponent<MovementController>();
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Mainhand != null)
            {
                return weaponWielder.GetAbilityClip(GetType(), weaponWielder.Mainhand.WeaponType) != null && weaponWielder.Mainhand.state == WeaponState.Idle;
            }
            return false;
        }

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config, AbilityPipe pipe)
        {
            GuardConfig guardConfig = (GuardConfig)config;
            guardConfig.currentWeapon = weaponWielder.Mainhand;
            guardConfig.animationSignaled = false;
            guardConfig.inputSignaled = false;

            guardConfig.m_key = moveControl.ModifyMoveSpeed(guardConfig.currentWeapon.GMoveSpeedModifier);
            guardConfig.r_key = moveControl.ModifyRotationSpeed(guardConfig.currentWeapon.GRotationSpeedModifier);
            guardConfig.animancerState = abilityRunner.StartAnimation(this, CurrentConfigName, weaponWielder.GetAbilityClip(GetType(), guardConfig.currentWeapon.WeaponType), guardConfig.currentWeapon.DefenseSpeed);
        }

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config)
        {
            GuardConfig g = (GuardConfig)config;
            g.currentWeapon.Pause();
            moveControl.UnmodifyMoveSpeed(g.m_key);
            moveControl.UnmodifyRotationSpeed(g.r_key);
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config, AbilityPipe pipe)
        {
            GuardConfig guardConfig = (GuardConfig)config; 

            while(!guardConfig.animationSignaled)
            {
                yield return null;
            }
            guardConfig.animationSignaled = false;
            guardConfig.animancerState.IsPlaying = false;
            guardConfig.currentWeapon.Action(WeaponState.Guarding);
            
            while(!guardConfig.inputSignaled)
            {
                yield return null;
            }
            guardConfig.inputSignaled = false;
            guardConfig.animancerState.IsPlaying = true;
            guardConfig.currentWeapon.Pause();

            // Wait for animation to finish
            while (true)
            {
                yield return null;
            }
        }

        protected override void Signal(AbilityConfig config, AnimationEvent animationEvent)
        {
            GuardConfig guardConfig = (GuardConfig)config;
            if (animationEvent != null)
            {
                guardConfig.animationSignaled = true;
            }
            else
            {
                guardConfig.inputSignaled = true;
            }
        }

        protected override void OnCoroutineReset(AbilityCoroutineConfig config)
        {
            throw new System.NotImplementedException();
        }
    }
}
