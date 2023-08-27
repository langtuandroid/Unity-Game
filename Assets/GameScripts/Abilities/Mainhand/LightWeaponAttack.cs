using LobsterFramework;
using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using LobsterFramework.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.Abilities
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
            if(weaponWielder.Mainhand != null)
            {
                int animation = Animator.StringToHash(weaponWielder.Mainhand.Name + "_light_attack");
                int index = abilityRunner.Animator.GetLayerIndex("Base Layer");
                return abilityRunner.Animator.HasState(index, animation) && weaponWielder.Mainhand.state != WeaponState.Attacking;
            }
            return false;
        }

        private void SubscribeWeaponEvent() {
            weaponWielder.Mainhand.Action();
            weaponWielder.Mainhand.onEntityHit += OnEntityHit;
            weaponWielder.Mainhand.onWeaponHit += OnWeaponHit;
        }

        private void UnSubscribeWeaponEvent() {
            weaponWielder.Mainhand.Pause();
            weaponWielder.Mainhand.onEntityHit -= OnEntityHit;
            weaponWielder.Mainhand.onWeaponHit -= OnWeaponHit;
        }

        private void OnEntityHit(Entity entity)
        {
            DealDamage(entity);
        }

        private void OnWeaponHit(Weapon weapon, Vector3 contactPoint)
        {
            if (clashSparkTag != null) {
                ObjectPool.Instance.GetObject(clashSparkTag.Value, contactPoint, Quaternion.identity);
            }
            DealDamage(weapon.Entity, weapon.HealthDamageReduction, weapon.PostureDamageReduction);
        }

        private void DealDamage(Entity entity, float hdReduction=0, float pdReduction=0) {
            if (targets.IsTarget(entity)) {
                float health = 0.7f * weaponWielder.Mainhand.Sharpness + 0.3f * weaponWielder.Mainhand.Weight;
                float posture = 0.3f * weaponWielder.Mainhand.Sharpness + 0.7f * weaponWielder.Mainhand.Weight;
                health *= (1 - hdReduction);
                posture *= (1 - pdReduction);
                Damage damage = new() { health = health, posture = posture, source = attacker };
                entity.Damage(damageModifier.ModifyDamage(damage));
                MovementController moveControl = entity.GetComponent<MovementController>();
                if(moveControl != null)
                {
                    moveControl.ApplyForce(entity.transform.position - abilityRunner.transform.position, weaponWielder.Mainhand.Weight);
                }
            }
        }

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            c.signaled = false;
            abilityRunner.StartAnimation<LightWeaponAttack>(config.Key, weaponWielder.Mainhand.Name + "_light_attack", weaponWielder.Mainhand.AttackSpeed);
            c.m_key = moveControl.ModifyMoveSpeed(weaponWielder.Mainhand.LMoveSpeedModifier);
            c.r_key = moveControl.ModifyRotationSpeed(weaponWielder.Mainhand.LRotationSpeedModifier);
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config, AbilityPipe pipe)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            // Wait for signal to attack
            while (!c.signaled)
            {
                yield return null;
            }
            c.signaled = false;
            SubscribeWeaponEvent();

            // Wait for signal of recovery
            while (!c.signaled)
            {
                yield return null;
            }
            c.signaled = false;
            UnSubscribeWeaponEvent();

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

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config){
            weaponWielder.Mainhand.Pause();
            LightWeaponAttackConfig l = (LightWeaponAttackConfig)config;
            if(l.m_key != -1)
            {
                moveControl.UnmodifyMoveSpeed(l.m_key);
            }
            if(l.r_key != -1) {
                moveControl.UnmodifyRotationSpeed(l.r_key);
            }
        }

        protected override void OnAnimationInterrupt(AbilityConfig config)
        {
            HaltAbilities();
        }

        protected override void OnCoroutineReset(AbilityCoroutineConfig config)
        {
            throw new System.NotImplementedException();
        }

        protected override void Signal(AbilityConfig config, bool isAnimation)
        {
            if (isAnimation)
            {
                LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
                c.signaled = true;
            }
        }
    }
}
