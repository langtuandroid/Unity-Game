using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using LobsterFramework.Utility.Groups;
using LobsterFramework.Utility.BufferedStats;
using Pathfinding.Util;

namespace LobsterFramework.EntitySystem
{
    /// <summary>
    /// Character class of the game. Provides an interface for health/damage system. Entities will be disabled
    /// when their health goes down to or below 0.
    /// </summary>
    [AddComponentMenu("Entity")]
    public class Entity : MonoBehaviour
    {
        #region Fields
        [Header("Group Setting")]
        [SerializeField] private List<EntityGroup> groups;

        #region Health/PostureFields
        [Header("Character Stats")]
        [SerializeField] private RefFloat maxHealth;
        [SerializeField] private RefFloat baseHealthRegen;
        [SerializeField] private RefFloat lowHealthRegen;
        [SerializeField] private RefFloat lowHealthThreshold;
        [SerializeField] private RefFloat startHealth;
        [SerializeField] private RefFloat maxPosture;
        [SerializeField] private RefFloat basePostureRegen;

        private float health;
        private float posture;

        public float MaxHealth { get { return maxHealth.Value; } }
        public float MaxPosture { get { return maxPosture.Value; } }
        public bool RegenSuppressed { get { return (Time.time - damagedSince) < GameManager.Instance.SUPPRESS_REGEN_DURATION; } }

        public float Health
        {
            get { return health; }
            private set { if (value > MaxHealth) { health = MaxHealth; } else { health = value; } }
        }

        public float Posture
        {
            get { return posture; }
            private set { if (value > MaxPosture) { posture = MaxPosture; } else { posture = value; } }
        }

        public UnityAction<Damage> onDamaged;
        public UnityAction<bool> onPostureStatusChange;

        private int pbHealthDamageModifierKey;

        public bool IsDead { get; private set; }
        public bool PostureBroken { get; private set; }
        private float pbCounter;
        #endregion

        #region StatusFields
        [Header("Status")]
        [SerializeField] private List<DamageTracker> damageHistory = new();
        [SerializeField] public Dictionary<Type, Effect> activeEffects = new();

        private float damagedSince;
        public Damage LatestDamageInfo
        {
            get
            {
                if (damageHistory.Count > 0) { return damageHistory[damageHistory.Count - 1].info; }
                return EntitySystem.Damage.none;
            }
        }
        #endregion
        // Buffers
        private RegenBuffer regenBuffer = new(true);
        private DamageBuffer damageBuffer = new(true);

        #endregion
        
        #region StatusUpdate
        private void Start()
        { 
            Health = startHealth.Value;
            IsDead = false;
            PostureBroken = false;
            Posture = MaxPosture;
            damagedSince = Time.time;

            regenBuffer.AddHealth(baseHealthRegen.Value);
            regenBuffer.AddPosture(basePostureRegen.Value);
        }
        

        private void Update()
        {
            Health -= damageBuffer.HealthDamage;
            posture -= damageBuffer.PostureDamage;
            damageBuffer.ResetDamage();
            damageBuffer.ResetDefense();
            if (Health <= 0) { Die(); }
            if (!PostureBroken && Posture <= 0) { PostureBreak(); }

            if (PostureBroken) {
                pbCounter += Time.deltaTime;
                if (pbCounter >= GameManager.Instance.POSTURE_BROKEN_DURATION) { 
                    PostureRecover();
                }
            }

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
            Regen();
        }

        /// <summary>
        /// Apply regeneration effects to health and posture
        /// </summary>
        private void Regen() {
            if (!RegenSuppressed)
            {
                if (Health < lowHealthThreshold)
                {
                    Health += lowHealthRegen * regenBuffer.HealthModifier * Time.deltaTime;
                    if (Health > lowHealthThreshold)
                    {
                        Health = lowHealthThreshold.Value;
                    }
                }
                Health += regenBuffer.HealthRegen * Time.deltaTime;
                Posture += regenBuffer.PostureRegen * Time.deltaTime;
            }
        }

        /// <summary>
        /// Reset the entity to its full health and posture and clear all of the damage and effects
        /// </summary>
        public void ResetStatus()
        {
            damageBuffer.Reset();
            if (PostureBroken)
            {
                PostureRecover();
            }
            Health = startHealth.Value;
            Posture = MaxPosture;

            foreach (Effect effect in activeEffects.Values) {
                effect.TerminateEffect();
                Destroy(effect);
            }
            activeEffects.Clear();
            
            IsDead = false;
            gameObject.SetActive(true);
        }

