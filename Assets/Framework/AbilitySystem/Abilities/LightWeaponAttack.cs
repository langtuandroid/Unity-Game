using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;
using static UnityEngine.EventSystems.EventTrigger;

namespace LobsterFramework.AbilitySystem
{
    [AddAbilityMenu]
    [RequireAbilityStats(typeof(DamageModifier))]
    [ComponentRequired(typeof(WeaponWielder))]
    public class LightWeaponAttack : WeaponAbility
    {
        [SerializeField] private TargetSetting targets;
        [SerializeField] private VarString clashSparkTag;
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

        protected override void Init()
        {
            attacker = WeaponWielder.Wielder;
            moveControl = attacker.GetComponent<MovementController>();
            damageModifier = abilityRunner.GetAbilityStat<DamageModifier>();
        }

        protected override bool WConditionSatisfied(AbilityConfig config)
        {
            return WeaponWielder.GetAbilityClip(GetType(), WeaponWielder.Mainhand.WeaponType) != null && WeaponWielder.Mainhand.state != WeaponState.Attacking;
        }

        protected override void OnCoroutineEnqueue(AbilityPipe pipe)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)CurrentConfig;
            c.currentWeapon = WeaponWielder.Mainhand;
            SubscribeWeaponEvent(c.currentWeapon);
            c.signaled = false;
            c.m_key = moveControl.ModifyMoveSpeed(c.currentWeapon.LMoveSpeedModifier);
            c.r_key = moveControl.ModifyRotationSpeed(c.currentWeapon.LRotationSpeedModifier);
            abilityRunner.StartAnimation(this, CurrentConfigName, WeaponWielder.GetAbilityClip(GetType(), WeaponWielder.Mainhand.WeaponType), c.currentWeapon.AttackSpeed);
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
            if (targets.IsTarget(entity))
            {
                WeaponUtility.WeaponDamage(WeaponWielder.Mainhand, entity, damageModifier);
            }
        }

        private void OnWeaponHit(Weapon weapon, Vector3 contactPoint)
        {
            if (clashSparkTag != null)
            {
                ObjectPool.Instance.GetObject(clashSparkTag.Value, contactPoint, Quaternion.identity);
            }
            Entity entity = weapon.Entity;
            if (targets.IsTarget(entity))
            {
                WeaponUtility.GuardDamage(WeaponWielder.Mainhand, weapon, damageModifier);
            }
        }
    }
}
