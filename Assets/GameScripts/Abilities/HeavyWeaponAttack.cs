using UnityEngine;
using LobsterFramework.AbilitySystem;
using System.Collections;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using System.Collections.Generic;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(WeaponWielder))]
    public class HeavyWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private TargetSetting targets;

        private Animator animator;
        private WeaponWielder weaponWielder;
        private Entity attacker;
        public class HeavyWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
            public RefFloat baseDamageModifier;
            public RefFloat maxChargeDamageIncrease;
            public RefFloat chargeMaxTime;

            [HideInInspector]
            public float chargeTimer;
            [HideInInspector]
            public HeavyWeaponAttack ability;

            public void DealDamage(Entity entity) {
                if (chargeTimer > chargeMaxTime) { 
                    chargeTimer = chargeMaxTime;
                }
                ability.DealDamage(entity, baseDamageModifier + maxChargeDamageIncrease *  (chargeTimer / chargeMaxTime));
            }

            public void DealGuardedDamage(Weapon weapon) {
                if (chargeTimer > chargeMaxTime)
                {
                    chargeTimer = chargeMaxTime;
                }
                ability.DealDamage(weapon.Entity, baseDamageModifier + maxChargeDamageIncrease * (chargeTimer / chargeMaxTime), 
                    weapon.HealthDamageReduction, weapon.PostureDamageReduction);
            }
        }

        protected override void Initialize()
        {
            animator = abilityRunner.Animator;
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            attacker = GameUtility.FindEntity(abilityRunner.gameObject);
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            if (weaponWielder.Mainhand != null)
            {
                int animation = Animator.StringToHash(weaponWielder.Mainhand.Name + "_heavy_attack");
                int index = abilityRunner.Animator.GetLayerIndex("Base Layer");
                return abilityRunner.Animator.HasState(index, animation) && weaponWielder.Mainhand.state != WeaponState.Attacking;
            }
            return false;
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)config;
            // Wait for signal to charge
            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            animator.speed = 0;
            // Wait for signal to attack
            while (!c.inputSignaled && c.chargeTimer < c.chargeMaxTime)
            {
                c.chargeTimer += Time.deltaTime;
                yield return null;
            }

            c.inputSignaled = false;
            animator.speed = weaponWielder.Mainhand.AttackSpeed;
            SubscribeWeaponEvent(c);

            // Wait for signal of recovery
            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            UnSubscribeWeaponEvent(c);

            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
        }

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            abilityRunner.StartAnimation<HeavyWeaponAttack>(config.Name, weaponWielder.Mainhand.Name + "_heavy_attack", weaponWielder.Mainhand.AttackSpeed);
            h.animationSignaled = false;
            h.inputSignaled = false;
            h.chargeTimer = 0;
            h.ability = this;
        }

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            h.animationSignaled = false;
            weaponWielder.Mainhand.Pause();
        }

        protected override void Signal(AbilityConfig config, bool isAnimation)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)config;
            if (isAnimation)
            {
                
                c.animationSignaled = true;
            }
            else {
                c.inputSignaled = true;
            }
        }

        private void SubscribeWeaponEvent(HeavyWeaponAttackConfig config)
        {
            weaponWielder.Mainhand.Action();
            weaponWielder.Mainhand.onEntityHit += config.DealDamage;
            weaponWielder.Mainhand.onWeaponHit += config.DealGuardedDamage;
        }

        private void UnSubscribeWeaponEvent(HeavyWeaponAttackConfig config)
        {
            weaponWielder.Mainhand.Pause();
            weaponWielder.Mainhand.onEntityHit -= config.DealDamage;
            weaponWielder.Mainhand.onWeaponHit -= config.DealGuardedDamage;
        }

        private void DealDamage(Entity entity, float modifier, float healthDamageReduction=0, float postureDamageReduction=0)
        {
            if (targets.IsTarget(entity))
            {
                float health = (0.7f * weaponWielder.Mainhand.Sharpness + 0.3f * weaponWielder.Mainhand.Weight) * 
                    modifier * ((100 - healthDamageReduction) / 100);
                float posture = (0.3f * weaponWielder.Mainhand.Sharpness + 0.7f * weaponWielder.Mainhand.Weight) * 
                    modifier * ((100 - postureDamageReduction) / 100);
                entity.Damage(health, posture, attacker);
                entity.ApplyForce(entity.transform.position - abilityRunner.transform.position, weaponWielder.Mainhand.Weight * modifier);
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
    }
}
