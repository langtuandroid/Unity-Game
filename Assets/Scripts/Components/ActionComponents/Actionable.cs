using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;

[AddComponentMenu("Actionable")]
public class Actionable : MonoBehaviour
{
    /* Component that provides access to a variety of movesets, it acts as a container and platform for
     * different kinds of actions and moves which are provided and fully implemented by associated sub-components
     
    Properties:
    executing: The current actions being executed
    availableActions: The set of actions available for execution
    components: The set of sub-components that provides additional data for action instances

     */
    [HideInInspector]
    public ActionInstanceSet executing = new();
    [HideInInspector]
    [SerializeField] private ActionableData actionableData;
    [SerializeField] private ActionableData data;
    [SerializeField] private string assetName;
    [SerializeField] private ActionQueue actionQueue;

    private Dictionary<string, ActionInstance> availableActions;
    private TypeActionComponentDictionary components;

    private void Awake()
    {
        if (actionableData == null) {
            if (data == null) {
                Debug.LogWarning("Actionable Data is not set!");
                return;
            }
            actionableData = Instantiate(data);
            actionableData.CopyActionAsset();
        }
        actionableData.identifier = GetInstanceID();
    }

    private void Start()
    {
        if (actionableData == null) {
            Debug.LogWarning("Actionable Data is not set!");
            return;
        }
        int id = GetInstanceID();
        if (actionableData.identifier != id)
        {
            actionableData = Instantiate(actionableData);
            actionableData.identifier = id;
            actionableData.CopyActionAsset();
        }
        components = actionableData.components;
        actionableData.Initialize(this, actionQueue);
        availableActions = actionableData.availableActions;
    }

    public void Reset()
    {
        foreach(ActionComponent component in components.Values) {
            component.Reset();
        }
        foreach (ActionInstance ac in availableActions.Values)
        {
            ac.Reset();
        }
    }


    private void Update()
    {
        UpdateComponents();
        Countdown();
        List<ActionInstance> removed = new();
        foreach (ActionInstance ac in executing)
        {
            if (ac.Executing == 0)
            {
                removed.Add(ac);
            }
        }
        foreach (ActionInstance ac in removed)
        {
            executing.Remove(ac);
        }
    }

    private void OnDestroy()
    {
        foreach (ActionInstance ac in actionableData.availableActions.Values) {
            ac.HaltActionExecution();
            ac.CleanUp();
        }
        foreach (ActionComponent ac in actionableData.components.Values)
        {
            ac.CleanUp();
        }
    }

    public bool IsInstanceReady<T>() where T: ActionInstance {
        string type = typeof(T).ToString();
        if (availableActions.ContainsKey(type)) {
            return availableActions[type].IsReady;
        }
        return false;
    }

    public void SetActionableData() {
        if (data != null) {
            if (actionableData != null && !AssetDatabase.Contains(actionableData)) {
                DestroyImmediate(actionableData, true);
            }
            actionableData = Instantiate(data);
            actionableData.CopyActionAsset();
        }
    }

    public void SaveActionableData() {
        if (actionableData != null) {
            AssetDatabase.CreateAsset(actionableData, "Assets/Scriptable Objects/Action/ActionableData/" + assetName + ".asset");
            actionableData.SaveContentsAsAsset();
            data = actionableData;
            actionableData = Instantiate(actionableData);
            actionableData.CopyActionAsset();
        }
    }

    public T GetActionComponent<T>() where T : ActionComponent{
        string type = typeof(T).ToString();
        if (components.ContainsKey(type)) {
            return (T)components[type];
        }
        return default(T);
    }

    private void Countdown()
    {
        foreach (ActionInstance ac in availableActions.Values)
        {
            ac.CountDown();
        }
    }

    private void UpdateComponents() {
        foreach (ActionComponent c in components.Values) {
            c.Update();
        }
    }

    private T GetActionInstance<T>() where T : ActionInstance {
        string type = typeof(T).ToString();
        if (availableActions.ContainsKey(type)) {
            return (T)availableActions[type];
        }
        return default;
    }

    public bool EnqueueAction<T>()  where T : ActionInstance
    {
        /* Place the action with the provided keyword arguments into the action
         * execution queue if the action with 'name' is present in the availableActions
         * and is not on cooldown
         */
        T action = GetActionInstance<T>();
        if (action == default) {
            return false;
        }
        if (action.EnqueueAction()) {
            executing.Add(action);
            return true;
        }
        return false;
    }

    public bool HaltAction<T>() where T: ActionInstance{
        /* Stops the execution of the action
         *  Returns the status of this operation
         */
        ActionInstance ac = GetActionInstance<T>();
        if (ac != default && executing.Contains(ac)) {
            ac.HaltActionExecution();
            return true;
        }
        return false;
    }
}
