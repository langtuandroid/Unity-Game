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
        [SerializeField] private float timeCounter;

        private TargetSetting targetSetting;
        private Entity attacker;

        public void Initialize(TargetSetting target, float duration, Entity attacker, float attackPower, int piercePower, float weight)
        {
            power = attackPower;
            pierceCount = piercePower;
            targetSetting = target;

            timeActive = duration;
            timeCounter = 0;
            this.attacker = attacker;
            this.weight = weight;
        }

        private void Update()
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= timeActive)
            {
                Explode();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = GameUtility.FindEntity(collision.gameObject);
            if (entity != null)
            {
                if (targetSetting.IsTarget(entity))
                {
                    entity.Damage(power, weight, attacker);
                    MovementController moveControl = entity.GetComponent<MovementController>();
                    moveControl.ApplyForce(entity.transform.position - transform.position, weight);
                }
            }
            else
            {
                pierceCount -= 1;
            }
            if (pierceCount < 0)
            {
                Explode();
            }
        }

        private void Explode()
        {
            gameObject.SetActive(false);
            
        }
    }
}
