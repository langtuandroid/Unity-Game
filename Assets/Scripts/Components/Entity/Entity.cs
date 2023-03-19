using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using LobsterFramework.Utility.Groups;

namespace LobsterFramework.EntitySystem
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private List<EntityGroup> groups;
        [SerializeField] private RefInt startMaxHealth;
        [SerializeField] private RefInt startHealth;

        public UnityAction<bool> onActionBlocked;
        public UnityAction<bool> onMovementBlocked;

        [SerializeField] private List<DamageInfoKeeper> damageHistory;
        [SerializeField] public Dictionary<Type, Effect> activeEffects;
        public bool ActionBlocked { get; private set; }
        public bool MovementBlocked { get; private set; }

        [HideInInspector]
        [SerializeField] private int maxHealth;
        [HideInInspector]
        [SerializeField] private int health;

        private int incomingDamage;


        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }
            private set
            {
                if (value <= 0)
                {
                    maxHealth = 1;
                }
                else
                {
                    maxHealth = value;
                }
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
            private set
            {
                if (value > maxHealth)
                {
                    health = maxHealth;
                }
                else
                {
                    health = value;
                }
            }
        }

        public int IncomingDamage
        {
            get { return incomingDamage; }
            set
            {
                if (value < 0)
                {
                    incomingDamage = 0;
                    return;
                }
                incomingDamage = value;
            }
        }

        public bool IsDead
        {
            get;
            private set;
        }

        public DamageInfo LatestDamageInfo
        {
            get
            {
                if (damageHistory.Count > 0) { return damageHistory[damageHistory.Count - 1].info; }
                return DamageInfo.none;
            }
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
            damageHistory = new();
            activeEffects = new();
            ActionBlocked = false;
            MovementBlocked = false;
        }

        private void Update()
        {
            for (int i = damageHistory.Count - 1; i >= 0; i--)
            {
                if (damageHistory[i].CountDown())
                {
                    damageHistory.RemoveAt(i);
                }
            }
            List<Effect> removed = new();
            foreach (Effect effect in activeEffects.Values)
            {
                if (!effect.Update())
                {
                    removed.Add(effect);
                }
            }
            foreach (Effect effect in removed)
            {
                activeEffects.Remove(effect.GetType());
                Destroy(effect);
            }
        }

        private void LateUpdate()
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
            foreach (EntityGroup group in groups)
            {
                group.Remove(this);
            }
            damageHistory.Clear();
            foreach (Effect effect in activeEffects.Values)
            {
                Destroy(effect);
            }
            activeEffects.Clear();
        }

        private void OnEnable()
        {
            foreach (EntityGroup group in groups)
            {
                group.Add(this);
            }
        }

        public void RegisterEffect(Effect effect)
        {
            Type t = effect.GetType();
            if (activeEffects.ContainsKey(t))
            {
                return;
            }
            Effect eft = Instantiate(effect);
            activeEffects.Add(t, eft);
            eft.ActivateEffect(this);
        }

        public void RegisterDamage(int damage, Entity attacker = null)
        {
            incomingDamage += damage;
            Debug.Log("Damage:" + incomingDamage);

            if (attacker != null)
            {
                DamageInfo info = new DamageInfo() { damage = damage, attacker = attacker };
                damageHistory.Add(new DamageInfoKeeper(info));
            }
        }

        public void BlockMovement(bool value)
        {
            bool before = MovementBlocked;
            MovementBlocked = value;
            if (onMovementBlocked != null && before != value)
            {
                onMovementBlocked.Invoke(value);
            }
        }

        public void BlockAction(bool value)
        {
            bool before = ActionBlocked;
            ActionBlocked = value;
            if (onActionBlocked != null && before != value)
            {
                onActionBlocked.Invoke(value);
            }
        }

        [System.Serializable]
        private class DamageInfoKeeper
        {
            public DamageInfo info;
            private float counter;

            public DamageInfoKeeper(DamageInfo info)
            {
                this.info = info;
                counter = 0;
            }

            public bool CountDown()
            {
                counter += Time.deltaTime;
                return counter >= Setting.EXPIRE_ATTACK_TIME;
            }
        }
    }

    [System.Serializable]

    public struct DamageInfo
    {
        public int damage;
        public Entity attacker;
        public static DamageInfo none = new() { damage = 0, attacker = null };
    };
}