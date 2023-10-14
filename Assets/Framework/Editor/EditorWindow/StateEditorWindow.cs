using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using LobsterFramework.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using Codice.Client.GameUI.Checkin;

namespace LobsterFramework.Editors
{

    public class StateEditorWindow : EditorWindow
    {
        public VisualTreeAsset uiPrefab;
        public Texture2D icon;
        private StateMachine stateMachine;
        private IMGUIContainer imgui;
        private DropdownField stateSelector;
        private Button browsePath;
        private TextField savePath;
        private TextField saveName;

        private List<State> availableStates;
        private List<string> stateOptions;

        private Editor stateEditor;

        

        // Toolbar
        private ToolbarButton ping;
        private ToolbarButton nextModified;
        private ToolbarButton saveButton;
        private ToolbarButton discardButton;
        private ToolbarButton saveAllButton;
        private ToolbarButton discardAllButton;

        // Globals
        private static Dictionary<StateMachine, HashSet<State>> dirties = new();

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
            LoadElements();

            availableStates = new();
            stateOptions = new();
            stateSelector.choices.Clear();

            // Callback register
            imgui.onGUIHandler += DrawInspector;
            stateSelector.RegisterCallback<ChangeEvent<string>>(OnStateSelected);
            browsePath.clicked += SetSavePath;
            saveButton.clicked += ApplyChanges;
            saveAllButton.clicked += ApplyAllChanges;
            discardButton.clicked += DiscardModifications;
            discardAllButton.clicked += DiscardAllModifications;

            // Toolbar configuration
            nextModified.clicked += PingNextModifiedObject;
            ping.clicked += PingCurrentObject;

            OnSelectionChange();

            if (stateMachine != null)
            {
                UpdateStateOptions();
            }
            if (availableStates.Count == 0) {
                stateSelector.choices.Add("Select State");
                stateSelector.index = 0;
            }

            UpdateSaveDiscardView();
            EditorApplication.playModeStateChanged += Refresh;
        }

        private void Refresh(PlayModeStateChange change) {
            if (change == PlayModeStateChange.ExitingPlayMode) {
                OnSelectionChange();
            }
        }

        private void LoadElements() {
            if (icon != null)
            {
                titleContent.image = icon;
            }
            titleContent.text = "StateEditor";

            // Load Fields
            VisualElement root = uiPrefab.CloneTree();
            rootVisualElement.Add(root);
            imgui = root.Q<IMGUIContainer>();
            stateSelector = root.Q<DropdownField>("state_selector");

            // Toolbar
            Toolbar toolbar = root.Q<Toolbar>();
            ping = toolbar.Q<ToolbarButton>("ping");
            nextModified = toolbar.Q<ToolbarButton>("next_modified");
            saveButton = toolbar.Q<ToolbarButton>("save");
            discardButton = toolbar.Q<ToolbarButton>("discard");
            saveAllButton = toolbar.Q<ToolbarButton>("save_all");
            discardAllButton = toolbar.Q<ToolbarButton>("discard_all");

            saveName = root.Q<TextField>("save_name");
            savePath = root.Q<TextField>("save_path");
            savePath.bindingPath = "statePath";
            browsePath = savePath.Q<Button>("browse_path"); 
        }

        private void OnSelectionChange()
        {
            if (savePath == null) {
                LoadElements();
            }

            if (Selection.activeGameObject != null) {
                stateMachine = Selection.activeGameObject.GetComponent<StateMachine>();
                try
                {
                    savePath.Unbind();
                }
                catch { }
            }

            if (stateMachine != null)
            {
                SerializedObject obj = new(stateMachine);
                savePath.Bind(obj);
                if (stateEditor != null)
                {
                    imgui.onGUIHandler -= stateEditor.OnInspectorGUI;
                    DestroyImmediate(stateEditor);
                }
                UpdateStateOptions();
                DisplayStateEditor();
                UpdateSaveDiscardView();
            }
        }

        private void UpdateStateOptions() {
            stateOptions.Clear();
            availableStates.Clear();

            if (Application.isPlaying) {
                foreach (var kwp in stateMachine.states) 
                {
                    availableStates.Add(kwp.Value);
                    stateOptions.Add(kwp.Key.FullName);
                }
            }
            else {
                availableStates.AddRange(stateMachine.allStates);
                stateOptions.AddRange(availableStates.Select((State s)=>{return s.GetType().FullName; }));
            }
            stateSelector.choices.Clear();
            stateSelector.choices.AddRange(stateOptions);
            stateSelector.index = -1;
            stateSelector.index = 0;
        }

        private void DisplayStateEditor() {
            if (stateEditor != null) {
                DestroyImmediate(stateEditor);
            }
            
            State state = GetStateAtIndex(stateSelector.index);
            if (state != null) {
                stateEditor = Editor.CreateEditor(state);
            }
        }

        private void OnStateSelected(ChangeEvent<string> selectedState = null) { 
            DisplayStateEditor();
            UpdateSaveDiscardView();
        }