        private void Die()
        {
            IsDead = true;
            gameObject.SetActive(false);
        }

        private void PostureBreak() {
            pbHealthDamageModifierKey = damageBuffer.AddHealthModifier(GameManager.Instance.POSTURE_BROKEN_DAMAGE_MODIFIER);
            pbCounter = 0;
            PostureBroken = true;
            if (onPostureStatusChange != null) { 
                onPostureStatusChange.Invoke(true);
            }
        }

        private void PostureRecover() {
            damageBuffer.RemoveHealthModifier(pbHealthDamageModifierKey);
            Posture = 0.7f * MaxPosture;
            PostureBroken = false;
            if (onPostureStatusChange != null)
            {
                onPostureStatusChange.Invoke(false);
            }
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
        #endregion

        #region ApplicationMethods
        /// <summary>
        /// Apply an effect to the entity
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffect(Effect effect)
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

        /// <summary>
        /// Add damage to the entity, the damage will be dealt on update of next frame
        /// </summary>
        /// <param name="health">The amount of health damage</param>
        /// <param name="posture">The amount of posture damage</param>
        /// <param name="source">The source of this attack</param>
        /// <param name="type">The type of the damage</param>
        public void Damage(float health, float posture, Entity source = null, DamageType type = DamageType.General)
        {
            Damage damage = new Damage() { health = health, posture = posture, source = source, type = type};
            damageBuffer.AddDamage(damage);
            damageHistory.Add(new DamageTracker(damage));
            if (onDamaged != null) {
                onDamaged.Invoke(damage);
            }
            damagedSince = Time.time;
        }

        public void Damage(Damage damage)
        {
            damageBuffer.AddDamage(damage);
            damageHistory.Add(new DamageTracker(damage));
            if (onDamaged != null)
            {
                onDamaged.Invoke(damage);
            }
            damagedSince = Time.time;
        }

        /// <summary>
        /// Add a flat damage reduction to the entity, the damage taken will be reduced by the specified amount on update of next frame
        /// </summary>
        /// <param name="healthDefense">The amount of health defense</param>
        /// <param name="postureDefense">The amount of posture defense</param>
        public void Defense(int healthDefense, int postureDefense) { 
            damageBuffer.AddDefense(healthDefense, postureDefense);
        }

        #endregion
        
        [System.Serializable]
        private class DamageTracker
        {
            public Damage info;
            private float counter;

            public DamageTracker(Damage info)
            {
                this.info = info;
                counter = 0;
            }

            public bool CountDown()
            {
                counter += Time.deltaTime;
                return counter >= GameManager.Instance.EXPIRE_ATTACK_TIME;
            }
        }
    }

    #region Utility Structs
    public enum DamageType { 
        WeaponHit,
        WeaponDeflect,
        StatusEffect,
        General,
        None
    }

    [System.Serializable]

    public struct Damage
    {
        public float health;
        public float posture;
        public Entity source;
        public DamageType type;
         
        public static Damage none = new() { health = 0, posture = 0, source = null, type = DamageType.None };

        public override bool Equals(object obj)
        {
            return obj is Damage && (Damage)obj == this;
        } 

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Damage operator +(Damage a, Damage b)
        {
            return new() { posture = a.posture + b.posture, health = a.health + b.health, source = a.source };
        }

        public static Damage operator *(Damage a, Damage b)
        {
            return new() { posture = a.posture * b.posture, health = a.health * b.health, source = a.source };
        }

        public static Damage operator *(Damage a, float b) {
            return new() { posture = a.posture * b, health = a.health * b, source = a.source };
        }

        public static bool operator ==(Damage a, Damage b) { 
            return a.health == b.health && a.posture == b.posture && a.source == b.source && a.type == b.type;
        }
        public static bool operator !=(Damage a, Damage b)
        {
            return a.health != b.health || a.posture != b.posture || a.source != b.source || a.type != b.type;
        }
    }
    public struct DamageBuffer {
        private FloatSum healthDamage;
        private FloatSum postureDamage;

        private FloatSum healthDefense;
        private FloatSum postureDefense;

        private FloatProduct hdModifier;
        private FloatProduct pdModifier;

        public DamageBuffer(bool defaultSetting=true) { 
            healthDamage = new(0, true);
            postureDamage = new(0, true);

            healthDefense = new(0, true);
            postureDefense = new(0, true);

            hdModifier = new(1, true);
            pdModifier = new(1, true);
            HealthDamage = 0;
            PostureDamage = 0;
        }

        public float HealthDamage {get; private set; }

