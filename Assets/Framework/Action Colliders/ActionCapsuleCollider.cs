using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility;

namespace LobsterFramework.Pool
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class ActionCapsuleCollider : MonoBehaviour
    {
        public UnityEvent<Entity> entityEvent;
        private CapsuleCollider2D collider2d;
        // Start is called before the first frame update
        void Awake()
        {
            collider2d = GetComponent<CapsuleCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = GameUtility.FindEntity(collision.gameObject);
            if (entity != null)
            {
                entityEvent.Invoke(entity);
            }
        }

        private void OnDisable()
        {
            entityEvent.RemoveAllListeners();
        }

        public void SetColliderSize(Vector2 size)
        {
            collider2d.size = size;
        }
    }
}
