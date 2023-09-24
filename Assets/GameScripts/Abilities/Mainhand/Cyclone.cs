using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.AbilitySystem;
using LobsterFramework;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;


namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(WeaponWielder), typeof(MovementController))]
    [AddWeaponArtMenu(false, WeaponType.Stick)]
    public class Cyclone : WeaponAbility
    {
        [SerializeField] TargetSetting targets;
        private MovementController moveControl;

        public class CycloneConfig : AbilityCoroutineConfig {
            
            [Range(0, 1)] public float moveSpeedModifier;
            [Range(0, 1)] public float rotationSpeedModifier;

            [HideInInspector] public Weapon currentWeapon;
            [HideInInspector] public bool stopped;
            [HideInInspector] public bool repeatAttack;
            [HideInInspector] public int m_key;
            [HideInInspector] public int r_key;
        }

        public class CyclonePipe : AbilityPipe {  }

        protected override bool WConditionSatisfied(AbilityConfig config)
        {
            return WeaponWielder.GetAbilityClip(GetType(), WeaponWielder.Mainhand.WeaponType) != null && WeaponWielder.Mainhand.state != WeaponState.Attacking;
        }

        protected override void Init()
        {
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        { 
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            cycloneConfig.currentWeapon = WeaponWielder.Mainhand;
            SubscribeWeaponEvent(cycloneConfig.currentWeapon);
            cycloneConfig.m_key = moveControl.ModifyMoveSpeed(cycloneConfig.moveSpeedModifier);
            cycloneConfig.r_key = moveControl.ModifyRotationSpeed(cycloneConfig.rotationSpeedModifier);
            abilityRunner.StartAnimation(this, CurrentConfigName, WeaponWielder.GetAbilityClip(GetType(), cycloneConfig.currentWeapon.WeaponType), WeaponWielder.Mainhand.AttackSpeed);
        }

        protected override IEnumerator<CoroutineOption> Coroutine( AbilityPipe pipe)
        {
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            cycloneConfig.stopped = false;
            cycloneConfig.repeatAttack = false;
            cycloneConfig.currentWeapon.Enable();
            while (!cycloneConfig.stopped) {
                if (cycloneConfig.repeatAttack) {
                    cycloneConfig.currentWeapon.Pause();
                    cycloneConfig.currentWeapon.Enable();
                    cycloneConfig.repeatAttack = false;
                }
                yield return null;
            }
            cycloneConfig.currentWeapon.Disable();
            while (true) {
                yield return null;
            }
        }

        protected override void OnCoroutineFinish()
        {
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            UnSubscribeWeaponEvent(cycloneConfig.currentWeapon);
            cycloneConfig.currentWeapon.Disable();
            moveControl.UnmodifyMoveSpeed(cycloneConfig.m_key);
            moveControl.UnmodifyRotationSpeed(cycloneConfig.r_key);
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }

        protected override void Signal(AnimationEvent animationEvent)
        {
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            if (animationEvent.intParameter != 0)
            {
                cycloneConfig.stopped = true;
            }
            else {
                cycloneConfig.repeatAttack = true;
            }
        }
        private void SubscribeWeaponEvent(Weapon weapon)
        {
            weapon.onEntityHit += OnEntityHit;
            weapon.onWeaponHit += OnWeaponHit;
        }

        private void UnSubscribeWeaponEvent(Weapon weapon)
        {
            weapon.onEntityHit -= OnEntityHit;
            weapon.onWeaponHit -= OnWeaponHit;
        }

        private void OnEntityHit(Entity entity)
        {
            CycloneConfig config = (CycloneConfig)CurrentConfig;
            if (targets.IsTarget(entity))
            {
                config.currentWeapon.SetOnHitDamage(WeaponUtility.ComputeDamage(config.currentWeapon));
            }
            else {
                config.currentWeapon.SetOnHitDamage(Damage.none);
            }
        }

        private void OnWeaponHit(Weapon weapon)
        {
            Entity entity = weapon.Entity;
            CycloneConfig config = (CycloneConfig)CurrentConfig;
            if (targets.IsTarget(entity))
            {
                config.currentWeapon.SetOnHitDamage(WeaponUtility.ComputeDamage(config.currentWeapon));
            }
            else
            {
                config.currentWeapon.SetOnHitDamage(Damage.none);
            }
        }
    }
}
