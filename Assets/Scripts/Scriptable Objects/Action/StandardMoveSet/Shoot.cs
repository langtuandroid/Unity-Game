using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility.Groups;
using LobsterFramework.EntitySystem;
using LobsterFramework.Pool;

namespace LobsterFramework.Action
{
    [ActionInstance(typeof(Shoot))]
    [RequireActionComponent(typeof(Shoot), typeof(CombatComponent), typeof(Mana))]
    public class Shoot : ActionInstance
    {


        public class ShootConfig : ActionConfig
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
            manaComponent = actionComponent.GetActionComponent<Mana>();
            attacker = actionComponent.GetComponent<Entity>();
        }

        protected override bool ConditionSatisfied(ActionConfig config)
        {
            ShootConfig s_config = (ShootConfig)config;
            return manaComponent.AvailableMana >= s_config.manaCost.Value;
        }

        protected override void OnEnqueue(ActionConfig config)
        {
            ShootConfig s_config = (ShootConfig)config;
            manaComponent.ReserveMana(s_config.manaCost.Value);
        }

        protected override bool ExecuteBody(ActionConfig a_config)
        {
            ShootConfig config = (ShootConfig)a_config;
            CombatComponent combatComponent = actionComponent.GetActionComponent<CombatComponent>();
            int firePower = combatComponent.attackDamage.Value;

            Transform transform = actionComponent.transform;
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