        private State GetStateAtIndex(int index) {
            try
            {
                return availableStates[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private void DrawInspector() {
            if (stateEditor == null) {
                return;
            }
            EditorGUI.BeginChangeCheck();
            stateEditor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck() && Application.isPlaying) {
                if (dirties.ContainsKey(stateMachine))
                {
                    dirties[stateMachine].Add((State)stateEditor.target);
                }
                else {
                    dirties.Add(stateMachine, new() {(State)stateEditor.target});
                }
                UpdateSaveDiscardView();
            }
        }

        private void ApplyChanges() {
            if (Application.isPlaying && stateMachine != null && dirties.ContainsKey(stateMachine)) {
                if (savePath.value == "") {
                    EditorUtility.DisplayDialog("Cannot Apply Changes", "You must specify a path to save the data!", "Ok");
                    return;
                }
                State current = (State)stateEditor.target;
                if (!dirties[stateMachine].Contains(current)) {
                    return;
                }
                
                string name = saveName.value;
                if (name == "") {
                    name = current.name;
                }
                string path = savePath.value;
                if (!path.StartsWith(Application.dataPath)) {
                    EditorUtility.DisplayDialog("Cannot create asset", "Path must be in project!", "Ok");
                    return;
                }
                path = path[Application.dataPath.Length..];

                State copy = Instantiate(current);
                stateMachine.allStates[stateSelector.index] = copy;
                AssetDatabase.CreateAsset(copy, "Assets/" + path + "/" + name + ".asset");

                dirties[stateMachine].Remove(current);
                if (dirties[stateMachine].Count == 0) {
                    dirties.Remove(stateMachine);
                }
                UpdateSaveDiscardView();
            }
        }

        private void ApplyAllChanges() {
            if (Application.isPlaying && stateMachine != null && dirties.ContainsKey(stateMachine))
            {
                string path = savePath.value;
                if (path == "")
                {
                    EditorUtility.DisplayDialog("Cannot Apply Changes", "You must specify a path to save the data!", "Ok");
                    return;
                }
                if (!path.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Cannot create asset", "Path must be in project!", "Ok");
                    return;
                }
                path = path[Application.dataPath.Length..];

                foreach (State current in dirties[stateMachine]) {
                    string name = current.name;
                    State copy = Instantiate(current);
                    int index = availableStates.IndexOf(current);
                    stateMachine.allStates[index] = copy;
                    AssetDatabase.CreateAsset(copy, "Assets/" + path + "/" + name + ".asset");
                }
                dirties.Remove(stateMachine);
                UpdateSaveDiscardView();
            }
        }

        private void DiscardModifications() {
            if (Application.isPlaying && stateMachine != null && dirties.ContainsKey(stateMachine)) {
                State current = (State)stateEditor.target;
                if (!dirties[stateMachine].Contains(current))
                {
                    return;
                }
                dirties[stateMachine].Remove(current);
                if (dirties[stateMachine].Count == 0)
                {
                    dirties.Remove(stateMachine);
                }
                UpdateSaveDiscardView();

                availableStates[stateSelector.index] = stateMachine.ResetState(stateSelector.index);

                DisplayStateEditor();
            }
        }

        private void DiscardAllModifications()
        {
            if (Application.isPlaying && stateMachine != null && dirties.ContainsKey(stateMachine))
            {
                foreach (State dirtied in dirties[stateMachine]) {
                    int index = availableStates.IndexOf(dirtied);
                    availableStates[index] = stateMachine.ResetState(index);
                }
                dirties.Remove(stateMachine);
                UpdateSaveDiscardView();
                DisplayStateEditor();
            }
        }

        private void SetSavePath() {
            string path = EditorUtility.OpenFolderPanel("Select Saving Path", Application.dataPath, "");
            if (path != "")
            {
                savePath.value = path;
            }
        }

        private void PingNextModifiedObject() {
            foreach (StateMachine sm in dirties.Keys) {
                EditorGUIUtility.PingObject(sm.gameObject);
                return;
            }
        }

        private void PingCurrentObject() {
            if (stateMachine != null) {
                EditorGUIUtility.PingObject(stateMachine.gameObject);
            }
        }

        private void UpdateSaveDiscardView()
        {
            if (stateMachine != null && dirties.ContainsKey(stateMachine))
            {
                saveAllButton.style.display = DisplayStyle.Flex;
                discardAllButton.style.display = DisplayStyle.Flex;

                if (stateEditor != null && dirties[stateMachine].Contains(stateEditor.target))
                {
                    saveButton.style.display = DisplayStyle.Flex;
                    discardButton.style.display = DisplayStyle.Flex;
                }
                else
                {
                    saveButton.style.display = DisplayStyle.None;
                    discardButton.style.display = DisplayStyle.None;
                }
            }
            else {
                saveAllButton.style.display = DisplayStyle.None;
                discardAllButton.style.display = DisplayStyle.None;
                saveButton.style.display = DisplayStyle.None;
                discardButton.style.display = DisplayStyle.None;
            }
        }

        private void OnDestroy()
        {
            if (stateEditor != null)
            {
                DestroyImmediate(stateEditor);
            }
            EditorApplication.playModeStateChanged -= Refresh;
        }
    }
}
