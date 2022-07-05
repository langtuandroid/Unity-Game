using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CombatComponents { 
    StandardMoveSet
}

[System.Serializable]
public class Combat : ActionImplementor
{
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private int defense;
    [SerializeField] private CombatComponentDictionary components;
    [SerializeReference] Actionable actionComponent;

    public Combat(Entity e)  : base(e)
    {
        attackDamage = 50;
        attackRange = 2f;
        defense = 25;
        components = new CombatComponentDictionary();
        actionComponent = e.GetComponent<Actionable>();
        actionComponent.AddComponent(ActionComponents.Combat, this);
    }

    public void AddComponent(CombatComponents name, ActionImplementor i) {
        components[name] = i; 
        actionComponent.UpdateActions(ActionComponents.Combat);
    }

    // Implements <IActionImplementor>......................................................................
    public override Dictionary<string, ActionInstance> AvailableActions() {
        Dictionary<string, ActionInstance> result = new Dictionary<string, ActionInstance>();
        if (components == null) {
            Debug.Log("");
        }
        foreach (ActionImplementor e in components.Values) {
            foreach (KeyValuePair<string, ActionInstance> a in e.AvailableActions()) {
                result[a.Key] = a.Value;
            }
        }
        return result;
    }

    public override ActionInstance GetAction(string actionName)
    {
        foreach (ActionImplementor a in components.Values) {
            if (a.HasAction(actionName)) {
                return a.GetAction(actionName);
            }
        }
        return default;
    }

    public override bool HasAction(string actionName)
    {
        foreach (ActionImplementor a in components.Values)
        {
            if (a.HasAction(actionName))
            {
                return true;
            }
        }
        return false;
    }

    public override ActionImplementor GetIdentifier(string actionName) {
        if (components == null)
        {
            Debug.Log("");
        }
        foreach (ActionImplementor a in components.Values)
        {
            ActionImplementor r = a.GetIdentifier(actionName);
            if (r != default)
            {
                return r;
            }
        }
        return default;
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

    public int GetAttackDamage() {
        return attackDamage;
    }

    public int GetDefense() { 
        return defense;
    }

    public float GetAttackRange() {
        return attackRange;
    }
}

public class CombatSubComponent { 
    public Entity entity;
    public CombatSubComponent(Entity entity) {
        this.entity = entity;
    }
}
