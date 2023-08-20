using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;
using System;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(Ability.AbilityConfig), true)]
    public class AbilityConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();
                DrawPropertiesExcluding(serializedObject, "m_Script");
                serializedObject.ApplyModifiedProperties();
            }catch(ArgumentException e)
            {
                // Ignore ArgumentException
            }
        }
    }
}
