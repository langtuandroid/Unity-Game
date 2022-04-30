using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
    
    private int id;
    private int health;
    private bool dead = false;
    private int incomingDamage;
    private HashSet<AttackInfo> damageHistory;

    protected HashSet<string> customTags;
    
    
    private Dictionary<string, EntityComponent> components = new Dictionary<string, EntityComponent>();

    public abstract class EntityComponent
    {
        public Entity entity;
        public EntityComponent(Entity e)
        {
            entity = e;
        }

        protected int GetIncomingDamage() { 
            return entity.incomingDamage;
        }

        protected void SetIncomingDamage(int incomingDamage) {
            if (incomingDamage < 0) {
                incomingDamage = 0;
            }
            entity.incomingDamage = incomingDamage;
        }

        public void AddTag(string tag)
        {
            entity.customTags.Add(tag);
        }

        public bool RemoveTag(string tag)
        {
            return entity.customTags.Remove(tag);
        }
    }

    internal interface IEntityComponentUpdate { 
        public abstract void Update();
    }

    // Start is called before the first frame update

    protected void Start()
    {
        health = 1;
        incomingDamage = 0;
        gameObject.tag = Setting.TAG_ENTITY;
        id = IdDistributor.GetId(Setting.ID_ENTITY);
        allEntities[id] = this;
        customTags = new HashSet<string>();
        damageHistory = new HashSet<AttackInfo>();
    }

    // Update is called once per frame
    protected void Update()
    {
        // Update components
        foreach (KeyValuePair<string, EntityComponent> kvp in components) {
            if (kvp.Value is IEntityComponentUpdate i) {
                i.Update();
            }
        }
        // Update damage history
        HashSet<AttackInfo> removing = new HashSet<AttackInfo>();
        foreach (AttackInfo a in damageHistory) {
            a.Countdown();
            if (a.Terminated()) { 
                removing.Add(a);
            }
        }
        foreach (AttackInfo a in removing) {
            damageHistory.Remove(a);
        }
       Movement();
    }
    protected abstract void Movement();
    public int GetId() {
        return id;
    }

    void Die() { 
        Destroy(gameObject);
        dead = true;
        int i = 0;
        HashSet<int> sent = new HashSet<int>();
        foreach (AttackInfo a in damageHistory) {
            int attacker = a.GetAttackerId();
            if (sent.Contains(attacker)) {
                continue;
            }
            if (i == 0)
            {
                allEntities[attacker].OnKillEntity(id, true);
            }
            else { 
                allEntities[attacker].OnKillEntity(id, false);
            }
            sent.Add(attacker);
            i++;
        }
    }

    public virtual void OnKillEntity(int id, bool killingBlow) { }

    protected void SetHealth(int amount) { 
        health = amount;
    }

    public void RegisterDamage(AttackInfo a) {
        incomingDamage += a.GetAttackDamage();
        damageHistory.Add(a);
    }

    public bool HasTag(string tag) {
        return customTags.Contains(tag);
    }

    public bool IsDead()
    {
        return dead;
    }
    
    private void LateUpdate() {
        health -= incomingDamage;
        incomingDamage = 0;
        if (health <= 0)
        {
            Die();
        }
        allEntities.Remove(id);
    }

    public bool HasEntityComponent(string name) {
        return components.ContainsKey(name);
    }

    internal EntityComponent GetEntityComponent(string name) {
        return components[name];
    }

    internal void SetEntityComponent(string name, EntityComponent component) {
        components[name] = component;
    }
}

public class AttackInfo
{
    private int id;     // Attacker
    private int attackDamage;
    private float duration;
    private float counter;

    public AttackInfo(int id, int attackDamage)
    {
        this.id = id;
        this.attackDamage = attackDamage;
        duration = Setting.EXPIRE_ATTACK_TIME;
        counter = 0;
    }

    public void Countdown()
    {
        counter += Time.deltaTime;
    }

    public bool Terminated()
    {
        return counter >= duration;
    }

    public int GetAttackerId()
    {
        return id;
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}