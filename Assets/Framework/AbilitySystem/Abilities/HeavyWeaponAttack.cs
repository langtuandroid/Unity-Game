using UnityEngine;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using System.Collections.Generic;
using Animancer;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    [ComponentRequired(typeof(WeaponWielder))]
    public class HeavyWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private TargetSetting targets;
        
        private WeaponWielder weaponWielder;
        private Entity attacker;
        private MovementController moveControl;
        private DamageModifier damageModifier;

        public class HeavyWeaponAttackConfig : AbilityCoroutineConfig {
            
            public RefFloat baseDamageModifier;
            public RefFloat maxChargeDamageIncrease;
            public RefFloat chargeMaxTime;
            public VarString clashSparkTag;

            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
            [HideInInspector] public Weapon currentWeapon;
            [HideInInspector] public AnimancerState animationState;

            [HideInInspector] public int m_key = -1;
            [HideInInspector] public int r_key = -1;

            [HideInInspector] public float chargeTimer;
            [HideInInspector] public HeavyWeaponAttack ability;

            public void OnEntityHit(Entity entity) {
                if (chargeTimer > chargeMaxTime) { 
                    chargeTimer = chargeMaxTime;
                }
                ability.DealDamage(entity, baseDamageModifier + maxChargeDamageIncrease *  (chargeTimer / chargeMaxTime));
            }

            public void OnWeaponHit(Weapon weapon, Vector3 contactPoint) {
                if (chargeTimer > chargeMaxTime)
                {
                    chargeTimer = chargeMaxTime;
                }
                if (clashSparkTag != null) {
                    Pool.ObjectPool.Instance.GetObject(clashSparkTag.Value, contactPoint, Quaternion.identity);
                }

                ability.DealDamage(weapon.Entity, baseDamageModifier + maxChargeDamageIncrease * (chargeTimer / chargeMaxTime), 
                    weapon.HealthDamageReduction, weapon.PostureDamageReduction);
            }

            public void SubscribeWeaponEvent()
            {
                currentWeapon.onEntityHit += OnEntityHit;
                currentWeapon.onWeaponHit += OnWeaponHit;
            }

            public void UnSubscribeWeaponEvent()
            {
                currentWeapon.onEntityHit -= OnEntityHit;
                currentWeapon.onWeaponHit -= OnWeaponHit;
            }
        }
        public class HeavyWeaponAttackPipe : AbilityPipe {
            private HeavyWeaponAttackConfig conf;
            public float MaxChargeTime { get { return conf.chargeMaxTime; } }
            public float MaxChargeDamageIncrease { get { return conf.maxChargeDamageIncrease; } }
            public float BaseDamageModifier { get { return conf.baseDamageModifier; } }

            public override void Construct()
            {
                conf = (HeavyWeaponAttackConfig)config;
            }
        }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            attacker = weaponWielder.Wielder;
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
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

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config, AbilityPipe pipe)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            h.currentWeapon = weaponWielder.Mainhand;
            h.SubscribeWeaponEvent();
            h.animationSignaled = false;
            h.inputSignaled = false;
            h.chargeTimer = 0;
            h.ability = this;
            h.m_key = moveControl.ModifyMoveSpeed(h.currentWeapon.HMoveSpeedModifier);
            h.r_key = moveControl.ModifyRotationSpeed(h.currentWeapon.HRotationSpeedModifier);
            h.animationState = abilityRunner.StartAnimation(this, CurrentConfigName, weaponWielder.GetAbilityClip(GetType(), h.currentWeapon.WeaponType), weaponWielder.Mainhand.AttackSpeed);
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config, AbilityPipe pipe)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)config;
            // Wait for signal to charge
            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            c.animationState.IsPlaying = false;
            // Wait for signal to attack
            while (!c.inputSignaled && c.chargeTimer < c.chargeMaxTime)
            {
                c.chargeTimer += Time.deltaTime;
                yield return null;
            }

            c.inputSignaled = false;
            c.animationState.IsPlaying = true;
            c.currentWeapon.Action();

            // Wait for signal of recovery
            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            c.currentWeapon.Pause();

            // Wait for animation to finish
            while (true)
            {
                yield return null;
            }
        }

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            h.UnSubscribeWeaponEvent();
            h.animationSignaled = false;
            h.currentWeapon.Pause();
            moveControl.UnmodifyMoveSpeed(h.m_key);
            moveControl.UnmodifyRotationSpeed(h.r_key);
        }

        protected override void Signal(AbilityConfig config, AnimationEvent animationEvent)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)config;
            if (animationEvent != null)
            {
                c.animationSignaled = true;
            }
            else {
                c.inputSignaled = true;
            }
        }

        

        private void DealDamage(Entity entity, float modifier, float healthDamageReduction=0, float postureDamageReduction=0)
        {
            if (targets.IsTarget(entity))
            {
                float health = (0.7f * weaponWielder.Mainhand.Sharpness + 0.3f * weaponWielder.Mainhand.Weight) * 
                    modifier * (1 - healthDamageReduction);
                float posture = (0.3f * weaponWielder.Mainhand.Sharpness + 0.7f * weaponWielder.Mainhand.Weight) * 
                    modifier * (1- postureDamageReduction);
                Damage damage = new() { health=health, posture=posture, source=attacker};
                entity.Damage(damageModifier.ModifyDamage(damage));
                MovementController moveControl = entity.GetComponent<MovementController>();
                if (moveControl != null)
                {
                    moveControl.ApplyForce(entity.transform.position - abilityRunner.transform.position, weaponWielder.Mainhand.Weight * modifier);
                }
            }
        }

        protected override void OnCoroutineReset(AbilityCoroutineConfig config)
        {
            throw new System.NotImplementedException();
        }
    }
}
