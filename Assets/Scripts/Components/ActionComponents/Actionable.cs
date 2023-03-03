using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;

[AddComponentMenu("Actionable")]
[RequireComponent(typeof(Entity))]
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
    public ActionSet executing = new();
    [HideInInspector]
    [SerializeField] private ActionableData actionableData;
    [SerializeField] private string assetName;
    private Dictionary<string, ActionInstance> availableActions;
    private TypeActionComponentDictionary components;
    [SerializeField] private ActionableData data;
    private Entity entity;

    public bool EnqueueAction<T>(string name) where T : ActionInstance
    {
        if (entity.ActionBlocked)
        {
            return false;
        }
        T action = GetActionInstance<T>();
        if (action == default)
        {
            return false;
        }
        if (action.EnqueueAction(name))
        {
            executing.Add(action);
            return true;
        }
        return false;
    }

    public bool EnqueueAction<T>() where T : ActionInstance
    {
        return EnqueueAction<T>("default");
    }

    public T GetActionComponent<T>() where T : ActionComponent
    {
        string type = typeof(T).ToString();
        if (components.ContainsKey(type))
        {
            return (T)components[type];
        }
        return default(T);
    }

    public bool HaltAction<T>(string name) where T : ActionInstance
    {
        /* Stops the execution of the action
         *  Returns the status of this operation
         */
        ActionInstance ac = GetActionInstance<T>();
        if (ac != default && executing.Contains(ac))
        {
            ac.HaltActionExecution(name);
            return true;
        }
        return false;
    }

    public bool IsInstanceReady<T>(string config) where T : ActionInstance
    {
        string type = typeof(T).ToString();
        if (availableActions.ContainsKey(type))
        {
            return availableActions[type].IsReady(config);
        }
        return false;
    }

    public bool IsInstanceReady<T>() where T : ActionInstance
    { 
        return IsInstanceReady<T>("default");
    }

    public void Reset()
    {
        foreach (ActionComponent component in components.Values)
        {
            component.Reset();
        }
        foreach (ActionInstance ac in availableActions.Values)
        {
            ac.ResetStatus();
        }
    }

    public void SaveActionableData()
    {
        if (actionableData != null)
        {
            AssetDatabase.CreateAsset(actionableData, "Assets/Resources/Scriptable Objects/Action/ActionableData/" + assetName + ".asset");
            actionableData.SaveContentsAsAsset();
            data = actionableData;
            actionableData = Instantiate(actionableData);
            actionableData.CopyActionAsset();
        }
    }

    public void SetActionableData()
    {
        if (data != null)
        {
            if (actionableData != null && !AssetDatabase.Contains(actionableData))
            {
                DestroyImmediate(actionableData, true);
            }
            actionableData = Instantiate(data);
            actionableData.CopyActionAsset();
        }
    }

    private void Awake()
    {
        actionableData = Instantiate(data);
        actionableData.CopyActionAsset();
        actionableData.identifier = GetInstanceID();
    }

    private void Countdown()
    {
        foreach (ActionInstance ac in availableActions.Values)
        {
            ac.CountDown();
        }
    }

    private T GetActionInstance<T>() where T : ActionInstance
    {
        string type = typeof(T).ToString();
        if (availableActions.ContainsKey(type))
        {
            return (T)availableActions[type];
        }
        return default;
    }

    private void OnDestroy()
    {
        foreach (ActionInstance ac in actionableData.availableActions.Values)
        {
            ac.HaltActions();
            ac.OnTermination();
        }
        foreach (ActionComponent ac in actionableData.components.Values)
        {
            ac.CleanUp();
        }
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
        actionableData.Initialize(this);
        availableActions = actionableData.availableActions;
        entity = GetComponent<Entity>();
    }
    private void Update()
    {
        UpdateComponents();
        Countdown();
        List<ActionInstance> removed = new();
        foreach (ActionInstance ac in executing)
        {
            if (entity.ActionBlocked) {
                ac.HaltActions(); 
            }
            if (ac.RunningCount() == 0)
            {
                removed.Add(ac);
            }
        }
        foreach (ActionInstance ac in removed)
        {
            executing.Remove(ac);
        }
    }
    private void UpdateComponents() {
        foreach (ActionComponent c in components.Values) {
            c.Update();
        }
    }
}
