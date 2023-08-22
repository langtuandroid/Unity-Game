using UnityEngine;
using LobsterFramework.AbilitySystem;
using System.Collections.Generic;
using LobsterFramework;

namespace GameScripts.Abilities
{
    [ComponentRequired(typeof(WeaponWielder))]
    [AddAbilityMenu]
    public class Guard : AbilityCoroutine
    {
        private WeaponWielder weaponWielder;
        private bool animationInterrupted;
        private MovementController moveControl;

        public class GuardConfig : AbilityCoroutineConfig
        {
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
            [HideInInspector] public int m_key;
            [HideInInspector] public int r_key;

            protected override void Initialize()
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
                int animation = Animator.StringToHash(weaponWielder.Mainhand.Name + "_guard");
                int index = abilityRunner.Animator.GetLayerIndex("Base Layer");
                return abilityRunner.Animator.HasState(index, animation) && weaponWielder.Mainhand.state == WeaponState.Idle;
            }
            return false;
        }

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config)
        {
            animationInterrupted = false;
            GuardConfig guardConfig = (GuardConfig)config;
            abilityRunner.StartAnimation<Guard>(config.Key, weaponWielder.Mainhand.Name + "_guard", weaponWielder.Mainhand.DefenseSpeed);
            guardConfig.animationSignaled = false;
            guardConfig.inputSignaled = false;

            guardConfig.m_key = moveControl.ModifyMoveSpeed(weaponWielder.Mainhand.GMoveSpeedModifier);
            guardConfig.r_key = moveControl.ModifyRotationSpeed(weaponWielder.Mainhand.GRotationSpeedModifier);
        }

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config)
        {
            if(!animationInterrupted) {
                weaponWielder.PlayDefaultWeaponAnimation();
            }
            GuardConfig g = (GuardConfig)config;
            if (g.m_key != -1) { 
                moveControl.UnmodifyMoveSpeed(g.m_key);
            }
            if(g.r_key != -1)
            {
                moveControl.UnmodifyRotationSpeed(g.r_key);
            }
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config, AbilityPipe pipe)
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
            moveControl.UnmodifyMoveSpeed(guardConfig.m_key);
            moveControl.UnmodifyRotationSpeed(guardConfig.r_key);
            guardConfig.m_key = -1;
            guardConfig.r_key = -1;

            // Wait for animation to finish
            while (true)
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

        protected override void OnCoroutineReset(AbilityCoroutineConfig config)
        {
            throw new System.NotImplementedException();
        }
    }
}
