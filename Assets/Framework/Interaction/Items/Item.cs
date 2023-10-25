using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Interaction
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private ItemType itemType;
        [SerializeField] private int itemLimit;
        [SerializeField] private Sprite icon;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private bool discardable;
        [SerializeField] private bool persistent;

        public string ItemName { get { return itemName; } }

        public int ItemLimit { get { return itemLimit; } }
        public ItemType ItemType { get { return itemType; } }

        public Sprite Icon { get { return icon; } }

        public string Description { get { return description; } }
        public bool Discardable { get { return discardable; } }
        public bool Persistent { get { return persistent; } }   
    }

    public enum ItemType { 
        Key,
        Equipment,
        Consumable
    }
}
