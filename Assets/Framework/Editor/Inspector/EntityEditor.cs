using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : UnityEditor.Editor
    {
        public void OnInspectorUpdate()
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Entity entity = (Entity)target;
            Rect r = EditorGUILayout.BeginVertical();
            if (entity.MaxHealth > 0) {
                EditorGUI.ProgressBar(r, (float)entity.Health / entity.MaxHealth, "Health " + entity.Health + "/" + entity.MaxHealth);
            }
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
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
