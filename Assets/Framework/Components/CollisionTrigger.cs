using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;
using LobsterFramework.Utility.Groups;

namespace LobsterFramework.Utility
{
    [RequireComponent(typeof(Collider2D))]
    public class CollisionTrigger : MonoBehaviour
    {
        [SerializeField] private List<EntityGroup> entityGroups = new();
        [SerializeField] private RefBool disableOnTrigger;
        [SerializeField] private RefBool repeatTrigger;

        public UnityEvent<Entity> OnTriggerCallBack = new();
        public UnityEvent<Entity> OnTriggerExitCallBack = new();

        private HashSet<Entity> trackingEntities = new();
        private HashSet<Entity> triggeredEntities = new();

        private void Awake()
        {
            foreach (EntityGroup group in entityGroups)
            {
                group.OnEntityAdded.AddListener(AddEntity);
                group.OnEntityRemoved.AddListener(RemoveEntity);
            }
        }

        private void AddEntity(Entity entity) { trackingEntities.Add(entity); }
        private void RemoveEntity(Entity entity) { trackingEntities.Remove(entity); }

        private void Update()
        {
            foreach (Entity entity in triggeredEntities)
            {
                OnTriggerCallBack.Invoke(entity);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity != null && trackingEntities.Contains(entity))
            {
                if (disableOnTrigger.Value)
                {
                    OnTriggerCallBack.Invoke(entity);
                    gameObject.SetActive(false);
                }
                else
                {
                    if (repeatTrigger.Value)
                    {
                        triggeredEntities.Add(entity);
                    }
                    else
                    {
                        OnTriggerCallBack.Invoke(entity);
                    }
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                if (triggeredEntities.Remove(entity))
                {
                    OnTriggerExitCallBack.Invoke(entity);
                }
            }
        }
    }
}
