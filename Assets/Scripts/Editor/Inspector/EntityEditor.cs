using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityComponentEditor : Editor
{
    public void OnInspectorUpdate() { 
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Entity entity = (Entity)target;
        EditorGUILayout.LabelField("Health: " + entity.Health + "/" + entity.MaxHealth);
    }
}
