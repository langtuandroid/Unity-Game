using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    [ComponentRequired(typeof(WeaponWielder))]
    public class LightWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private TargetSetting targets;
        [SerializeField] private VarString clashSparkTag;
        private WeaponWielder weaponWielder;
        private Entity attacker;
        private MovementController moveControl;
        private DamageModifier damageModifier;

        public class LightWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector]
            public bool signaled;
            [HideInInspector]
            public int m_key;
            [HideInInspector]
            public int r_key;
            [HideInInspector]
            public Weapon currentWeapon;
        }

        public class LightWeaponAttackPipe : AbilityPipe { }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            attacker = weaponWielder.Wielder;
            moveControl = attacker.GetComponent<MovementController>();
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Mainhand != null)
            {
                return weaponWielder.GetAbilityClip(GetType(), weaponWielder.Mainhand.WeaponType) != null && weaponWielder.Mainhand.state != WeaponState.Attacking;
            }
            return false;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)CurrentConfig;
            c.currentWeapon = weaponWielder.Mainhand;
            SubscribeWeaponEvent(c.currentWeapon);
            c.signaled = false;
            c.m_key = moveControl.ModifyMoveSpeed(c.currentWeapon.LMoveSpeedModifier);
            c.r_key = moveControl.ModifyRotationSpeed(c.currentWeapon.LRotationSpeedModifier);
            abilityRunner.StartAnimation(this, CurrentConfigName, weaponWielder.GetAbilityClip(GetType(), weaponWielder.Mainhand.WeaponType), c.currentWeapon.AttackSpeed);
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)CurrentConfig;
            // Wait for signal to attack
            while (!c.signaled)
            {
                yield return null;
            }
            c.signaled = false;

            c.currentWeapon.Action();
            // Wait for signal of recovery
            while (!c.signaled)
            {
                yield return null;
            }
            c.signaled = false;
            c.currentWeapon.Pause();

            moveControl.UnmodifyMoveSpeed(c.m_key);
            moveControl.UnmodifyRotationSpeed(c.r_key);
            c.m_key = -1;
            c.r_key = -1;

            // Wait for animation to finish
            while (true)
            {
                yield return null;
            }
        }

        protected override void OnCoroutineFinish(){
            LightWeaponAttackConfig l = (LightWeaponAttackConfig)CurrentConfig;
            UnSubscribeWeaponEvent(l.currentWeapon);
            l.currentWeapon.Pause();
            if (l.m_key != -1)
            {
                moveControl.UnmodifyMoveSpeed(l.m_key);
            }
            if(l.r_key != -1) {
                moveControl.UnmodifyRotationSpeed(l.r_key);
            }
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }

        protected override void Signal(AnimationEvent animationEvent)
        {
            if (animationEvent != null)
            {
                LightWeaponAttackConfig c = (LightWeaponAttackConfig)CurrentConfig;
                c.signaled = true;
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
                entity.Damage(damageModifier.ModifyDamage(damage));
                MovementController moveControl = entity.GetComponent<MovementController>();
                if (moveControl != null)
                {
                    moveControl.ApplyForceQueued(entity.transform.position - abilityRunner.transform.position, weaponWielder.Mainhand.Weight);
                }
            }
        }
    }
}
