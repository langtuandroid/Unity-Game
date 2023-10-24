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
        }

        private void DisplayTabContent() { 
            foreach (Transform obj in tabContentDisplay.transform) {
                if (obj.gameObject.activeInHierarchy) {
                    Destroy(obj.gameObject);
                }
            }

            foreach (InventoryItem item in inventory.items[(int)currentType]) {
                if (item.itemData == null) {
                    continue;
                }
                GameObject obj = Instantiate(itemEntryTemplate);
                obj.SetActive(true);
                if (item.itemData.Icon != null) {
                    obj.GetComponent<Image>().sprite = item.itemData.Icon;
                }
                obj.GetComponent<Button>().onClick.AddListener(() => { DisplayItemPopupMenu(item); });
                obj.transform.SetParent(tabContentDisplay.transform);
            }
        }
        private void DisplayItemPopupMenu(InventoryItem item) {
            // Generate Options
            List<GameObject> options = new();
            if (item.IsComsumable) {
                GameObject option = Instantiate(itemPopupOptionTemplate);
                option.GetComponent<Button>().onClick.AddListener(() => { item.Consume(inventory); itemPopupMenu.SetActive(false); });
                options.Add(option);
                option.GetComponentInChildren<TextMeshProUGUI>().text = "Consume";
            }

            if (options.Count == 0) {
                return;
            }

            // Enable popup
            Vector3 mousePos = mouse.action.ReadValue<Vector2>();
            itemPopupMenu.SetActive(true);
            itemPopupMenu.GetComponent<RectTransform>().position = mousePos;

            // Clear options
            foreach (Transform child in itemPopupMenu.transform) {
                if (child.gameObject.activeInHierarchy) {
                    Destroy(child.gameObject);
                }                
            }

            // Repopulate options
            foreach (GameObject option in options) {
                option.SetActive(true);
                option.transform.SetParent(itemPopupMenu.transform);
            }
        }
    }
}
