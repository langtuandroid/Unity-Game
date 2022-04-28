using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
    
    private int id;
    private int health;
    private bool dead = false;
    private int incomingDamage;
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
    public static void UpdateEntityStatus() {
        Dictionary<int, Entity>.KeyCollection keys = allEntities.Keys;
        HashSet<int> dead = new HashSet<int>();
        foreach (int key in keys) {
            allEntities[key].UpdateStatus();
            if (allEntities[key].IsDead()) {
                dead.Add(key);
            }
        }
        foreach (int key in dead)
        {
            allEntities.Remove(key);
        }
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
       Movement();
    }
    protected abstract void Movement();
    public int GetId() {
        return id;
    }

    void Die() { 
        Destroy(gameObject);
        dead = true;
    }

    protected void SetHealth(int amount) { 
        health = amount;
    }

    public void RegisterDamage(AttackInfo a) {
        incomingDamage += a.attackDamage;
    }

    public bool HasTag(string tag) {
        return customTags.Contains(tag);
    }

    public bool IsDead()
    {
        return dead;
    }
    
    private void UpdateStatus() {
        health -= incomingDamage;
        incomingDamage = 0;
        if (health <= 0)
        {
            Die();
        }
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