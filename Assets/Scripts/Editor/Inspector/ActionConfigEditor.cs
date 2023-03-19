using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.Action;

namespace LobsterFramework.EditorUtility
{
    [CustomEditor(typeof(ActionInstance.ActionConfig), true)]
    public class ActionConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
