using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StandardMoveSet : ActionImplementor
{
    private static readonly Dictionary<string, ActionInstance> actions;

    private GameObject guardAnimation;
    [SerializeReference] private Combat combatComponent;
    [SerializeField] private ActionInstanceDictionary actionInstances;

    static StandardMoveSet()
    {
        actions = new Dictionary<string, ActionInstance>();
        // Circle Attack 
        string id = Setting.STD_CIRCLE_ATTACK;
        int priority = Setting.ACTION_ATTACK_PRIORITY;
        float cooldown = Setting.CD_CIRCLE_ATTACK;
        int duration = Setting.DURATION_CIRCLE_ATTACK;
        ActionInstance circleAttack = new ActionInstance(priority, id, Setting.STD_CIRCLE_ATTACK, cooldown, duration);
        actions[id] = circleAttack;

        // Guard
        id = Setting.STD_GUARD;
        priority = Setting.ACTION_DEFEND_PRIORITY;
        cooldown = Setting.CD_CIRCLE_GUARD;
        duration = Setting.DURATION_GUARD;
        ActionInstance guard = new ActionInstance(priority, id, Setting.STD_GUARD, cooldown, duration);
        actions[id] = guard;
    }

    public StandardMoveSet(Entity e, Combat cc  = null) : base(e) {
        guardAnimation = null;
        if (cc == null) {
            cc = new Combat(e);
        }
        combatComponent = cc;
        actionInstances = new();
        foreach (string key in actions.Keys)
        {
            actionInstances[key] = actions[key].Clone();
        }
        cc.AddComponent(CombatComponents.StandardMoveSet, this);
    }

    // Moveset..........................................................................................................
    internal static void CircleAttack(Dictionary<string, object> args)
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, (float)(range / 2));
        GameObject g = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(range, 
            range, 1), Color.yellow, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        Object.Destroy(g, 0.25f);
        int id = attacker.entity.GetId();
        HashSet<int> ignore = null;
        if (args.ContainsKey(Setting.IGNORE_TARGET)) {
            ignore = (HashSet<int>)args[Setting.IGNORE_TARGET];
        }
        foreach (Collider2D collider in colliders)
        {
            Entity collided = collider.gameObject.GetComponent<Entity>();
            if (!collider.CompareTag(Setting.TAG_ENTITY) ){
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

    internal static void Guard(Dictionary<string, object> args)
    {
        StandardMoveSet defender = (StandardMoveSet)args[Setting.HANDLING_COMPONENT];
        if (defender.entity.GetIncomingDamage() > 0)
        {
            Debug.Log("Blocked: " + defender.combatComponent.GetDefense());
        }
        defender.entity.SetIncomingDamage(defender.entity.GetIncomingDamage() - defender.combatComponent.GetDefense());
        if (defender.guardAnimation == null)
        {
            GameObject gameObject = defender.entity.gameObject;
            Transform transform = gameObject.transform;
            Color color = Color.blue;
            color.a = 0.5f;
            defender.guardAnimation = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(2, 2, 1), color, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        }
    }

    // Terminate Action
    internal static void Exit(Dictionary<string, object> args) { } // For moves that don't require additional processing steps after termination
    internal static void ExitGuard(Dictionary<string, object> args)
    {
        StandardMoveSet defender = (StandardMoveSet)args[Setting.HANDLING_COMPONENT];
        Object.Destroy(defender.guardAnimation);
        defender.guardAnimation = null;
    }

    public override Dictionary<string, ActionInstance> AvailableActions()
    {
        return actionInstances;
    }

    public override ActionInstance GetAction(string actionName)
    {
        return actionInstances[actionName];
    }

    public override bool HasAction(string actionName)
    {
        return actionInstances.ContainsKey(actionName);
    }

    public override ActionImplementor GetIdentifier(string actionName) {
        if (actionInstances.ContainsKey(actionName)) {
            return this;
        }
        return default;
    }
}
