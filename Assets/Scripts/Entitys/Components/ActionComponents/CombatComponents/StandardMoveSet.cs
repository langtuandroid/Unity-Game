using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class StandardMoveSet : Entity.EntityComponent, IActionImplementor
{
    private GameObject guardAnimation;
    private bool animationDestroyed;
    private static readonly Dictionary<string, ActionInstance> actions;
    private Combat combatComponent;
    private Dictionary<string, ActionInstance> actionInstances;

    static StandardMoveSet()
    {
        actions = new Dictionary<string, ActionInstance>();
        // Circle Attack 
        string id = "circle_attack";
        int priority = Setting.ACTION_ATTACK_PRIORITY;
        float cooldown = Setting.CD_CIRCLE_ATTACK;
        int duration = Setting.DURATION_CIRCLE_ATTACK;
        ActionInstance circleAttack = new ActionInstance(priority, id, CircleAttack, Exit, cooldown, duration);
        actions[id] = circleAttack;

        // Guard
        id = "guard";
        priority = Setting.ACTION_DEFEND_PRIORITY;
        cooldown = Setting.CD_CIRCLE_GUARD;
        duration = Setting.DURATION_GUARD;
        ActionInstance guard = new ActionInstance(priority, id, Guard, ExitGuard, cooldown, duration);
        actions[id] = guard;
    }

    internal StandardMoveSet(Entity e, Combat cc  = null) : base(e) {
        animationDestroyed = true;
        guardAnimation = null;
        if (cc == null) {
            cc = new Combat(e);
        }
        combatComponent = cc;
        actionInstances = new Dictionary<string, ActionInstance>();
        foreach (string key in actions.Keys)
        {
            actionInstances[key] = actions[key].Clone();
        }
        cc.AddComponent(Setting.COMPONENT_STD_MOVESET, this);
    }

    // Moveset..........................................................................................................
    private static void CircleAttack(Dictionary<string, object> args)
    {
        /* Damages entities in radius of attack range by attack damage, has no effect on the attacker
         * 
         * Cooldown: See settings
         * Arguments:
         * - ignore: Hashset<int> -> The set of ids of entities to ignore
         */
        StandardMoveSet attacker = (StandardMoveSet)args[Setting.HANDLING_COMPONENT];
        GameObject gameObject = attacker.entity.gameObject;
        Transform transform = gameObject.transform;
        float range = attacker.combatComponent.GetAttackRange();
        int damage = attacker.combatComponent.GetAttackDamage();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, range);
        GameObject g = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(range, 
            range, 1), Color.yellow, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        Object.Destroy(g, 0.25f);
        int id = attacker.entity.GetId();
        HashSet<int> ignore = null;
        if (args.ContainsKey("ignore")) {
            ignore = (HashSet<int>)args["ignore"];
        }

        foreach (Collider2D collider in colliders)
        {
            Entity collided = collider.gameObject.GetComponent<Entity>();
            if (!collider.CompareTag("entity") ){
                continue;
            }
            int cid = collided.GetId();
            if (ignore != null && ignore.Contains(cid))
            {
                continue;
            }
            if (cid != id)
            {
                AttackInfo attackInfo = new AttackInfo(id, damage);
                collided.RegisterDamage(attackInfo);
            }
        }
    }

    private static void Guard(Dictionary<string, object> args)
    {
        StandardMoveSet defender = (StandardMoveSet)args[Setting.HANDLING_COMPONENT];

        if (defender.GetIncomingDamage() > 0)
        {
            Debug.Log("Blocked");
        }
        defender.SetIncomingDamage(defender.GetIncomingDamage() - defender.combatComponent.GetDefense());
        if (defender.animationDestroyed)
        {
            defender.animationDestroyed = false;
            GameObject gameObject = defender.entity.gameObject;
            Transform transform = gameObject.transform;
            Color color = Color.blue;
            color.a = 0.5f;
            defender.guardAnimation = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(2, 2, 1), color, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        }
    }

    // Terminate Action
    private static void Exit(Dictionary<string, object> args) { } // For moves that don't require additional processing steps after termination
    private static void ExitGuard(Dictionary<string, object> args)
    {
        StandardMoveSet defender = (StandardMoveSet)args[Setting.HANDLING_COMPONENT];
        defender.animationDestroyed = true;
        Object.Destroy(defender.guardAnimation);
    }

    Dictionary<string, ActionInstance> IActionImplementor.AvailableActions()
    {
        return actionInstances;
    }

    ActionInstance IActionImplementor.GetAction(string actionName)
    {
        return actionInstances[actionName];
    }

    bool IActionImplementor.HasAction(string actionName)
    {
        return actionInstances.ContainsKey(actionName);
    }

    public IActionImplementor GetIdentifier(string actionName) {
        if (actionInstances.ContainsKey(actionName)) {
            return this;
        }
        return default;
    }
}
