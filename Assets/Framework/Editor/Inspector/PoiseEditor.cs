using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEditor;


namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(Poise))]
    public class PoiseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Poise poise = (Poise)target;
            float progress = 0;
            if (poise.MaxPoise != 0) {
                progress = poise.PoiseValue / poise.MaxPoise; 
            }
            EditorGUILayout.LabelField("Poise");
            Rect rect = EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(rect, progress, poise.PoiseValue + "/" + poise.MaxPoise);
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
        }
    }
}
