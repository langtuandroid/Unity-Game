using System.Collections.Generic;
using UnityEngine;

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
                incomingDamage = 0;
                return;
            }
            incomingDamage = value;
        } 
    }

    public bool IsDead {
        get;
        private set;
    }

    private void Start()
    {
        foreach (EntityGroup group in groups)
        {
            group.Add(this);
        }

        incomingDamage = 0;
        MaxHealth = startMaxHealth.Value;
        Health = startHealth.Value;
        gameObject.tag = Setting.TAG_ENTITY;
        IsDead = false;
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

    public void Reset()
    {
        incomingDamage = 0;
        MaxHealth = startMaxHealth.Value;
        Health = startHealth.Value;
        IsDead = false;
        gameObject.SetActive(true);
    }

    public void Die()
    {
        IsDead = true;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (EntityGroup group in groups) {
            group.Remove(this);
        }
    }

    private void OnEnable()
    {
        foreach (EntityGroup group in groups)
        {
            group.Add(this);
        }
    }

    public void RegisterDamage(int damage)
    {
        incomingDamage += damage;
        Debug.Log("Damage:" + incomingDamage);
    }
}