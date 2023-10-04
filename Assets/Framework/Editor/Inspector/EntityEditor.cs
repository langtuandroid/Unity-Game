using UnityEngine;
using UnityEditor;

namespace LobsterFramework.Editors
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : Editor
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
        }
    }
}
