using UnityEngine;
using LobsterFramework.Interaction;
using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace LobsterFramework.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Clear Popups")]
        [SerializeField] Button clearButton;
        [Header("Tabs")]
        [SerializeField] private GameObject tabLocation;
        [SerializeField] private GameObject tabTemplate;
        [SerializeField] private GameObject tabContentDisplay;
        [Header("Item")]
        [SerializeField] private GameObject itemEntryTemplate;
        [SerializeField] private GameObject itemPopupMenu;
        [SerializeField] private GameObject itemPopupOptionTemplate;
        [Header("References")]
        [SerializeField] private Inventory inventory;
        [SerializeField] private InputActionReference mouse;
        private List<GameObject> tabs;
        private ItemType currentType;

        // Start is called before the first frame update
        void Start()
        {
            tabs = new();
            foreach (ItemType type in Enum.GetValues(typeof(ItemType))) {
                GameObject tab = Instantiate(tabTemplate);
                tab.SetActive(true);
                TextMeshProUGUI textMesh = tab.GetComponentInChildren<TextMeshProUGUI>();
                textMesh.text = type.ToString();
                tabs.Add(tab);
                tab.GetComponent<Button>().onClick.AddListener(()=> { currentType = type; DisplayTabContent(); });
                tab.transform.SetParent(tabLocation.transform);
            }

            clearButton.onClick.AddListener(() =>
            {
                itemPopupMenu.SetActive(false);
            });
        }

        private void DisplayTabContent() { 
            foreach (Transform obj in tabContentDisplay.transform) {
                if (obj.gameObject != itemEntryTemplate) {
                    Destroy(obj.gameObject);
                }
            }

            foreach (InventoryItem item in inventory.items[(int)currentType]) {
                if (item.itemData == null) {
                    continue;
                }
                GameObject obj = Instantiate(itemEntryTemplate);
                obj.SetActive(true);
                ItemEntryUI itemEntryUI = obj.GetComponent<ItemEntryUI>();
                obj.GetComponent<Button>().onClick.AddListener(() => { DisplayItemPopupMenu(item, itemEntryUI);});
                itemEntryUI.DisplayItem(item);
                obj.transform.SetParent(tabContentDisplay.transform);
            }
        }
        private void DisplayItemPopupMenu(InventoryItem item, ItemEntryUI itemEntryUI) {
            // Generate Options
            List<GameObject> options = new();
            if (item.IsComsumable) {
                GameObject option = Instantiate(itemPopupOptionTemplate);
                option.GetComponent<Button>().onClick.AddListener(() => { item.Consume(inventory); itemEntryUI.DisplayItem(item); itemPopupMenu.SetActive(false); });
                options.Add(option);
                option.GetComponentInChildren<TextMeshProUGUI>().text = "Consume";
            }
            if (item.itemData.Discardable) {
                options.Add(CreateDiscardOption(item, itemEntryUI));
            }

            if (options.Count == 0) {
                return;
            }

            // Enable popup
            Vector3 mousePos = mouse.action.ReadValue<Vector2>();
            Debug.Log(mousePos);
            itemPopupMenu.SetActive(true);
            itemPopupMenu.transform.position = mousePos;

            // Clear options
            foreach (Transform child in itemPopupMenu.transform) {
                if (child.gameObject != itemPopupOptionTemplate) {
                    Destroy(child.gameObject);
                }                
            }

            // Repopulate options
            foreach (GameObject option in options) {
                option.SetActive(true);
                option.transform.SetParent(itemPopupMenu.transform);
            }
        }

        private GameObject CreateDiscardOption(InventoryItem item, ItemEntryUI itemEntryUI) {
            GameObject option = Instantiate(itemPopupOptionTemplate);
            option.GetComponent<Button>().onClick.AddListener(() => { inventory.DropItem(item); itemEntryUI.DisplayItem(item); itemPopupMenu.SetActive(false); });
            option.GetComponentInChildren<TextMeshProUGUI>().text = "Discard";
            return option;
        }
    }
}
