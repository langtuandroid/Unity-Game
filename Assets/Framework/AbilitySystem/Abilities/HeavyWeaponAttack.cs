using UnityEngine;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using System.Collections.Generic;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    [ComponentRequired(typeof(WeaponWielder))]
    public class HeavyWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private TargetSetting targets;
        

        private Animator animator;
        private WeaponWielder weaponWielder;
        private Entity attacker;
        private MovementController moveControl;
        private DamageModifier damageModifier;

        public class HeavyWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
            public RefFloat baseDamageModifier;
            public RefFloat maxChargeDamageIncrease;
            public RefFloat chargeMaxTime;
            public VarString clashSparkTag;

            [HideInInspector]
            public int m_key = -1;
            [HideInInspector]
            public int r_key = -1;

            [HideInInspector]
            public float chargeTimer;
            [HideInInspector]
            public HeavyWeaponAttack ability;

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
                    ObjectPool.Instance.GetObject(clashSparkTag.Value, contactPoint, Quaternion.identity);
                }

                ability.DealDamage(weapon.Entity, baseDamageModifier + maxChargeDamageIncrease * (chargeTimer / chargeMaxTime), 
                    weapon.HealthDamageReduction, weapon.PostureDamageReduction);
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
            animator = abilityRunner.Animator;
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            attacker = weaponWielder.Wielder;
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
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

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config, AbilityPipe pipe)
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

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config, AbilityPipe pipe)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            abilityRunner.StartAnimation(this, config.Key, weaponWielder.Mainhand.Name + "_heavy_attack", weaponWielder.Mainhand.AttackSpeed);
            h.animationSignaled = false;
            h.inputSignaled = false;
            h.chargeTimer = 0;
            h.ability = this;
            h.m_key = moveControl.ModifyMoveSpeed(weaponWielder.Mainhand.HMoveSpeedModifier);
            h.r_key = moveControl.ModifyRotationSpeed(weaponWielder.Mainhand.HRotationSpeedModifier);
        }

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            h.animationSignaled = false;
            weaponWielder.Mainhand.Pause();
            if (h.m_key != -1) {
                moveControl.UnmodifyMoveSpeed(h.m_key);
            }
            if (h.r_key != -1) {
                moveControl.UnmodifyRotationSpeed(h.r_key);
            }
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

        private void SubscribeWeaponEvent(HeavyWeaponAttackConfig config)
        {
            weaponWielder.Mainhand.Action();
            weaponWielder.Mainhand.onEntityHit += config.OnEntityHit;
            weaponWielder.Mainhand.onWeaponHit += config.OnWeaponHit;
        }

        private void UnSubscribeWeaponEvent(HeavyWeaponAttackConfig config)
        {
            weaponWielder.Mainhand.Pause();
            weaponWielder.Mainhand.onEntityHit -= config.OnEntityHit;
            weaponWielder.Mainhand.onWeaponHit -= config.OnWeaponHit;
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

        protected override void OnAnimationInterrupt(AbilityConfig config)
        {
            HaltOnAllConfigs();
        }


        protected override void OnCoroutineReset(AbilityCoroutineConfig config)
        {
            throw new System.NotImplementedException();
        }
    }
}
