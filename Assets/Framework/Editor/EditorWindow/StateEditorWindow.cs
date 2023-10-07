using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using LobsterFramework.AI;

namespace LobsterFramework.Editors
{
   
    public class StateEditorWindow : EditorWindow
    {
        public VisualTreeAsset uiPrefab;
        private StateMachine stateMachine;
        private IMGUIContainer imgui;
        private Editor stateEditor;

        [MenuItem("Window/LobsterFramework/StateEditor")]
        public static void ShowWindow() { 
            StateEditorWindow window = GetWindow<StateEditorWindow>();
            if (Selection.activeGameObject != null) {
                window.stateMachine = Selection.activeGameObject.GetComponent<StateMachine>();
            }
            window.Show();
        }

        public static void ShowWindow(StateMachine stateMachine) {
            StateEditorWindow window = GetWindow<StateEditorWindow>();
            window.stateMachine = stateMachine;
            window.Show();
        }

        public void CreateGUI()
        {
            VisualElement root = uiPrefab.CloneTree();
            rootVisualElement.Add(root);
            imgui = root.Q<IMGUIContainer>();

            if (stateMachine != null)
            {
                stateEditor = Editor.CreateEditor(stateMachine); 
                imgui.onGUIHandler += stateEditor.OnInspectorGUI;
            }
        }

        public void OnSelectionChange() 
        {
            if (Selection.activeGameObject != null) {
                stateMachine = Selection.activeGameObject.GetComponent<StateMachine>(); 
            }
            
            if (stateMachine != null)
            {
                if (stateEditor != null) 
                {
                    imgui.onGUIHandler -= stateEditor.OnInspectorGUI;
                    DestroyImmediate(stateEditor);
                }
                stateEditor = Editor.CreateEditor(stateMachine);
                imgui.onGUIHandler += stateEditor.OnInspectorGUI;
            }
        }

        private void OnDestroy()
        {
            if (stateEditor != null)
            {
                imgui.onGUIHandler -= stateEditor.OnInspectorGUI;
                DestroyImmediate(stateEditor);
            }
        }
    }
}
