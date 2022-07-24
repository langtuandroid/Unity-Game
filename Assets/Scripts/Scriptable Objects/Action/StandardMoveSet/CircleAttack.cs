using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[ActionInstance(typeof(CircleAttack))]
public class CircleAttack : ActionInstance
{
    [Header("Required Action Components")]
    public readonly List<string> required = new List<string>{"CombatComponent"};
    protected override void ExecuteBody() {
        /* Damages entities in radius of attack range by attack damage, has no effect on the attacker
         * 
         */
        CombatComponent cc = actionComponent.GetActionComponent<CombatComponent>();
        if (cc == null) {
            Debug.Log("The object is missing the required action component to complete the action!");
            return;
        }
        GameObject gameObject = actionComponent.gameObject;
        Transform transform = gameObject.transform;
        float range = cc.attackRange;
        int damage = cc.attackDamage;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, (float)(range / 2));
        GameObject g = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(range,
            range, 1), Color.yellow, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        Object.Destroy(g, 0.25f);
        foreach (Collider2D collider in colliders)
        {
            Entity collided = collider.gameObject.GetComponent<Entity>();
            if (collided == null)
            {
                continue;
            }
            if (actionComponent.gameObject.name != collider.gameObject.name)
            {
                collided.RegisterDamage(damage);
            }
        }
    }
}
