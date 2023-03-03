using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[RequireActionComponent(typeof(CircleAttack), typeof(CombatComponent))]
[ActionInstance(typeof(CircleAttack))]
public class CircleAttack : ActionInstance
{
    public class CircleAttackConfig : ActionConfig {
        public EntityGroup[] targetGroups;
        public EntityGroup[] ignoreGroups;
        public Sprite sprite;
        public VarString spritePoolTag;
        public RefFloat spriteDuration;
        public List<Effect> effects;
        public HashSet<Entity> targets = new();
        public HashSet<Entity> ignoreTargets = new();

        public override void Initialize() {
            foreach (EntityGroup group in targetGroups)
            {
                group.OnEntityAdded.AddListener((Entity entity) => { targets.Add(entity); });
                foreach (Entity entity in group)
                {
                    targets.Add(entity);
                }
            }

            foreach (EntityGroup group in ignoreGroups)
            {
                group.OnEntityAdded.AddListener((Entity entity) => { ignoreTargets.Add(entity); });
                foreach (Entity entity in group)
                {
                    ignoreTargets.Add(entity);
                }
            }
        }

        public override void Close() {
            foreach (EntityGroup group in targetGroups)
            {
                group.OnEntityAdded.RemoveListener((Entity entity) => { targets.Add(entity); });
            }

            foreach (EntityGroup group in ignoreGroups)
            {
                group.OnEntityAdded.RemoveListener((Entity entity) => { ignoreTargets.Add(entity); });
            }
            targets.Clear();
            ignoreTargets.Clear();
        }
    }
    
    private CombatComponent combatComponent;
    private Entity attacker;

    protected override void Initialize()
    {   
        combatComponent =  actionComponent.GetActionComponent<CombatComponent>();
        attacker = actionComponent.GetComponent<Entity>();
    }

    protected override bool ExecuteBody(ActionConfig config) {
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
        gTransform.localPosition = Vector3.zero;
        gTransform.localScale = Vector3.one;
        Vector3 scale = gTransform.lossyScale;
        gTransform.localScale = new Vector3(range / scale.x, range / scale.y, 1);
        TemporalObject tmp = g.GetComponent<TemporalObject>();
        tmp.ResetCounter();
        tmp.duration = c_config.spriteDuration.Value;
        SpriteRenderer spriteRenderer = g.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite =  c_config.sprite;

        foreach (Collider2D collider in colliders)
        {
            // Ignore attacker
            if (collider.gameObject.name == gameObject.name) {
                continue;
            }
            Entity collided = collider.gameObject.GetComponent<Entity>();
            if (collided == null)
            {
                continue;
            }

            if (c_config.targets.Contains(collided) && !c_config.ignoreTargets.Contains(collided))
            {
                collided.RegisterDamage(damage, attacker);
                foreach(Effect effect in c_config.effects) {
                    collided.RegisterEffect(effect);
                }
            }
        }

        return false;
    }
}
