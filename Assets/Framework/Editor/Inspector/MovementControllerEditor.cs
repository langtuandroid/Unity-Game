using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.AbilitySystem;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(MovementController))]
    public class MovementControllerEditor : Editor
    {
        public override void OnInspectorGUI() { 
            base.OnInspectorGUI();
            MovementController controller = (MovementController)target;
            EditorGUILayout.LabelField("Current Max Speed", controller.Speed + "");
            EditorGUILayout.LabelField("Current Rotate Speed", controller.RotateSpeed + "");
        }
    }
}
