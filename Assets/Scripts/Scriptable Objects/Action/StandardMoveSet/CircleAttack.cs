using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Pool;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility.Groups;

namespace LobsterFramework.Action
{
    [RequireActionComponent(typeof(CircleAttack), typeof(CombatComponent))]
    [ActionInstance(typeof(CircleAttack))]
    public class CircleAttack : ActionInstance
    {
        public class CircleAttackConfig : ActionConfig
        {
            public Sprite sprite;
            public VarString spritePoolTag;
            public RefFloat spriteDuration;
            public TargetSetting targetSetting;
            public List<Effect> effects;
        }

        private CombatComponent combatComponent;
        private Entity attacker;

        protected override void Initialize()
        {
            combatComponent = actionComponent.GetActionComponent<CombatComponent>();
            attacker = actionComponent.GetComponent<Entity>();
        }

        protected override bool ExecuteBody(ActionConfig config)
        {
            /* Damages entities in radius of attack range by attack damage, has no effect on the attacker
             *  Entities without colliders are ignored
             */
            CombatComponent cc = combatComponent;
            GameObject gameObject = actionComponent.gameObject;
            Transform transform = gameObject.transform;
            float range = cc.attackRange.Value;
            int damage = cc.attackDamage.Value;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, range);

            // Set up attack sprite
            CircleAttackConfig c_config = (CircleAttackConfig)config;
            GameObject g = ObjectPool.Instance.GetObject(c_config.spritePoolTag.Value, Vector3.zero, Quaternion.identity);
            Transform gTransform = g.transform;
            gTransform.SetParent(transform);
            GameUtility.SetAbsoluteScale(g, new Vector2(range, range));

            TemporalObject tmp = g.GetComponent<TemporalObject>();
            tmp.ResetCounter();
            tmp.duration = c_config.spriteDuration.Value;

            SpriteRenderer spriteRenderer = g.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = c_config.sprite;
            int id = gameObject.GetInstanceID();

            foreach (Collider2D collider in colliders)
            {
                // Ignore attacker
                if (collider.gameObject.GetInstanceID() == id)
                {
                    continue;
                }
                Entity collided = collider.gameObject.GetComponent<Entity>();
                if (collided == null)
                {
                    continue;
                }

                if (c_config.targetSetting.IsTarget(collided))
                {
                    collided.RegisterDamage(damage, attacker);
                    foreach (Effect effect in c_config.effects)
                    {
                        collided.RegisterEffect(effect);
                    }
                }
            }

            return false;
        }
    }
}
