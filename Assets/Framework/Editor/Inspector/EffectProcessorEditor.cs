using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.Effects;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(EffectProcessor))]
    public class EffectProcessorEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EffectProcessor processor = (EffectProcessor)target;
            if (processor.activeEffects != null && processor.activeEffects.Count > 0)
            {
                EditorGUILayout.LabelField("Effects:");
                EditorGUI.indentLevel++;
                foreach (Effect effect in processor.activeEffects.Values)
                {
                    EditorGUILayout.LabelField(effect.name + ": " + effect.Counter_t + "/" + effect.Duration);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
