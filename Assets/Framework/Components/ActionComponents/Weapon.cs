using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;

namespace LobsterFramework.AbilitySystem
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private string weaponName;
        [SerializeField] private float weight;
        [SerializeField] private float sharpness;
        [SerializeField] private float attackSpeed;
        [SerializeField] private bool doubleHanded;
        [Range(0, 100)]
        [SerializeField] private float healthDamageReduction;
        [Range(0, 100)]
        [SerializeField] private float postureDamageReduction;

        private float momentumMultiplier;
        private float oppressingForce;

        internal WeaponWielder weaponWielder;

        public UnityAction<Entity> onEntityHit;
        public UnityAction<Weapon> onWeaponHit;

        new private Collider2D collider;

        public string Name { get { return weaponName; } }
        public float Weight { get { return weight; } }
        public float Sharpness { get { return sharpness; } }
        public float AttackSpeed { get {  return attackSpeed; } }

        public bool DoubleHanded { get { return doubleHanded; } }

        public float Momentum { get { return momentumMultiplier * weight; } }

        public float HealthDamageReduction { get { return healthDamageReduction; } }
        public float PostureDamageReduction { get { return postureDamageReduction; } }

        public Entity Entity { get; set; }

        private HashSet<Entity> hit;

        public WeaponState state { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            collider = GetComponent<Collider2D>();
            collider.enabled = false;
            momentumMultiplier = 1;
            oppressingForce = 0;
            hit = new();
        }

        /// <summary>
        /// Enable the collider of the weapon and set its weapon state. Momentum will start to accumulate after this. Can only be used after calling On().
        /// </summary>
        /// <param name="state">The weapon state to set the weapon to be</param>
        public void Action(WeaponState state = WeaponState.Attacking) {
            collider.enabled = true;
            this.state = state;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity != null && onEntityHit != null && !hit.Contains(entity)) {
                onEntityHit.Invoke(entity);
                hit.Add(entity);
                return;
            }

            Weapon weapon = collision.GetComponent<Weapon>();
            if(weapon != null && weapon.state == WeaponState.Guarding && onWeaponHit != null && !hit.Contains(weapon.Entity))
            {
                onWeaponHit.Invoke(weapon);
                hit.Add(weapon.Entity);
            }
        }

        private void Update()
        {
            if(state == WeaponState.Attacking) {
                momentumMultiplier += (attackSpeed / Time.deltaTime);
            }
        }

        private void OnValidate()
        {
            if (attackSpeed <= 0) {
                attackSpeed = 1;
                Debug.LogWarning("Attack Speed Can't be non-positive");
            }
        }

        /// <summary>
        /// Disable the weapon collider and reset momentum
        /// </summary>
        public void Pause() { 
            collider.enabled=false;
            momentumMultiplier = 1;
            state = WeaponState.Idle;
            hit.Clear();
        }
    }

    public enum WeaponType { 
        Sword,
        Hammer,
        Shiv,
        Rod
    }

    public enum WeaponState { 
        Attacking,
        Guarding,
        Idle
    }
}
