using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.AbilitySystem.Weapon
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

        public UnityAction<Entity> onEntityHit;
        public UnityAction<Entity> onEntityHitExit;
        new private Collider2D collider;

        public float Weight { get { return weight; } }
        public float Sharpness { get { return sharpness; } }

        // Start is called before the first frame update
        void Start()
        {
            collider = GetComponent<Collider2D>();
            collider.enabled = false;
        }

        public void Activate() { 
            collider.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null && onEntityHit != null) {
                onEntityHit.Invoke(entity);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null && onEntityHitExit != null)
            {
                onEntityHitExit.Invoke(entity);
            }
        }

        public void Deactivate() { 
            collider.enabled=false;
        }
    }

    public enum WeaponType { 
        Sword,
        Hammer,
        Shiv,
        Rod
    }
}
