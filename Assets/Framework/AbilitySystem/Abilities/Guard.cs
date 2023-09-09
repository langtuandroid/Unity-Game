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
            [HideInInspector] public int m_key;
            [HideInInspector] public int r_key;
            [HideInInspector] public Weapon currentWeapon;
            [HideInInspector] public AnimancerState animancerState;
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

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            GuardConfig guardConfig = (GuardConfig)CurrentConfig;
            guardConfig.currentWeapon = weaponWielder.Mainhand;
            guardConfig.animationSignaled = false;

            guardConfig.m_key = moveControl.ModifyMoveSpeed(guardConfig.currentWeapon.GMoveSpeedModifier);
            guardConfig.r_key = moveControl.ModifyRotationSpeed(guardConfig.currentWeapon.GRotationSpeedModifier);
            guardConfig.animancerState = abilityRunner.StartAnimation(this, CurrentConfigName, weaponWielder.GetAbilityClip(GetType(), guardConfig.currentWeapon.WeaponType), guardConfig.currentWeapon.DefenseSpeed);
        }

        protected override void OnCoroutineFinish()
        {
            GuardConfig g = (GuardConfig)CurrentConfig;
            g.currentWeapon.Pause();
            moveControl.UnmodifyMoveSpeed(g.m_key);
            moveControl.UnmodifyRotationSpeed(g.r_key);
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
        {
            GuardConfig guardConfig = (GuardConfig)CurrentConfig; 

            while(!guardConfig.animationSignaled)
            {
                yield return null;
            }
            guardConfig.animancerState.IsPlaying = false;
            guardConfig.currentWeapon.Action(WeaponState.Guarding);

            // Wait for animation to finish
            while (true)
            {
                yield return null;
            }
        }

        protected override void Signal(AnimationEvent animationEvent)
        {
            GuardConfig guardConfig = (GuardConfig)CurrentConfig;
            if (animationEvent != null)
            {
                guardConfig.animationSignaled = true;
            }
            else
            {
                HaltAbilityExecution(CurrentConfigName);
            }
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }
    }
}
