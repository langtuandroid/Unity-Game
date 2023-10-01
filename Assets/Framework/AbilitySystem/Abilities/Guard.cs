using UnityEngine;
using System.Collections.Generic;
using Animancer;

namespace LobsterFramework.AbilitySystem
{
    [ComponentRequired(typeof(WeaponWielder))]
    [AddAbilityMenu]
    public class Guard : WeaponAbility
    {
        private MovementController moveControl;

        public class GuardConfig : AbilityCoroutineConfig
        {
            // Properties
            public float delfectTime;

            // Hidden Runtime info
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public int m_key;
            [HideInInspector] public int r_key;
            [HideInInspector] public Weapon currentWeapon;
            [HideInInspector] public AnimancerState animancerState;
            [HideInInspector] public bool deflected;
            [HideInInspector] public float deflectOver;
        }
        public class GuardPipe : AbilityPipe{ }

        protected override void Init()
        {
            moveControl = WeaponWielder.Wielder.GetComponent<MovementController>();
        }

        protected override bool WConditionSatisfied(AbilityConfig config)
        {
            return WeaponWielder.GetAbilityClip(GetType(), WeaponWielder.Mainhand.WeaponType) != null && WeaponWielder.Mainhand.state == WeaponState.Idle;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            GuardConfig guardConfig = (GuardConfig)CurrentConfig;
            guardConfig.currentWeapon = WeaponWielder.Mainhand;
            guardConfig.animationSignaled = false;
            guardConfig.deflected = false;

            guardConfig.m_key = moveControl.ModifyMoveSpeed(guardConfig.currentWeapon.GMoveSpeedModifier);
            guardConfig.r_key = moveControl.ModifyRotationSpeed(guardConfig.currentWeapon.GRotationSpeedModifier);
            guardConfig.animancerState = abilityRunner.StartAnimation(this, CurrentConfigName, WeaponWielder.GetAbilityClip(GetType(), guardConfig.currentWeapon.WeaponType), guardConfig.currentWeapon.DefenseSpeed);

            guardConfig.currentWeapon.onWeaponDeflect += OnDeflect;
        }

        protected override void OnCoroutineFinish()
        {
            GuardConfig g = (GuardConfig)CurrentConfig;
            g.currentWeapon.Disable();
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
            guardConfig.animationSignaled = false;
            guardConfig.animancerState.IsPlaying = false;
            guardConfig.currentWeapon.Enable(WeaponState.Deflecting);
            guardConfig.deflectOver = guardConfig.delfectTime + Time.time;

            float currentClipTime = guardConfig.animancerState.Time;

            // Wait for deflect, if deflect period has passed then wait for Guard cancel
            while (!guardConfig.deflected)
            {
                if (guardConfig.currentWeapon.state == WeaponState.Deflecting && Time.time >= guardConfig.deflectOver)
                {
                    guardConfig.currentWeapon.state = WeaponState.Guarding;
                }
                yield return null;
            }

            // Deflect
            guardConfig.animancerState.IsPlaying = true;
            // Wait for deflect animation end
            while (!guardConfig.animationSignaled)
            {
                yield return null;
            }
            guardConfig.currentWeapon.state = WeaponState.Guarding;
            guardConfig.animancerState.IsPlaying = false;
            guardConfig.animancerState.Time = currentClipTime;
            guardConfig.animationSignaled = false;

            // Wait for guard cancel
            while (true) {
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

        private void OnDeflect() {
            GuardConfig guardConfig = (GuardConfig)CurrentConfig;
            guardConfig.deflected = true;
        }
    }
}
