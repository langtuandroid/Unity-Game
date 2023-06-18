using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility.Groups;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;

namespace LobsterFramework.AbilitySystem
{
    [Ability(typeof(Shoot))]
    [RequireAbilityStats(typeof(Shoot), typeof(CombatStat), typeof(Mana))]
    public class Shoot : Ability
    {


        public class ShootConfig : AbilityConfig
        {
            public VarString bulletPoolTag;
            public RefFloat manaCost;
            public RefFloat bulletSpeed;
            public RefInt piercePower;
            public RefFloat travelDuration;
            public TargetSetting targetSetting;
        }

        private Mana manaComponent;
        private Entity attacker;


        protected override void Initialize()
        {
            manaComponent = abilityRunner.GetAbilityStat<Mana>();
            attacker = abilityRunner.GetComponent<Entity>();
        }

        protected override bool ConditionSatisfied(AbilityConfig config)
        {
            ShootConfig s_config = (ShootConfig)config;
            return manaComponent.AvailableMana >= s_config.manaCost.Value;
        }

        protected override void OnEnqueue(AbilityConfig config, string configName)
        {
            ShootConfig s_config = (ShootConfig)config;
            manaComponent.ReserveMana(s_config.manaCost.Value);
        }

        protected override bool Action(AbilityConfig a_config)
        {
            ShootConfig config = (ShootConfig)a_config;
            CombatStat combatComponent = abilityRunner.GetAbilityStat<CombatStat>();
            int firePower = combatComponent.attackDamage.Value;

            Transform transform = abilityRunner.transform;
            GameObject obj = ObjectPool.Instance.GetObject(config.bulletPoolTag.Value, transform.position + Vector3.Scale(transform.lossyScale, transform.up), transform.rotation);
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("The prefab used under the tag: " + config.bulletPoolTag.Value + " is not a valid bullet prefab.");
                obj.SetActive(false);
                return false;
            }

            Transform bulletTransform = obj.transform;
            bullet.Initialize(config.targetSetting, config.travelDuration.Value, attacker, firePower, config.piercePower.Value);
            Rigidbody2D body = bullet.GetComponent<Rigidbody2D>();
            body.velocity = bulletTransform.up.normalized * config.bulletSpeed.Value;
            return false;
        }
    }
}
