using System.Collections;
using System.Collections.Generic;
using UnityEngine;
internal class Combat : Entity.EntityComponent, IActionImplementor
{
    private int attackDamage;
    private float attackRange;
    private int defense;
    private Dictionary<string, IActionImplementor> components;
    Action actionComponent;

    public Combat(Entity e) : base(e)
    {
        attackDamage = 0;
        attackRange = 2f;
        defense = 0;
        components = new Dictionary<string, IActionImplementor>();
        if (!e.HasEntityComponent(Setting.COMPONENT_ACTION))
        {
            actionComponent = new Action(e);
        }
        else { 
            actionComponent = (Action)e.GetEntityComponent(Setting.COMPONENT_ACTION);
        }
        
        actionComponent.AddComponent(Setting.COMPONENT_COMBAT, this);
    }

    public void AddComponent(string name, IActionImplementor i) {
        components[name] = i; 
        actionComponent.UpdateActions(Setting.COMPONENT_COMBAT);
    }

    // Implements <IActionImplementor>......................................................................
    public Dictionary<string, ActionInstance> AvailableActions() {
        Dictionary<string, ActionInstance> result = new Dictionary<string, ActionInstance>();
        foreach (IActionImplementor e in components.Values) {
            foreach (KeyValuePair<string, ActionInstance> a in e.AvailableActions()) {
                result[a.Key] = a.Value;
            }
        }
        return result;
    }

    ActionInstance IActionImplementor.GetAction(string actionName)
    {
        foreach (IActionImplementor a in components.Values) {
            if (a.HasAction(actionName)) {
                return a.GetAction(actionName);
            }
        }
        return default;
    }

    bool IActionImplementor.HasAction(string actionName)
    {
        foreach (IActionImplementor a in components.Values)
        {
            if (a.HasAction(actionName))
            {
                return true;
            }
        }
        return false;
    }

    public IActionImplementor GetIdentifier(string actionName) {
        foreach (IActionImplementor a in components.Values)
        {
            IActionImplementor r = a.GetIdentifier(actionName);
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
