using LobsterFramework.AbilitySystem;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using System.Collections;
using UnityEngine;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(WeaponWielder))]
    public class LightWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private string animation;
        [SerializeField] private TargetSetting targets;
        
        private Animator animator;
        private WeaponWielder weapon;
        private Entity attacker;

        public class LightWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector]
            public bool signaled;
        }

        protected override void Initialize()
        {
            animator = abilityRunner.Animator;
            weapon = abilityRunner.GetComponent<WeaponWielder>();
            attacker = GameUtility.FindEntity(abilityRunner.gameObject);
        }

        protected override IEnumerator Coroutine(AbilityConfig config)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            Debug.Log("WindUp");
            // Wait for signal to attack
            while (!c.signaled) { 
                yield return null;
            }
            Debug.Log("Attack");
            c.signaled = false;
            SubscribeWeaponEvent();

            // Wait for signal of recovery
            while (!c.signaled)
            {
                yield return null;
            }
            Debug.Log("Recovery");
            c.signaled = false;
            UnSubscribeWeaponEvent();

            while (!c.signaled) {
                yield return null;
            }
            c.signaled = false;
            Debug.Log("Finish");
        }

        private void SubscribeWeaponEvent() {
            weapon.Weapon.Activate();
            weapon.Weapon.onEntityHit += DealDamage;
        }

        private void UnSubscribeWeaponEvent() {
            weapon.Weapon.Deactivate();
            weapon.Weapon.onEntityHit -= DealDamage;
        }

        private void DealDamage(Entity entity) {
            if (targets.IsTarget(entity)) {
                float health = 0.7f * weapon.Weapon.Sharpness + 0.3f * weapon.Weapon.Weight;
                float posture = 0.3f * weapon.Weapon.Sharpness + 0.7f * weapon.Weapon.Weight;
                entity.Damage(health, posture, attacker);
            }
        }

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            LightWeaponAttackConfig c = (LightWeaponAttackConfig)config;
            c.signaled = false;
            abilityRunner.StartAnimation<LightWeaponAttack>(configName, animation, weapon.Weapon.AttackSpeed);
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
