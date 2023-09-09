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
    public class Cyclone : AbilityCoroutine
    {
        [SerializeField] TargetSetting targets;
        [SerializeField] private VarString clashSparkTag;
        private WeaponWielder weaponWielder;
        private MovementController moveControl;
        private Entity attacker;

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

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Mainhand != null)
            {
                return weaponWielder.GetAbilityClip(GetType(), weaponWielder.Mainhand.WeaponType) != null && weaponWielder.Mainhand.state != WeaponState.Attacking;
            }
            return false; 
        }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponentInBoth<WeaponWielder>();
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
            attacker = abilityRunner.GetComponentInBoth<Entity>();
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            cycloneConfig.currentWeapon = weaponWielder.Mainhand;
            SubscribeWeaponEvent(cycloneConfig.currentWeapon);
            cycloneConfig.m_key = moveControl.ModifyMoveSpeed(cycloneConfig.moveSpeedModifier);
            cycloneConfig.r_key = moveControl.ModifyRotationSpeed(cycloneConfig.rotationSpeedModifier);
            abilityRunner.StartAnimation(this, CurrentConfigName, weaponWielder.GetAbilityClip(GetType(), cycloneConfig.currentWeapon.WeaponType), weaponWielder.Mainhand.AttackSpeed);
        }

        protected override IEnumerator<CoroutineOption> Coroutine( AbilityPipe pipe)
        {
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            cycloneConfig.stopped = false;
            cycloneConfig.repeatAttack = false;
            cycloneConfig.currentWeapon.Action();
            while (!cycloneConfig.stopped) {
                if (cycloneConfig.repeatAttack) {
                    cycloneConfig.currentWeapon.Pause();
                    cycloneConfig.currentWeapon.Action();
                    cycloneConfig.repeatAttack = false;
                }
                yield return null;
            }
            cycloneConfig.currentWeapon.Pause();
            while (true) {
                yield return null;
            }
        }

        protected override void OnCoroutineFinish()
        {
            CycloneConfig cycloneConfig = (CycloneConfig)CurrentConfig;
            UnSubscribeWeaponEvent(cycloneConfig.currentWeapon);
            cycloneConfig.currentWeapon.Pause();
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
            DealDamage(entity);
        }

        private void OnWeaponHit(Weapon weapon, Vector3 contactPoint)
        {
            if (clashSparkTag != null)
            {
                ObjectPool.Instance.GetObject(clashSparkTag.Value, contactPoint, Quaternion.identity);
            }
            DealDamage(weapon.Entity, weapon.HealthDamageReduction, weapon.PostureDamageReduction);
        }

        private void DealDamage(Entity entity, float hdReduction = 0, float pdReduction = 0)
        {
            if (targets.IsTarget(entity))
            {
                float health = 0.7f * weaponWielder.Mainhand.Sharpness + 0.3f * weaponWielder.Mainhand.Weight;
                float posture = 0.3f * weaponWielder.Mainhand.Sharpness + 0.7f * weaponWielder.Mainhand.Weight;
                health *= (1 - hdReduction);
                posture *= (1 - pdReduction);
                Damage damage = new() { health = health, posture = posture, source = attacker };
                entity.Damage(damage);
                MovementController moveControl = entity.GetComponent<MovementController>();
                if (moveControl != null)
                {
                    moveControl.ApplyForceQueued(entity.transform.position - abilityRunner.transform.position, weaponWielder.Mainhand.Weight);
                }
            }
        }
    }
}
