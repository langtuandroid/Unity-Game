using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using LobsterFramework.EntitySystem;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Utility;

namespace LobsterFramework.Pool
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Bullet : MonoBehaviour
    {
        private float power;
        private float weight;
        [SerializeField] private int pierceCount;
        [SerializeField] private float timeActive;
        private float expireTime;

        private TargetSetting targetSetting;
        private HashSet<Entity> hitted = new();
        private Entity attacker;

        private void OnEnable()
        {
            hitted.Clear();
        }

        public void Initialize(TargetSetting target, float duration, Entity attacker, float attackPower, int piercePower, float weight)
        {
            power = attackPower;
            pierceCount = piercePower;
            targetSetting = target;
            expireTime = Time.time + duration;
            this.attacker = attacker;
            this.weight = weight;
        }

        private void Update()
        {
            if (Time.time >= expireTime)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = collision.GetComponent<Entity>();
            Weapon weapon = collision.GetComponent<Weapon>();
            if (entity != null)
            {
                if (targetSetting.IsTarget(entity))
                {
                    entity.Damage(power, weight, attacker);
                    MovementController moveControl = entity.GetComponent<MovementController>();
                    moveControl.ApplyForce(entity.transform.position - transform.position, weight);
                    pierceCount -= 1;
                }
            }
            else {
                pierceCount = -1; 
            }
            if (weapon != null && weapon.ClashSpark != null && targetSetting.IsTarget(weapon.Entity)) {
                ObjectPool.GetObject(weapon.ClashSpark, transform.position, transform.rotation);
            }

            if (pierceCount < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
