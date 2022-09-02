using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsUpdate))]
[System.Serializable]
public class Entity : MonoBehaviour
{
    [SerializeField] private List<EntityGroup> groups;
    [SerializeField] private RefInt startMaxHealth;
    [SerializeField] private RefInt startHealth;

    [HideInInspector]
    [SerializeField] private int maxHealth;
    [HideInInspector]
    [SerializeField] private int health;
    private int incomingDamage;
    
    public int MaxHealth { 
        get {
            return maxHealth;
        } 
        private set {
            if (value <= 0)
            {
                maxHealth = 1;
            }
            else { 
                maxHealth = value;
            }   
        }
    }
  
    public int Health { 
        get { 
            return health;
        }
        private set {
            if (value > maxHealth)
            {
                health = maxHealth;
            }
            else { 
                health = value;
            }
        }
    }

    public int IncomingDamage { get { return incomingDamage; } 
        set {
            if (value < 0)
            {
                return;
            }
            incomingDamage = value;
        } 
    }

    private void Awake()
    {
        foreach (EntityGroup group in groups)
        {
            group.Add(this);
        }

        incomingDamage = 0;
        MaxHealth = startMaxHealth.Value;
        Health = startHealth.Value;
        gameObject.tag = Setting.TAG_ENTITY;
    }

    protected void LateUpdate()
    {
        Health -= incomingDamage;
        incomingDamage = 0;
        if (Health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        foreach (EntityGroup group in groups) {
            group.Remove(this);
        }
    }

    public void RegisterDamage(int damage)
    {
        incomingDamage += damage;
        Debug.Log("Damage:" + incomingDamage);
    }
}