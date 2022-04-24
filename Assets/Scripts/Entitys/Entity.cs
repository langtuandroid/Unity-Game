using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
    
    private int id;
    private int health;
    private bool dead = false;
    private int incomingDamage;
    
    private Dictionary<string, EntityComponent> components = new Dictionary<string, EntityComponent>();

    internal abstract class EntityComponent
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
    }

    internal interface IEntityComponentUpdate { 
        public abstract void Update();
    }
    public static void UpdateEntityStatus() {
        Dictionary<int, Entity>.KeyCollection keys = allEntities.Keys;
        HashSet<int> dead = new HashSet<int>();
        foreach (int key in keys) {
            allEntities[key].UpdateStatus();
            if (allEntities[key].isDead()) {
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
        gameObject.tag = "entity";
        id = IdDistributor.GetId(Setting.ID_ENTITY);
        allEntities[id] =  this;
    }

    // Update is called once per frame
    protected void Update()
    {
        // Update components
        foreach (KeyValuePair<string, EntityComponent> kvp in components) {
            if (kvp.Value is IEntityComponentUpdate) {
                ((IEntityComponentUpdate)kvp.Value).Update();
            }
        }
       Movement();
    }
    protected abstract void Movement();
    public int getId() {
        return id;
    }

    void die() { 
        Destroy(gameObject);
        dead = true;
    }

    protected void SetHealth(int amount) { 
        health = amount;

    }

    public void RegisterDamage(AttackInfo a) {
        incomingDamage += a.attackDamage;
    }

    public bool isDead()
    {
        return dead;
    }
    
    private void UpdateStatus() {
        health -= incomingDamage;
        incomingDamage = 0;
        if (health <= 0)
        {
            die();
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