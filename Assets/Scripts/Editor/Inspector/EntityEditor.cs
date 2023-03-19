using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.EditorUtility
{
    [CustomEditor(typeof(Entity))]
    public class EntityComponentEditor : Editor
    {
        public void OnInspectorUpdate()
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Entity entity = (Entity)target;
            EditorGUILayout.LabelField("Health: " + entity.Health + "/" + entity.MaxHealth);
            EditorGUILayout.LabelField("Action Blocked: " + entity.ActionBlocked);
            EditorGUILayout.LabelField("Movement Blocked: " + entity.MovementBlocked);
            if (entity.activeEffects != null && entity.activeEffects.Count > 0)
            {
                EditorGUILayout.LabelField("Effects:");
                EditorGUI.indentLevel++;
                foreach (Effect effect in entity.activeEffects.Values)
                {
                    EditorGUILayout.LabelField(effect.name + ": " + effect.Counter_t + "/" + effect.Duration);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
