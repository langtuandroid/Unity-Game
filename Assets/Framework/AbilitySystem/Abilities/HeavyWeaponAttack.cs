using UnityEngine;
using System.Collections.Generic;
using Animancer;
using LobsterFramework.Utility;

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
        private BufferedValueAccessor<float> move;
        private BufferedValueAccessor<float> rotate;

        protected override void Init()
        {
            moveControl = abilityRunner.GetComponentInBoth<MovementController>();
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
            move = moveControl.moveSpeedModifier.GetAccessor();
            rotate = moveControl.rotateSpeedModifier.GetAccessor();
        }

        protected override bool WConditionSatisfied(AbilityConfig config)
        {
            return WeaponWielder.GetAbilityClip(GetType(), WeaponWielder.Mainhand.WeaponType) != null && WeaponWielder.Mainhand.state != WeaponState.Attacking;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)CurrentConfig;
            h.currentWeapon = WeaponWielder.Mainhand;
            SubscribeWeaponEvent();
            h.animationSignaled = false;
            h.inputSignaled = false;
            h.chargeTimer = 0;
            h.ability = this;
            move.Acquire(h.currentWeapon.HMoveSpeedModifier);
            rotate.Acquire(h.currentWeapon.HRotationSpeedModifier);
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
            c.currentWeapon.Enable();

            // Wait for signal of recovery
            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            c.currentWeapon.Disable();

            // Wait for animation to finish
            while (true)
            {
                yield return null;
            }
        }

        protected override void OnCoroutineFinish()
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)CurrentConfig;
            UnSubscribeWeaponEvent();
            h.animationSignaled = false;
            h.currentWeapon.Disable();
            move.Release();
            rotate.Release();
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

        protected override void OnCoroutineReset()
        {
            throw new System.NotImplementedException();
        }

        public void OnEntityHit(Entity entity)
        {
            HeavyWeaponAttackConfig config = (HeavyWeaponAttackConfig)CurrentConfig;
            if (targets.IsTarget(entity))
            {
                if (config.chargeTimer > config.chargeMaxTime)
                {
                    config.chargeTimer = config.chargeMaxTime;
                }
                config.currentWeapon.SetOnHitDamage(WeaponUtility.ComputeDamage(WeaponWielder.Mainhand, damageModifier));
            }
            else {
                config.currentWeapon.SetOnHitDamage(Damage.none);
            }
        }

        public void OnWeaponHit(Weapon weapon)
        {
            HeavyWeaponAttackConfig config = (HeavyWeaponAttackConfig)CurrentConfig;
            if (targets.IsTarget(weapon.Entity))
            {
                if (config.chargeTimer > config.chargeMaxTime)
                {
                    config.chargeTimer = config.chargeMaxTime;
                }
                config.currentWeapon.SetOnHitDamage(WeaponUtility.ComputeDamage(WeaponWielder.Mainhand, damageModifier));
            }
            else
            {
                config.currentWeapon.SetOnHitDamage(Damage.none);
            }
        }

        public void SubscribeWeaponEvent()
        {
            HeavyWeaponAttackConfig config = (HeavyWeaponAttackConfig)CurrentConfig;
            config.currentWeapon.onEntityHit += OnEntityHit;
            config.currentWeapon.onWeaponHit += OnWeaponHit;
        }

        public void UnSubscribeWeaponEvent()
        {
            HeavyWeaponAttackConfig config = (HeavyWeaponAttackConfig)CurrentConfig;
            config.currentWeapon.onEntityHit -= OnEntityHit;
            config.currentWeapon.onWeaponHit -= OnWeaponHit;
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

            [HideInInspector] public float chargeTimer;
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
