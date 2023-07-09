using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using System.Collections;
using UnityEditor.Playables;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(WeaponWielder))]
    public class LightWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private string animation;
        [SerializeField] private TargetSetting targets;
        
        private Animator animator;
        private WeaponWielder weaponWielder;
        private Entity attacker;

        public class LightWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector]
            public bool signaled;
        }

        protected override void Initialize()
        {
            animator = abilityRunner.Animator;
            weaponWielder = abilityRunner.GetComponent<WeaponWielder>();
            attacker = GameUtility.FindEntity(abilityRunner.gameObject);
        }

        protected override IEnumerator Coroutine(AbilityConfig config)
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
            weaponWielder.Weapon.Activate();
            weaponWielder.Weapon.onEntityHit += DealDamage;
            weaponWielder.Weapon.onWeaponHit += DealGuardedDamage;
        }

        private void UnSubscribeWeaponEvent() {
            weaponWielder.Weapon.Deactivate();
            weaponWielder.Weapon.onEntityHit -= DealDamage;
            weaponWielder.Weapon.onWeaponHit -= DealGuardedDamage;
        }

        private void DealDamage(Entity entity) {
            if (targets.IsTarget(entity)) {
                float health = 0.7f * weaponWielder.Weapon.Sharpness + 0.3f * weaponWielder.Weapon.Weight;
                float posture = 0.3f * weaponWielder.Weapon.Sharpness + 0.7f * weaponWielder.Weapon.Weight;
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

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            c.signaled = false;
            abilityRunner.StartAnimation<LightWeaponAttack>(configName, animation, weaponWielder.Weapon.AttackSpeed);
        }

        protected override void Signal(AbilityConfig config, bool isAnimation)
        {
            if (isAnimation) {
                LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
                c.signaled = true;
            }
        }

        protected override void OnCoroutineFinish(AbilityConfig config){}

        protected override void OnAnimationInterrupt(AbilityConfig config)
        {
            HaltAbilities();
        }
    }
}
