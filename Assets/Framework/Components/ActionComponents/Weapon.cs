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
        [SerializeField] private float weight;
        [SerializeField] private float sharpness;
        [SerializeField] private float attackSpeed;
        [Range(0, 100)]
        [SerializeField] private float healthDamageReduction;
        [Range(0, 100)]
        [SerializeField] private float postureDamageReduction;

        private float momentumMultiplier;
        private float oppressingForce;

        public UnityAction<Entity> onEntityHit;
        public UnityAction<Weapon> onWeaponHit;

        new private Collider2D collider;

        public float Weight { get { return weight; } }
        public float Sharpness { get { return sharpness; } }
        public float AttackSpeed { get {  return attackSpeed; } }

        public float Momentum { get { return momentumMultiplier * weight; } }

        public float HealthDamageReduction { get { return healthDamageReduction; } }
        public float PostureDamageReduction { get { return postureDamageReduction; } }

        public Entity Entity { get; set; }

        private HashSet<Entity> blocked;

        public WeaponState state { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            collider = GetComponent<Collider2D>();
            collider.enabled = false;
            momentumMultiplier = 1;
            oppressingForce = 0;
            blocked = new();
        }

        public void Activate(WeaponState state = WeaponState.Attacking) { 
            collider.enabled = true;
            this.state = state;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = GameUtility.FindEntity(collision.gameObject);
            if (entity != null && onEntityHit != null && !blocked.Contains(entity)) {
                onEntityHit.Invoke(entity);
                return;
            }

            Weapon weapon = collider.GetComponent<Weapon>();
            if(weapon != null && weapon.state == WeaponState.Guarding && onWeaponHit != null)
            {
                onWeaponHit.Invoke(weapon);
                blocked.Add(weapon.Entity);
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

        public void Deactivate() { 
            collider.enabled=false;
            momentumMultiplier = 1;
            state = WeaponState.Idle;
            blocked.Clear();
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
