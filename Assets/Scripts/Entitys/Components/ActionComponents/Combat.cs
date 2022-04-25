using System.Collections;
using System.Collections.Generic;
using UnityEngine;
internal class Combat : Entity.EntityComponent, IActionImplementor
{
    private int attackDamage;
    private float attackRange;
    private int defense;

    private static readonly Dictionary<string, ActionInstance> actions;
    private Dictionary<string, ActionInstance> actionInstances;
    private GameObject guardAnimation;
    private bool animationDestroyed;
    
    static Combat() { 
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

    public Combat(Entity e) : base(e)
    {
        attackDamage = 0;
        attackRange = 2f;
        defense = 0;
        animationDestroyed = true;
        guardAnimation = null;
        Action actionComponent;
        if (!e.HasEntityComponent("Action"))
        {
            actionComponent = new Action(e);
            e.SetEntityComponent("Action", actionComponent);
        }
        else { 
            actionComponent = (Action)e.GetEntityComponent("Action");
        }
        actionInstances = new Dictionary<string, ActionInstance>();
        foreach (string key in actions.Keys)
        {
            actionInstances[key] = actions[key].Clone();
        }
        actionComponent.AddComponent("Combat", this);
    }

    // Implements <IActionImplementor>......................................................................
    public Dictionary<string, ActionInstance> AvailableActions() { 
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

    public void Countdown() {
        Dictionary<string, ActionInstance>.KeyCollection keys = actionInstances.Keys;
        foreach (string key in keys) {
            ActionInstance info = actionInstances[key];
            info.CountDown();
        }
    }

    // Setter....................................................................................................................
    public void SetAttackDamage(int damage)
    {
        if (damage < 0)
        {
            damage = 0;
        }
        attackDamage = damage;
    }

    public void SetDefense(int d) {
        if (d < 0) { 
            d = 0;
        }
        defense = d;
    }


    // Moveset..........................................................................................................
    private static void CircleAttack(Dictionary<string, object> args)
    {
        Combat attacker = (Combat)args["_component"];
        GameObject gameObject = attacker.entity.gameObject;
        Transform transform = gameObject.transform; 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, attacker.attackRange);
        GameObject g = ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(attacker.attackRange, attacker.attackRange, 1), Color.yellow, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        Object.Destroy(g, 0.25f);
        int id = attacker.entity.GetId();

        foreach (Collider2D collider in colliders)
        {
            if (!collider.CompareTag("entity"))
            {
                continue;
            }
            Entity collided = collider.gameObject.GetComponent<Entity>();
            if (collided.GetId() != id)
            {
                AttackInfo attackInfo = new AttackInfo(id, attacker.attackDamage);
                collided.RegisterDamage(attackInfo);
            }
        }
    }

    private static void Guard(Dictionary<string, object> args) {
        Combat defender = (Combat)args["_component"];
        if (defender.GetIncomingDamage() > 0) {
            Debug.Log("Blocked");
        }
        defender.SetIncomingDamage(defender.GetIncomingDamage() - defender.defense);
        if (defender.animationDestroyed) {
            defender.animationDestroyed = false;
            GameObject gameObject = defender.entity.gameObject;
            Transform transform = gameObject.transform;
            Color color = Color.blue;
            color.a = 0.5f;
            defender.guardAnimation =  ObjectGenerator.SpriteCircle(transform, transform.position, transform.rotation, new Vector3(2, 2, 1), color, LayerMask.NameToLayer("Default"), SortingLayer.NameToID("Default"));
        }
    }

    // Terminate Action
    private static void Exit(Dictionary<string, object> args) { } // For moves that don't require additional processing steps after termination
    private static void ExitGuard(Dictionary<string, object> args) {
        Combat defender = (Combat)args["_component"];
        defender.animationDestroyed = true;
        Object.Destroy(defender.guardAnimation);
    }
}

public struct AttackInfo
{
    public int id;     // Attacker
    public int attackDamage;

    public AttackInfo(int id, int attackDamage)
    {
        this.id = id;
        this.attackDamage = attackDamage;
    }
}
