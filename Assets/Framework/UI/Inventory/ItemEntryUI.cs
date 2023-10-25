using LobsterFramework.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LobsterFramework
{
    public class ItemEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemQuantity;
        private Image image; 

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void DisplayItem(InventoryItem item) {
            if (item.Quantity > 0 || item.itemData.Persistent)
            {
                itemName.text = item.itemData.ItemName;
                itemQuantity.text = item.Quantity.ToString();
                if (item.itemData.Icon != null)
                {
                    image.sprite = item.itemData.Icon;
                }
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}
