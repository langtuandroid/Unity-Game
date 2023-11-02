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
        /// <summary>
        /// Container for the items inside this inventory, contains arrays of items of each item type indexed by the Enum ItemType. 
        /// </summary>
        [SerializeField] internal List<ItemContainer> items;
        /// <summary>
        /// The entity holding this inventory
        /// </summary>
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

                // Make sure the quantity does not exceed max quantity allowed for this item
                itemToCollect.Quantity = itemToCollect.Quantity;

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

                if (itemToCollect.Quantity == 0)
                {
                    item.CheckStatus();
                    return;
                }

                // Move the rest to empty slots
                for (int i = 0; i < container.Count; i++)
                {
                    if (container[i].itemData == null || (container[i].itemData.ItemType != itemType && !container[i].itemData.Persistent))
                    {
                        container[i].AddItem(itemToCollect.Clone());
                        item.Item.Quantity = 0;
                        item.CheckStatus();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Drop specified inventory item by removing it from the inventory and generating a CollectableItem on the ground
        /// </summary>
        /// <param name="itemToDrop">The item to be dropped, must be present in the inventory, otherwise this method do nothing</param>
        public void DropItem(InventoryItem itemToDrop) {
            ItemType type = itemToDrop.itemData.ItemType;
            ItemContainer container = items[(int)type];
            int index = container.IndexOf(itemToDrop);
            if (index == -1) {
                return;
            }
            InventoryUtil.GenerateItem(itemToDrop, transform.position);
            if (itemToDrop.itemData.Persistent)
            {
                itemToDrop.Quantity = 0;
            }
            else {
                container[index].itemData = null;
            }
        }
    }

    [System.Serializable]
    public class InventoryItem
    {
        [SerializeField] private int quantity;
        public Item itemData;

        public int Quantity
        {
            get {
                return quantity;
            }
            set {
                if (value >= 0 && itemData != null) {
                    quantity = Mathf.Min(itemData.ItemLimit, value);
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
                Quantity = itemToAdd.quantity;
                itemToAdd.quantity = 0;
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

        public InventoryItem Clone() {
            InventoryItem obj = new();
            obj.quantity = quantity;
            obj.itemData = itemData;
            return obj;
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