        public float PostureDamage {get;private set; }

        private void ComputeDamage() {
            HealthDamage = Math.Max(healthDamage.Stat - healthDefense.Stat, 0) * hdModifier.Stat;
            PostureDamage = Math.Max(postureDamage.Stat - postureDefense.Stat, 0) * pdModifier.Stat;
        }

        public void AddDamage(Damage damage) { 
            healthDamage.AddEffector(damage.health);
            postureDamage.AddEffector(damage.posture);
            ComputeDamage();
        }

        public void AddDefense(int healthDefense, int postureDefense) {
            if (healthDefense > 0) {this.healthDefense.AddEffector(healthDefense);}
            if (postureDefense > 0) {this.postureDefense.AddEffector(postureDefense);}
            ComputeDamage();
        }

        #region Health/Posture Modifiers
        public int AddHealthModifier(float factor) {
           int key = hdModifier.AddEffector(factor);
           ComputeDamage();
           return key;
        }

        public bool RemoveHealthModifier(int key)
        {
            bool result = hdModifier.RemoveEffector(key);
            ComputeDamage();
            return result;
        }

        public int AddPostureModifier(float factor)
        {
            int key = pdModifier.AddEffector(factor);
            ComputeDamage();
            return key;
        }
        public bool RemovePostureModifier(int key)
        {
            bool result = pdModifier.RemoveEffector(key);
            ComputeDamage();
            return result;
        }
        #endregion
        #region Reset
        public void ResetDamage() {
            healthDamage.ClearEffectors();
            postureDamage.ClearEffectors();
            ComputeDamage();
        }

        public void ResetDefense() {
            healthDefense.ClearEffectors();
            postureDefense.ClearEffectors();
            ComputeDamage();
        }

        public void ResetModifiers() {
            hdModifier.ClearEffectors();
            pdModifier.ClearEffectors();
            ComputeDamage();
        }

        public void Reset() { 
            healthDamage.ClearEffectors();
            postureDamage.ClearEffectors();

            healthDefense.ClearEffectors();
            postureDefense.ClearEffectors();

            hdModifier.ClearEffectors();
            pdModifier.ClearEffectors();
            ComputeDamage();
        }
        #endregion
    }

    public struct RegenBuffer
    {
        private FloatSum healthRegen;
        private FloatSum postureRegen;

        private FloatProduct hrModifier;
        private FloatProduct prModifier;

        public RegenBuffer(bool defaultSetting = true)
        {
            healthRegen = new(0, true);
            postureRegen = new(0, true);

            hrModifier = new(1, true);
            prModifier = new(1, true);
            HealthRegen = 0;
            PostureRegen = 0;
            HealthModifier = 1;
            PostureModifier = 1;
        }

        public float HealthRegen { get; private set; }

        public float PostureRegen { get; private set; }
        public float PostureModifier { get; private set; }
        public float HealthModifier { get; private set; }

        private void ComputeRegen()
        {
            HealthRegen = healthRegen.Stat * hrModifier.Stat;
            PostureRegen = postureRegen.Stat * prModifier.Stat;
            PostureModifier = prModifier.Stat;
            HealthModifier = hrModifier.Stat;
        }

        public void AddHealth(float health)
        {
            healthRegen.AddEffector(health);
            ComputeRegen();
        }

        public void AddPosture(float posture) { 
            postureRegen.AddEffector(posture);
            ComputeRegen();
        }

        public int AddHealthModifier(float factor)
        {
            int key = hrModifier.AddEffector(factor);
            ComputeRegen();
            return key;
        }

        public bool RemoveHealthModifier(int key)
        {
            bool result = hrModifier.RemoveEffector(key);
            ComputeRegen();
            return result;
        }

        public int AddPostureModifier(float factor)
        {
            int key = prModifier.AddEffector(factor);
            ComputeRegen();
            return key;
        }

        public bool RemovePostureModifier(int key)
        {
            bool result = prModifier.RemoveEffector(key);
            ComputeRegen();
            return result;
        }

        public void ResetRegen()
        {
            healthRegen.ClearEffectors();
            postureRegen.ClearEffectors();
            ComputeRegen();
        }

        public void ResetModifiers()
        {
            hrModifier.ClearEffectors();
            prModifier.ClearEffectors();
            ComputeRegen();
        }

        public void Reset()
        {
            healthRegen.ClearEffectors();
            postureRegen.ClearEffectors();

            hrModifier.ClearEffectors();
            prModifier.ClearEffectors();
            ComputeRegen();
        }

    }
}
#endregion