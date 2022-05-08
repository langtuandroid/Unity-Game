using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour
{
    private static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
    
    private int id;
    private int health;
    private int incomingDamage;
    private AttackInfoSet damageHistory;

    // Start is called before the first frame update

    protected void Start()
    {
        health = 1;
        incomingDamage = 0;
        gameObject.tag = Setting.TAG_ENTITY;
        id = IdDistributor.GetId(Setting.ID_ENTITY);
        allEntities[id] = this;
        damageHistory = new AttackInfoSet();
        health = 100;
    }

    // Update is called once per frame
    protected void Update()
    {
        // Update damage history
        AttackInfoSet removing = new();
        foreach (AttackInfo a in damageHistory) {
            a.Countdown();
            if (a.Terminated()) { 
                removing.Add(a);
            }
        }
        foreach (AttackInfo a in removing) {
            damageHistory.Remove(a);
        }
    }

    protected void LateUpdate() {
        health -= incomingDamage;
        incomingDamage = 0;
        if (health <= 0)
        {
            Die();
        }
    }
    public int GetId() {
        return id;
    }

    public int NumOfEntities() { 
        return allEntities.Count;
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
        allEntities.Remove(id);
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

    public void RegisterDamage(AttackInfo a) {
        incomingDamage += a.GetAttackDamage();
        Debug.Log("Damage:" + incomingDamage);
        damageHistory.Add(a);
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