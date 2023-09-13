using UnityEngine;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using System.Collections.Generic;
using Animancer;
using static UnityEngine.EventSystems.EventTrigger;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    [ComponentRequired(typeof(WeaponWielder))]
    public class HeavyWeaponAttack : WeaponAbility
    {
        [SerializeField] private TargetSetting targets;
        
        private MovementController moveControl;
        private DamageModifier damageModifier;

        protected override void Init()
        {
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
        }

        protected override bool WConditionSatisfied(AbilityConfig config)
        {
            return WeaponWielder.GetAbilityClip(GetType(), WeaponWielder.Mainhand.WeaponType) != null && WeaponWielder.Mainhand.state != WeaponState.Attacking;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)CurrentConfig;
            h.currentWeapon = WeaponWielder.Mainhand;
            h.SubscribeWeaponEvent();
            h.animationSignaled = false;
            h.inputSignaled = false;
            h.chargeTimer = 0;
            h.ability = this;
            h.m_key = moveControl.ModifyMoveSpeed(h.currentWeapon.HMoveSpeedModifier);
            h.r_key = moveControl.ModifyRotationSpeed(h.currentWeapon.HRotationSpeedModifier);
            h.animationState = abilityRunner.StartAnimation(this, CurrentConfigName, WeaponWielder.GetAbilityClip(GetType(), h.currentWeapon.WeaponType), WeaponWielder.Mainhand.AttackSpeed);
        }

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityPipe pipe)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)CurrentConfig;
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

        protected override void OnCoroutineFinish()
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)CurrentConfig;
            h.UnSubscribeWeaponEvent();
            h.animationSignaled = false;
            h.currentWeapon.Pause();
            moveControl.UnmodifyMoveSpeed(h.m_key);
            moveControl.UnmodifyRotationSpeed(h.r_key);
        }

        protected override void Signal(AnimationEvent animationEvent)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)CurrentConfig;
            if (animationEvent != null)
            {
                c.animationSignaled = true;
            }
            else {
                c.inputSignaled = true;
            }
        }

        private void DealDamage(Entity entity, float modifier)
        {
            if (targets.IsTarget(entity))
            {
                WeaponUtility.WeaponDamage(WeaponWielder.Mainhand, entity, damageModifier, modifier);
            }
        }

        private void DealGuardDamage(Weapon guardWeapon, float modifier) {
            Entity entity = guardWeapon.Entity;
            if (targets.IsTarget(entity))
            {
                WeaponUtility.GuardDamage(WeaponWielder.Mainhand, guardWeapon, damageModifier, modifier);
            }
        }

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }
        public class HeavyWeaponAttackConfig : AbilityCoroutineConfig
        {

            public RefFloat baseDamageModifier;
            public RefFloat maxChargeDamageIncrease;
            public RefFloat chargeMaxTime;

            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
            [HideInInspector] public Weapon currentWeapon;
            [HideInInspector] public AnimancerState animationState;

            [HideInInspector] public int m_key = -1;
            [HideInInspector] public int r_key = -1;

            [HideInInspector] public float chargeTimer;
            [HideInInspector] public HeavyWeaponAttack ability;

            public void OnEntityHit(Entity entity)
            {
                if (chargeTimer > chargeMaxTime)
                {
                    chargeTimer = chargeMaxTime;
                }
                ability.DealDamage(entity, baseDamageModifier + maxChargeDamageIncrease * (chargeTimer / chargeMaxTime));
            }

            public void OnWeaponHit(Weapon weapon, Vector3 contactPoint)
            {
                if (chargeTimer > chargeMaxTime)
                {
                    chargeTimer = chargeMaxTime;
                }
                if (weapon.ClashSpark != null)
                {
                    Pool.ObjectPool.GetObject(weapon.ClashSpark, contactPoint, Quaternion.identity);
                }

                ability.DealGuardDamage(weapon, baseDamageModifier + maxChargeDamageIncrease * (chargeTimer / chargeMaxTime));
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
        public class HeavyWeaponAttackPipe : AbilityPipe
        {
            private HeavyWeaponAttackConfig conf;
            public float MaxChargeTime { get { return conf.chargeMaxTime; } }
            public float MaxChargeDamageIncrease { get { return conf.maxChargeDamageIncrease; } }
            public float BaseDamageModifier { get { return conf.baseDamageModifier; } }

            public override void Construct()
            {
                conf = (HeavyWeaponAttackConfig)config;
            }
        }
    }
}
