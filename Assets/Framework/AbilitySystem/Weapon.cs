using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;
using static Codice.Client.Common.Connection.AskCredentialsToUser;
using System;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private string weaponName;
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private float weight;
        [SerializeField] private float sharpness;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float defenseSpeed;
        [SerializeField] private bool doubleHanded;

        [Header("Guard")]
        [Range(0, 1)]
        [SerializeField] private float healthDamageReduction;
        [Range(0, 1)]
        [SerializeField] private float postureDamageReduction;
        [Range(0, 1)]
        [SerializeField] private float gRotationSpeedModifier;
        [Range(0, 1)]
        [SerializeField] private float gMoveSpeedModifier;

        [Header("Light Attack")]
        [Range(0, 1)]
        [SerializeField] private float lRotationSpeedModifier;
        [Range(0, 1)]
        [SerializeField] private float lMoveSpeedModifier;

        [Header("Heavy Attack")]
        [Range(0, 1)]
        [SerializeField] private float hRotationSpeedModifier;
        [Range(0, 1)]
        [SerializeField] private float hMoveSpeedModifier;

        [Header("Special Move")]
        [SerializeField] private WeaponArtSelector abilitySelector;

        private float momentumMultiplier;
        private float oppressingForce;

        internal WeaponWielder weaponWielder;

        public UnityAction<Entity> onEntityHit;
        public UnityAction<Weapon, Vector3> onWeaponHit;

        private Collider2D thisCollider;
        new private Transform transform;

        public string Name { get { return weaponName; } }
        public WeaponType WeaponType { get { return weaponType; } }

        public ValueTuple<Type, string> AbilitySetting { get { return ValueTuple.Create(abilitySelector.Type, abilitySelector.ConfigName); } }
        public float Weight { get { return weight; } }
        public float Sharpness { get { return sharpness; } }
        public float AttackSpeed { get {  return attackSpeed; } }

        public float DefenseSpeed { get { return defenseSpeed; } }

        public bool DoubleHanded { get { return doubleHanded; } }

        public float Momentum { get { return momentumMultiplier * weight; } }

        #region Guard
        public float HealthDamageReduction { get { return healthDamageReduction; } }
        public float PostureDamageReduction { get { return postureDamageReduction; } }
        public float GMoveSpeedModifier { get { return gMoveSpeedModifier; } }
        public float GRotationSpeedModifier { get { return gRotationSpeedModifier; } }
        #endregion

        #region LightAttack
        public float LMoveSpeedModifier { get { return lMoveSpeedModifier; } }
        public float LRotationSpeedModifier { get { return lRotationSpeedModifier; } }
        #endregion

        #region HeavyAttack
        public float HMoveSpeedModifier { get { return hMoveSpeedModifier; } }
        public float HRotationSpeedModifier { get { return hRotationSpeedModifier; } }
        #endregion

        public Entity Entity { get; set; }

        private HashSet<Entity> hitted;
        private HashSet<Entity> newHit;

        public WeaponState state { get; set; }
        // Start is called before the first frame update
        void Awake()
        {
            thisCollider = GetComponent<Collider2D>();
            thisCollider.enabled = false;
            transform = GetComponent<Transform>();
            momentumMultiplier = 1;
            oppressingForce = 0;
            state = WeaponState.Idle;
            newHit = new();
            hitted = new();
        }

        /// <summary>
        /// Enable the collider of the weapon and set its weapon state. Momentum will start to accumulate after this. Can only be used after calling On().
        /// </summary>
        /// <param name="state">The weapon state to set the weapon to be</param>
        public void Action(WeaponState state = WeaponState.Attacking) {
            thisCollider.enabled = true;
            this.state = state;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (state == WeaponState.Guarding) {
                return;
            }
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null && !hitted.Contains(entity)) {
                newHit.Add(entity);
                hitted.Add(entity);
                return;
            }

            Weapon weapon = collider.GetComponent<Weapon>();
            
            if(weapon != null && weapon.state == WeaponState.Guarding && onWeaponHit != null && (!hitted.Contains(weapon.Entity) || newHit.Contains(weapon.Entity)))
            {
                Vector2 point = Physics2D.ClosestPoint(transform.position, collider);
                onWeaponHit.Invoke(weapon, point);
                if (newHit.Contains(weapon.Entity))
                {
                    newHit.Remove(weapon.Entity);
                }
                hitted.Add(weapon.Entity);
            }
        }

        private void Update()
        {
            if(state == WeaponState.Attacking) {
                momentumMultiplier += (attackSpeed / Time.deltaTime);
            }
            if(onEntityHit != null)
            {
                foreach (Entity entity in newHit)
                {
                    onEntityHit.Invoke(entity);
                }
            }
            
            newHit.Clear();
        }

        private void OnValidate()
        {
            if (attackSpeed <= 0) {
                attackSpeed = 1;
                Debug.LogWarning("Attack Speed Can't be non-positive");
            }
            if(defenseSpeed <= 0) {  
                defenseSpeed = 1;
                Debug.LogWarning("Guard Speed Can't be non-positive");
            }
            if (abilitySelector.weaponType != weaponType) {
                abilitySelector.weaponType = weaponType;
            } 
        }

        /// <summary>
        /// Disable the weapon collider and reset momentum
        /// </summary>
        public void Pause() { 
            thisCollider.enabled=false;
            momentumMultiplier = 1;
            state = WeaponState.Idle;
            hitted.Clear();
        }
    }
    [Serializable]
    public enum WeaponType { 
        Sword,
        Hammer,
        Dagger,
        Stick,
        EmptyHand,
    }

    public enum WeaponState { 
        Attacking,
        Guarding,
        Idle
    }
}
