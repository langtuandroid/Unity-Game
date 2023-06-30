using UnityEngine;
using LobsterFramework.AbilitySystem;
using System.Collections;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using static GameScripts.Abilities.LightWeaponAttack;

namespace GameScripts.Abilities
{
    [AddAbilityMenu]
    [ComponentRequired(typeof(Animator))]
    public class HeavyWeaponAttack : AbilityCoroutine
    {
        [SerializeField] private string animation;
        [SerializeField] private TargetSetting targets;

        private Animator animator;
        private WeaponWielder weapon;
        private Entity attacker;
        public class HeavyWeaponAttackConfig : AbilityCoroutineConfig {
            [HideInInspector] public bool animationSignaled;
            [HideInInspector] public bool inputSignaled;
        }

        protected override void Initialize()
        {
            animator = abilityRunner.Animator;
            weapon = abilityRunner.GetComponent<WeaponWielder>();
            attacker = GameUtility.FindEntity(abilityRunner.gameObject);
        }

        protected override IEnumerator Coroutine(AbilityConfig config)
        {
            HeavyWeaponAttackConfig c = (HeavyWeaponAttackConfig)config;
            Debug.Log("WindUp");
            // Wait for signal to charge
            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            animator.speed = 0;
            Debug.Log("Charging");
            // Wait for signal to attack
            while (!c.inputSignaled)
            {
                yield return null;
            }

            Debug.Log("Attack");
            c.inputSignaled = false;
            animator.speed = weapon.Weapon.AttackSpeed;
            SubscribeWeaponEvent();

            // Wait for signal of recovery
            while (!c.animationSignaled)
            {
                yield return null;
            }
            Debug.Log("Recovery");
            c.animationSignaled = false;
            UnSubscribeWeaponEvent();

            while (!c.animationSignaled)
            {
                yield return null;
            }
            c.animationSignaled = false;
            Debug.Log("Finish");
        }

        protected override void OnCoroutineEnqueue(AbilityConfig config, string configName)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            abilityRunner.StartAnimation<HeavyWeaponAttack>(configName, animation);
            h.animationSignaled = false;
            h.inputSignaled = false;
        }

        protected override void OnCoroutineFinish(AbilityConfig config)
        {
            HeavyWeaponAttackConfig h = (HeavyWeaponAttackConfig)config;
            h.animationSignaled = false;
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

        private void SubscribeWeaponEvent()
        {
            weapon.Weapon.Activate();
            weapon.Weapon.onEntityHit += DealDamage;
        }

        private void UnSubscribeWeaponEvent()
        {
            weapon.Weapon.Deactivate();
            weapon.Weapon.onEntityHit -= DealDamage;
        }

        private void DealDamage(Entity entity)
        {
            if (targets.IsTarget(entity))
            {
                float health = 0.7f * weapon.Weapon.Sharpness + 0.3f * weapon.Weapon.Weight;
                float posture = 0.3f * weapon.Weapon.Sharpness + 0.7f * weapon.Weapon.Weight;
                entity.Damage(health, posture, attacker);
            }
        }

        protected override void OnAnimationInterrupt(AbilityConfig config)
        {
            HaltAbilities();
        }
    }
}
