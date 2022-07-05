using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Actionable))]
public class ActionEditor : Editor
{
    ActionComponents selected = ActionComponents.Combat;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Actionable action = (Actionable)target;
        int count = action.components.Count;

        EditorGUILayout.BeginHorizontal();
        selected = (ActionComponents)EditorGUILayout.EnumPopup("ActionComponents: ", selected);
        if (GUILayout.Button("+", GUILayout.Width(25))) {
            AddComponent();
        }
        EditorGUILayout.EndHorizontal();

        if (count > 0) {
            EditorGUI.indentLevel++;
            List<ActionComponents> removed = new List<ActionComponents>();
            foreach (ActionComponents a in action.components.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(a.ToString());
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    removed.Add(a);
                }
                EditorGUILayout.EndHorizontal();
            }

            foreach (ActionComponents a in removed)
            {
                action.RemoveComponent(a);
            }

            EditorGUI.indentLevel--;           
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddComponent() {
        Actionable action = (Actionable)target;
        if (action.components.ContainsKey(selected)) {
            return;
        }
        Entity e = action.GetComponent<Entity>();
        switch (selected) {
            case ActionComponents.Combat:
                Combat combat = new Combat(e);
                new StandardMoveSet(e, combat);
                action.AddComponent(selected, combat);
                break;
            case ActionComponents.Move:
                break;
        }
    } 
}
