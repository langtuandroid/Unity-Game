
using LobsterFramework.Utility;
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
        private Entity attacker;
        private MovementController moveControl;
        private DamageModifier damageModifier;
        private BufferedValueAccessor<float> move;
        private BufferedValueAccessor<float> rotate;

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
            move = moveControl.moveSpeedModifier.GetAccessor();
            rotate = moveControl.rotateSpeedModifier.GetAccessor();
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
            move.Acquire(c.currentWeapon.LMoveSpeedModifier);
            rotate.Acquire(c.currentWeapon.LRotationSpeedModifier);
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

            c.currentWeapon.Enable();
            // Wait for signal of recovery
            while (!c.signaled)
            {
                yield return null;
            }
            c.signaled = false;
            c.currentWeapon.Disable();

            move.Release();
            rotate.Release();
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
            l.currentWeapon.Disable();
            move.Release();
            rotate.Release();
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
            LightWeaponAttackConfig config = (LightWeaponAttackConfig)CurrentConfig;
            if (targets.IsTarget(entity))
            {
                config.currentWeapon.SetOnHitDamage(WeaponUtility.ComputeDamage(WeaponWielder.Mainhand, damageModifier));
            }
            else {
                config.currentWeapon.SetOnHitDamage(Damage.none);
            }
        }

        private void OnWeaponHit(Weapon weapon)
        {
            LightWeaponAttackConfig config = (LightWeaponAttackConfig)CurrentConfig;
            
            if (targets.IsTarget(weapon.Entity))
            {
                config.currentWeapon.SetOnHitDamage(WeaponUtility.ComputeDamage(WeaponWielder.Mainhand, damageModifier));
            }
            else
            {
                config.currentWeapon.SetOnHitDamage(Damage.none);
            }
        }
    }
}
