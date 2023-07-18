using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(WeaponWielder))]
    public class LightWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private TargetSetting targets;
        
        private WeaponWielder weaponWielder;
        private Entity attacker;

        public class LightWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector]
            public bool signaled;
        }

        protected override void Initialize()
        {
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            attacker = GameUtility.FindEntity(abilityRunner.gameObject);
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

        protected override IEnumerator<CoroutineOption> Coroutine(AbilityCoroutineConfig config)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            // Wait for signal to attack
            while (!c.signaled) { 
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

            while (!c.signaled) {
                yield return null;
            }
            c.signaled = false;
        }

        private void SubscribeWeaponEvent() {
            weaponWielder.Mainhand.Action();
            weaponWielder.Mainhand.onEntityHit += DealDamage;
            weaponWielder.Mainhand.onWeaponHit += DealGuardedDamage;
        }

        private void UnSubscribeWeaponEvent() {
            weaponWielder.Mainhand.Pause();
            weaponWielder.Mainhand.onEntityHit -= DealDamage;
            weaponWielder.Mainhand.onWeaponHit -= DealGuardedDamage;
        }

        private void DealDamage(Entity entity) {
            if (targets.IsTarget(entity)) {
                float health = 0.7f * weaponWielder.Mainhand.Sharpness + 0.3f * weaponWielder.Mainhand.Weight;
                float posture = 0.3f * weaponWielder.Mainhand.Sharpness + 0.7f * weaponWielder.Mainhand.Weight;
                entity.Damage(health, posture, attacker);
            }
        }

        public void DealGuardedDamage(Weapon weapon)
        {
            if (targets.IsTarget(weapon.Entity))
            { 
                float health = 0.7f * weapon.Sharpness + 0.3f * weapon.Weight;
                float posture = 0.3f * weapon.Sharpness + 0.7f * weapon.Weight;
                float hp = (100 - weapon.HealthDamageReduction) / 100;
                float pp = (100 - weapon.PostureDamageReduction) / 100;
                weapon.Entity.Damage(health * hp, posture * pp, attacker);
                Debug.Log("Guarded:" + Time.time);
            }
        }

        protected override void OnCoroutineEnqueue(AbilityCoroutineConfig config)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            c.signaled = false;
            abilityRunner.StartAnimation<LightWeaponAttack>(config.Name, weaponWielder.Mainhand.Name + "_light_attack", weaponWielder.Mainhand.AttackSpeed);
        }

        protected override void Signal(AbilityConfig config, bool isAnimation)
        {
            if (isAnimation) {
                LightWeaponAttackConfig c = (LightWeaponAttackConfig)config; 
                c.signaled = true;
            }
        }

        protected override void OnCoroutineFinish(AbilityCoroutineConfig config){
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
