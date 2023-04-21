using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using LobsterFramework.EntitySystem;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.Pool
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private RefInt innatePower;
        [SerializeField] private RefInt piercePower;
        [SerializeField] private Sprite explosionSprite;
        [SerializeField] private VarString spritePoolTag;
        [SerializeField] private RefFloat explodeDuration;
        [SerializeField] private RefFloat explosionRadius;
        private int power;
        [SerializeField] private int pierceCount;
        [SerializeField] private float timeActive;
        [SerializeField] private float timeCounter;

        private TargetSetting targetSetting;
        private Entity attacker;
        private Transform _transform;

        public void Initialize(TargetSetting target, float duration, Entity attacker = null, int attackPower = -1, int piercePower = -1)
        {
            if (attackPower == -1)
            {
                attackPower = innatePower.Value;
            }

            if (piercePower == -1)
            {
                piercePower = this.piercePower.Value;
            }

            power = attackPower;
            pierceCount = piercePower;
            targetSetting = target;

            timeActive = duration;
            timeCounter = 0;
            this.attacker = attacker;

        }

        private void Start()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= timeActive)
            {
                Explode();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleCollision(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HandleCollision(collision);
        }

        private void HandleCollision(Collider2D collision)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                if (targetSetting.IsTarget(entity))
                {
                    entity.RegisterDamage(power, attacker);
                    AIPath ai = entity.GetComponent<AIPath>();
                    Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
                    if (ai != null)
                    {
                        ai.canMove = false;
                        GameManager.ExecuteDelegate(() => { ai.canMove = true; }, 1);
                    }
                    rb.AddForce(power * (entity.transform.position - _transform.position).normalized);
                    pierceCount -= 1;
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
            GameObject obj = ObjectPool.Instance.GetObject(spritePoolTag.Value, transform.position, Quaternion.identity);
            TemporalObject o = obj.GetComponent<TemporalObject>();
            o.ResetCounter();
            o.duration = explodeDuration.Value;
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = explosionSprite;
            float radius = explosionRadius.Value;
            obj.transform.localScale = new Vector3(radius, radius, 1);
        }
    }
}
