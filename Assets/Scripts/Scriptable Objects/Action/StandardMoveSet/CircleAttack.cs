using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[RequireActionComponent(typeof(CircleAttack), typeof(CombatComponent))]
[ActionInstance(typeof(CircleAttack))]
public class CircleAttack : ActionInstance
{
    [SerializeField] private EntityGroup[] targetGroups;
    [SerializeField] private EntityGroup[] ignoreGroups;
    [SerializeField] private Sprite sprite;
    [SerializeField] private VarString spritePoolTag;
    [SerializeField] private RefFloat spriteDuration;

    private HashSet<Entity> targets = new();
    private HashSet<Entity> ignoreTargets = new();
    private CombatComponent combatComponent;

    public override void Initialize()
    {
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
        combatComponent =  actionComponent.GetActionComponent<CombatComponent>();
    }

    public override void CleanUp()
    {
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

    protected override void ExecuteBody() {
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
        GameObject g = ObjectPool.Instance.GetObject(spritePoolTag.Value, Vector3.zero, Quaternion.identity);
        Transform gTransform = g.transform;
        gTransform.SetParent(transform);
        gTransform.localPosition = Vector3.zero;
        gTransform.localScale = Vector3.one;
        Vector3 scale = gTransform.lossyScale;
        gTransform.localScale = new Vector3(range / scale.x, range / scale.y, 1);
        TemporalObject tmp = g.GetComponent<TemporalObject>();
        tmp.ResetCounter();
        tmp.duration = spriteDuration.Value;
        SpriteRenderer spriteRenderer = g.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

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

            if (targets.Contains(collided) && !ignoreTargets.Contains(collided))
            {
                collided.RegisterDamage(damage);
            }
        }
    }
}
