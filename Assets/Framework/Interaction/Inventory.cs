using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    public class Inventory : Interactor
    {
        /* Inventory used for storing items. Items with the same name are treated as the same type 
         regardless of their other properties such as description, icon and itemLimit, and therefore are 
        put into the same item slot.
         */
        [SerializeField] internal List<ItemContainer> items;
        [SerializeField] private Entity entity;

        #region EditorFields
        [SerializeField] internal ItemType selectedType;
        #endregion

        public Entity Entity { get { return entity; } }

        protected override void OnInteract(InteractableObject obj, InteractionType interactionType)
        {
            Debug.Log("Collecting Item");
            if (obj.GetType() == typeof(CollectableItem))
            {
                CollectableItem item = (CollectableItem)obj;

                InventoryItem itemToCollect = item.Item;
                if (itemToCollect.itemData == null)
                {
                    return;
                }

                // Attempt to merge item with existing items in inventory
                ItemType itemType = itemToCollect.itemData.ItemType;
                ItemContainer container = items[(int)itemType];
                for (int i = 0; i < container.Count && itemToCollect.Quantity > 0; i++)
                {
                    if (container[i].itemData == itemToCollect.itemData)
                    {
                        container[i].AddItem(itemToCollect);
                    }
                }

                // Move the rest to empty slots
                for (int i = 0; i < container.Count && itemToCollect.Quantity > 0; i++)
                {
                    if (container[i].itemData == null || container[i].itemData.ItemType != itemType)
                    {
                        container[i].AddItem(itemToCollect);
                    }
                }

                item.Item = itemToCollect;
            }
        }
    }

    [System.Serializable]
    public class InventoryItem
    {
        [SerializeField] private int quantity;
        private int maxQuantity;
        public Item itemData;

        public int Quantity
        {
            get {
                return quantity;
            }
            set {
                if (value >= 0) {
                    quantity = Mathf.Max(maxQuantity, value);
                }
            }
        }

        public bool IsComsumable { get {
                return itemData != null && typeof(IConsumable).IsAssignableFrom(itemData.GetType());
         }}

        public void AddItem(InventoryItem itemToAdd)
        {
            if (itemToAdd.itemData == null) {
                return;
            }

            // Overwrite the empty slot data with the incoming item
            if (itemData == null)
            {
                itemData = itemToAdd.itemData;
                maxQuantity = itemToAdd.maxQuantity;
                Quantity = itemToAdd.quantity;
                itemToAdd.quantity -= quantity;
                return;
            }

            // Merge item quantity with respect to the itemLimit of "this"
            if (itemToAdd.itemData == itemData) {
                int before = quantity;
                Quantity += itemToAdd.Quantity;
                itemToAdd.quantity -= (quantity - before);
            }
        }

        /// <summary>
        /// Attempt to apply consume effect if this item
        /// </summary>
        /// <param name="inventory"></param>
        public void Consume(Inventory inventory) {
            try
            {
                if (Quantity > 0)
                {
                    ((IConsumable)itemData).Consume(inventory);
                    Quantity -= 1;
                }
            }
            catch
            {
                Debug.Log("Cannot use consume on this item!");
            }
        }
    }
    [System.Serializable]
    internal class ItemContainer : SerializableArray<InventoryItem>{
        public static implicit operator InventoryItem[](ItemContainer array) { if (array == null) { return null; } return array.items; }
        public static implicit operator ItemContainer(InventoryItem[] array)
        {
            if (array == null) { return null; }
            ItemContainer obj = new();
            obj.items = array;
            return obj;
        }
    }
}