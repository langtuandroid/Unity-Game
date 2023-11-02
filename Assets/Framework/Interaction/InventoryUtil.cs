using LobsterFramework.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    public class InventoryUtil : MonoBehaviour
    {
        private static InventoryUtil instance;

        [SerializeField] private Transform interactableObjects;

        private void Awake()
        {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        public static void GenerateItem(InventoryItem item, Vector3 position){
            GameObject obj = new(item.itemData.ItemName, typeof(CollectableItem), typeof(SpriteRenderer));
            CollectableItem collectableItem = obj.GetComponent<CollectableItem>();
            collectableItem.Item = item.Clone();
            collectableItem.interactEnabled = true;
            collectableItem.destroyWhenDeplete = true;
            obj.transform.position = position;
            obj.transform.SetParent(instance.interactableObjects.transform, true);
        }
    }
}
