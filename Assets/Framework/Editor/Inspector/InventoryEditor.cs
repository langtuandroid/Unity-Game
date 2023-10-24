using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.Interaction;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using Codice.CM.SEIDInfo;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        public VisualTreeAsset treeAsset;

        private VisualElement root;
        private EnumField itemType;
        private ScrollView scrollView;
        private PropertyField itemList;

        public override VisualElement CreateInspectorGUI()
        {
            LoadElements();
            itemType.bindingPath = "selectedType";
            itemType.Bind(serializedObject);
            itemType.RegisterCallback<ChangeEvent<Enum>>(DisplaySetting);
            DisplaySetting();
            return root;
        }

        private void LoadElements() {
            if (treeAsset == null) {
                treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Framework/Editor/Inspector/InventoryInspector.uxml");
            }
            root = treeAsset.CloneTree();
            itemType = root.Q<EnumField>("item_type");
            scrollView = root.Q<ScrollView>();
            itemList = scrollView.Q<PropertyField>();
        }

        private void DisplaySetting(ChangeEvent<Enum> changeEvent=null) {
            Inventory inventory = (Inventory)target;
            if (inventory.items == null) {
                inventory.items = new();
            }
            ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
            
            if (inventory.items.Count != itemTypes.Length) { 
                if (inventory.items.Count < itemTypes.Length)
                {
                    for (int i = inventory.items.Count;i < itemTypes.Length;i++) {
                        inventory.items.Add(new());
                    }
                }
                else { 
                    inventory.items.RemoveRange(itemTypes.Length, inventory.items.Count - itemTypes.Length);
                }
            }
            int index = (int)inventory.selectedType;
            SerializedProperty lstProperty = serializedObject.FindProperty("items");
            SerializedProperty container = lstProperty.GetArrayElementAtIndex(index);
            SerializedProperty array = container.FindPropertyRelative("items");
            itemList.BindProperty(array);
        }
    }
}
