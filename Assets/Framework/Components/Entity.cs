using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using LobsterFramework.Utility;
using static Animancer.Validate;
using Codice.Client.Common.GameUI;

namespace LobsterFramework
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

        private BufferedValueAccessor<float> pbHealthDamageModifierAccessor;

        public bool IsDead { get; private set; }
        public bool PostureBroken { get; private set; }
        private float pbCounter;
        #endregion

        #region StatusFields
        [Header("Status")]
        [SerializeField] private List<DamageTracker> damageHistory = new();
        

        private float damagedSince;
        public Damage LatestDamageInfo
        {
            get
            {
                if (damageHistory.Count > 0) { return damageHistory[damageHistory.Count - 1].info; }
                return LobsterFramework.Damage.none;
            }
        }
        #endregion
        // Buffers
        private RegenBuffer regenBuffer = new();
        private DamageBuffer damageBuffer = new();
        private BufferedValueAccessor<float> baseHealthRegenAccessor;
        private BufferedValueAccessor<float> basePostureRegenAccessor;

        #endregion
        
        #region StatusUpdate
        private void Start()
        { 
            Health = startHealth.Value;
            IsDead = false;
            PostureBroken = false;
            Posture = MaxPosture;
            damagedSince = Time.time;

            baseHealthRegenAccessor = regenBuffer.healthRegen.GetAccessor();
            baseHealthRegenAccessor.Acquire(baseHealthRegen.Value);

            basePostureRegenAccessor = regenBuffer.postureRegen.GetAccessor();
            basePostureRegenAccessor.Acquire(basePostureRegen.Value);

            pbHealthDamageModifierAccessor = damageBuffer.healthDamageModifier.GetAccessor();
        }
        

        private void Update()
        {
            Health -= damageBuffer.HealthDamage;
            posture -= damageBuffer.PostureDamage;
            damageBuffer.Reset();
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
            
            Regen();
            Damage healing = regenBuffer.Heal;
            regenBuffer.Reset();
            Health += healing.health;
            Posture += healing.posture;
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
                Health += regenBuffer.HealthRegen * regenBuffer.HealthModifier * Time.deltaTime;
                Posture += regenBuffer.PostureRegen * regenBuffer.PostureModifier * Time.deltaTime;
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
            
            IsDead = false;
            gameObject.SetActive(true);
        }

        private void Die()
        {
            IsDead = true;
            gameObject.SetActive(false);
        }

        private void PostureBreak() {
            pbHealthDamageModifierAccessor.Acquire(GameManager.Instance.POSTURE_BROKEN_DAMAGE_MODIFIER);
            pbCounter = 0;
            PostureBroken = true;
            if (onPostureStatusChange != null) { 
                onPostureStatusChange.Invoke(true);
            }
        }

        private void PostureRecover() {
            pbHealthDamageModifierAccessor.Release();
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
        public void Defense(Damage defense) {
            damageBuffer.AddDefense(defense);
        }
        public void Heal(Damage heal) {
            regenBuffer.AddHeal(heal);
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
    internal class DamageBuffer {
        private readonly List<Damage> damages;
        private readonly List<Damage> defenses;

        public readonly FloatProduct healthDamageModifier;
        public readonly FloatProduct postureDamageModifier;

        public DamageBuffer() {
            damages = new();
            defenses = new();

            healthDamageModifier = new(1, true);
            postureDamageModifier = new(1, true);
            HealthDamage = 0;
            PostureDamage = 0;
            healthDamageModifier.onValueChanged += ComputeDamage;
            postureDamageModifier.onValueChanged += ComputeDamage;
        }

        ~DamageBuffer() {
            healthDamageModifier.onValueChanged -= ComputeDamage;
            postureDamageModifier.onValueChanged -= ComputeDamage;
        }

        public float HealthDamage {get; private set; }

        public float PostureDamage {get;private set; }

        private void ComputeDamage(float doNotUseThisParameter=0) {
            float health = 0;
            float posture = 0;
            foreach (Damage damage in damages) {
                health += damage.health;
                posture += damage.posture;
            }
            foreach (Damage defense in defenses)
            {
                health -= defense.health;
                posture -= defense.posture;
            }

            HealthDamage = Math.Max(health, 0) * healthDamageModifier.Value;
            PostureDamage = Math.Max(posture, 0) * postureDamageModifier.Value;
        }

        public void AddDamage(Damage damage) { 
            damages.Add(damage);
            ComputeDamage();
        }

        public void AddDefense(Damage defense) {
            defenses.Add(defense);
            ComputeDamage();
        }

        public void Reset() {
            damages.Clear();
            ComputeDamage();
        }
    }

    internal class RegenBuffer
    {
        public readonly FloatSum healthRegen;
        public readonly FloatSum postureRegen;

        public readonly FloatProduct hrModifier;
        public readonly FloatProduct prModifier;

        private List<Damage> heals;

        public RegenBuffer()
        {
            healthRegen = new(0, true);
            postureRegen = new(0, true);

            hrModifier = new(1, true);
            prModifier = new(1, true);

            heals = new();
        }

        public Damage Heal { get {
                Damage healing = new();
                foreach (Damage heal in heals) {
                    healing += heal;
                }
                return healing;
            } 
        }

        internal void AddHeal(Damage heal) {
            heals.Add(heal);
        }

        internal void Reset() {
            heals.Clear();
        }

        public float HealthRegen { get { return healthRegen.Value* hrModifier.Value; } }

        public float PostureRegen { get { return postureRegen.Value * prModifier.Value; } }
        public float PostureModifier { get { return prModifier.Value; } }
        public float HealthModifier { get { return hrModifier.Value; } }
    }
}
#endregion