using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using System;

namespace LobsterFramework.Editors
{
    [CustomPropertyDrawer(typeof(AbilitySelector))]
    public class AbilitySelectorDrawer : PropertyDrawer
    {
        private List<Type> selectionMapping;
        private GUIContent[] options;
        private List<string> names;
        private int selectionIndex;

        public AbilitySelectorDrawer() { 
            selectionMapping = new();
            selectionMapping.AddRange(AddAbilityMenuAttribute.abilities);
            selectionIndex = -1;
            names = new List<string>();
            names.Add("None");
            options = new GUIContent[selectionMapping.Count + 1];
            options[0] = new("None");
            for(int i = 1;i < selectionMapping.Count + 1;i++) {
                Type type = selectionMapping[i - 1];
                GUIContent content = new(type.Name);
                if (AddAbilityMenuAttribute.abilityIcons.ContainsKey(type)) {
                    content.image = AddAbilityMenuAttribute.abilityIcons[type];
                }
                options[i] = content;
                names.Add(type.AssemblyQualifiedName);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (selectionMapping.Count == 0) {
                return;
            }
            while(property.name != "qualifieldTypeName") {
                property.Next(true);
            }
            if (selectionIndex == -1) {
                selectionIndex = names.IndexOf(property.stringValue);
                if (selectionIndex == -1) {
                    selectionIndex = 0;
                }
            }

            selectionIndex  = EditorGUI.Popup(position, label, selectionIndex, options);
            string abilityName = names[selectionIndex];
            property.stringValue = abilityName;
        }
    }
}
