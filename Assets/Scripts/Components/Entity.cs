using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(PhysicsUpdate))]
public class Entity : MonoBehaviour
{
    public static BidirectionalMap<int, Entity> AllEntities = new();
    public static IdDistributor distributor = new();
    private int id;
    private int health;
    private int incomingDamage;

    // Start is called before the first frame update

    protected void Start()
    {
        health = 1;
        incomingDamage = 0;
        gameObject.tag = Setting.TAG_ENTITY;
        id = distributor.GetID();
        AllEntities[id] = this;
        health = 100;
    }

    // Update is called once per frame
    protected void Update()
    {
    }

    protected void LateUpdate() {
        health -= incomingDamage;
        incomingDamage = 0;
        if (health <= 0)
        {
            Die();
        }
    }
    public int ID {
        get { return id; }
    }

    public int GetIncomingDamage() { 
        return incomingDamage;
    }

    public void SetIncomingDamage(int d) {
        if (d < 0) {
            return;
        }
        incomingDamage = d;
    }

    void Die() {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        AllEntities.RemoveValue(this);
    }

    public virtual void OnKillEntity(int id, bool killingBlow) {
        if (killingBlow) {
            Debug.Log("Killed Entity: " + id);
        }
    }

    public void SetHealth(int amount) { 
        health = amount;
    }

    public int GetHealth() { 
        return health;
    }

    public void RegisterDamage(int damage) {
        incomingDamage += damage;
        Debug.Log("Damage:" + incomingDamage);
    }
}