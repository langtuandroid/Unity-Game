using UnityEngine;

public class Inventory : Interactor
{
    /* Inventory used for storing items. Items with the same name are treated as the same type 
     regardless of their other properties such as description, icon and itemLimit, and therefore are 
    put into the same item slot.
     */
    [SerializeField] private InventoryItem[] items;

    protected override void OnInteract(InteractableObject obj, InteractionType interactionType) {
        Debug.Log("Collecting Item");
        if (obj.GetType() == typeof(CollectableItem)) {
            CollectableItem item = (CollectableItem)obj;
            int index;
            InventoryItem itemToCollect = item.Item;
            while ((index = FindSpot(itemToCollect)) != -1) {
                items[index].AddItem(ref itemToCollect);
                if (itemToCollect.Quantity == 0) { 
                    break;
                }
            }
            item.Item = itemToCollect;
        }
    }

    // Find a available spot for the item and return its index, return -1 if there's no spot left
    public int FindSpot(InventoryItem itemToCollect) {
        for (int i = 0;i < items.Length;i++) {
            InventoryItem item = items[i];
            if ((item.itemData == null) || (item.itemData.name == itemToCollect.itemData.name && item.Quantity < item.itemData.ItemLimit)) {
                return i;
            }
        }
        return -1;
    }
}

[System.Serializable]
public struct InventoryItem {
    [SerializeField] private int quantity;
    public SimpleItem itemData;

    public int Quantity { 
        get { return quantity; }
    }

    public void AddItem (ref InventoryItem itemToMerge)  {
        // Overwrite the empty slot data with the incoming item
        if (itemData == null) {
            itemData = itemToMerge.itemData;
            quantity = itemToMerge.quantity;
            CorrectInventoryQuantity();
            itemToMerge.quantity -= quantity;
            return;
        }

        // Merge item quantity with respect to the itemLimit of "this"
        int availableSpace = itemData.ItemLimit - quantity;
        int amount = itemToMerge.Quantity;
        if (availableSpace >= amount)
        {
            quantity += amount;
            itemToMerge.quantity = 0;
        }
        else {
            quantity = itemData.ItemLimit;
            itemToMerge.quantity = amount - availableSpace;
        } 
    }

    public void CorrectInventoryQuantity(bool enforceLimit = true) {
        if (quantity < 0) {
            quantity = 0;
            return;
        }
        if (enforceLimit && quantity > itemData.ItemLimit) {
            quantity = itemData.ItemLimit;
        }
    }
}


