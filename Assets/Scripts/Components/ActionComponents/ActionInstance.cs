using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;
using System.Linq;

public struct ActionConfigPair : IComparable<ActionConfigPair>
{
    public string config;
    public ActionInstance instance;
    public ActionConfigPair(ActionInstance instance, string config)
    {
        this.instance = instance;
        this.config = config;
    }

    public int CompareTo(ActionConfigPair other)
    {
        return instance.CompareConfig(config, other.instance, other.config);
    }
}

public abstract class ActionInstance : ScriptableObject
{
    /* An instance of Action that defines the kind of action the parent object can take. Each class of
     * ActionInstance defines its own ActionConfig and can be runned on multiple instances of its ActionConfigs.
     */
    [HideInInspector]
    public Actionable actionComponent;

    [HideInInspector]
    [SerializeField] internal StringActionConfigDictionary configs = new();

    private HashSet<string> executing = new(); 

    // Called by the inspector, add the config with specified name to this ActionInstance. 
    public bool AddConfig(string name) {
        if (HasConfig()) {
            if (configs.ContainsKey(name)) {
                Debug.LogError("The action config with name '" + name + "' is already added!");
                return false;
            }
            Type type = GetType();
            string typeString = type.ToString();
            var m = type.GetMethod("AddConfigGeneric");
            MethodInfo method = m.MakeGenericMethod(Type.GetType(typeString + "+" + typeString + "Config"));
            method.Invoke(this, new[] {name});
            return true;
        }
        Debug.LogError("The action config for '" + GetType().ToString() + "' is not declared!");
        return false;
    }

    public void AddConfigGeneric<T>(string name) where T : ActionConfig {
        string typeString = GetType().ToString();
        if (typeof(T).ToString() != (typeString + "+" + typeString + "Config")) {
            Debug.LogError("Config of type" + typeof(T).ToString() + " cannot be added!");
            return;
        }
        T config = CreateInstance<T>();
        configs.Add(name, config);
        if (AssetDatabase.Contains(this))
        {
            AssetDatabase.AddObjectToAsset(config, this);
            AssetDatabase.SaveAssets();
        }
    }

    // Remove the config with specified name if present
    public bool RemoveConfig(string name) {
        if (!configs.ContainsKey(name)) {
            return false;
        }
        ActionConfig config = configs[name];
        configs.Remove(name);
        DestroyImmediate(config, true);
        AssetDatabase.SaveAssets();
        return true;
    }

    // Callback when the gameobject is about to be disabled or destroyed
    public virtual void CleanUp() { }

    // Comparer used to sort action by their priority
    public int CompareConfig(string config, ActionInstance other, string o_config)
    {
        return configs[config].actionData.Value.Priority - other.configs[o_config].actionData.Value.Priority;
    }

    // Check if the action component satisfies the requirements of the ActionInstance defined by its attributes.
    public bool ComponentCheck(Actionable actionComponent)
    {
        Type type = GetType();
        bool f2 = true;
        // Check of gameobject Components
        if (RequireCMPAttribute.requirement.ContainsKey(type))
        {
            HashSet<Type> ts2 = RequireCMPAttribute.requirement[type];

            List<Type> lst2 = new List<Type>();
            foreach (Type t in ts2)
            {
                if (actionComponent.GetComponent(t.ToString()) == null)
                {
                    f2 = false;
                    lst2.Add(t);
                }
            }
            if (!f2)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Missing Component: ");
                foreach (Type t in lst2)
                {
                    sb.Append(t.Name);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                Debug.LogError(sb.ToString());
            }
        }
        return f2;
    }

    // Override this to imeplement additional Skill Ready Check
    protected virtual bool ConditionSatisfied(ActionConfig config) { return true; }

    // Update cooldown
    public void CountDown()
    {
        foreach (ActionConfig config in configs.Values)
        {
            config.CountDown();
        }
    }

    // Add the action with specified configuration to the action queue for execution
    public bool EnqueueAction(string config)
    {
        if (IsReady(config))
        {
            configs[config].accessKey = ActionOverseer.EnqueueAction(new ActionConfigPair(this, config));
            configs[config].OnCooldown = true;
            executing.Add(config);
            OnEnqueue(configs[config]);
        }
        return false;
    }

    // Execute the config with the provided name. Assumes the action is already in action queue and is only being called by ActionOverseer.
    public bool Execute(string name)
    {
        try
        {
            ActionConfig config = configs[name];
            bool result = ExecuteBody(config);
            if (!result || config.isSuspended)
            {
                config.accessKey = -1;
                config.isSuspended = false;
                executing.Remove(name);
                OnActionFinish(config);
            }
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError("Action '" + GetType().ToString() + "' failed to execute with config + '" + name + "':\n " + ex.ToString());
        }
        return false;
    }

    // Get the number of configs currently running
    public int RunningCount() { return executing.Count; }

    // Suspend the execution of provided action and force it to finish at the current frame
    public bool HaltActionExecution(string name)
    {
        if (!configs.ContainsKey(name))
        {
            return false;
        }
        ActionConfig config = configs[name];
        if (config.accessKey == -1)
        {
            return false;
        }
        config.isSuspended = true;
        return true;
    }

    public void HaltActions()
    {
        foreach (string name in configs.Keys)
        {
            HaltActionExecution(name);
        }
    }

    // Callback when the gameobject is being initialized during Start(). Do not initialize action configs in this method! Override This!
    protected virtual void Initialize() { }

    // Check if the provided config is executing, this method will return false if the config is not present
    public bool IsExecuting(string name)
    {
        return executing.Contains(name);
    }

    // Check if the provided config is ready for execution (to be added to the action queue).
    public bool IsReady(string name)
    {
        if (!configs.ContainsKey(name)) {
            Debug.LogError(GetType().ToString() + " does not have config with name '" + name + "'");
            return false;
        }
        ActionConfig config = configs[name];
        return !config.IsExecuting && !config.OnCooldown && ConditionSatisfied(config);
    }

    // Called when the parent component is being initialized
    public void OnStartUp()
    {
        if (!HasConfig())
        {
            Debug.LogError("The action config for '" + GetType().ToString() + "' is not declared!");
        }
        Initialize();
        foreach (ActionConfig config in configs.Values)
        {
            config.Initialize();
        }
    }

    // Called when the parent object is destroyed, this 
    public void OnTermination()
    {
        CleanUp();
        foreach (ActionConfig config in configs.Values)
        {
            config.Close();
        }
    }

    // Reset, called when the parent component is resetted
    public void ResetStatus()
    {
        CleanUp();
        Initialize();
    }

    public void OnDestroy()
    {

        foreach (string name in configs.Keys.ToList()) {
            RemoveConfig(name);
        }
    }

    public void SaveConfigsAsAsset() {
        foreach (ActionConfig config in configs.Values) {
            AssetDatabase.AddObjectToAsset(config, this);
        }
    }

    // Main body of the action execution, override this to create different actions!
    protected abstract bool ExecuteBody(ActionConfig config);

    // Callback when the action is finished
    protected virtual void OnActionFinish(ActionConfig config) { }

    // Callback when the action is queued
    protected virtual void OnEnqueue(ActionConfig config) { }

    private bool HasConfig()
    {
        Type type = GetType();
        string typeName = type.ToString() + "+" + type.ToString() + "Config";
        foreach (Type innerType in type.GetNestedTypes())
        {
            string inner = innerType.ToString();
            if (inner.Equals(typeName))
            {
                return true;
            }
        }
        return false; 
    }

    [Serializable]
    public class ActionConfig : ScriptableObject
    {
        /* A configuration of the ActionInstance, each configuration has its own settings that affects the execution of the ActionInstance
         */
        [HideInInspector]
        public int accessKey = -1;
        public RefActionData actionData;
        private float cdCounter = 0;
        public bool IsExecuting { get { return accessKey != -1; } }
        public bool OnCooldown { get; set; }

        public bool isSuspended = false;

        // Override this to perform any additional setups on destruction of parent object (i.e Remove links to unity action/events)
        public virtual void Close() { }

        public void CountDown()
        {
            if (OnCooldown)
            {
                cdCounter += Time.deltaTime;
                if (cdCounter >= actionData.Value.Cooldown)
                {
                    cdCounter = 0;
                    OnCooldown = false;
                }
            }
        }

        // Override this to initialize action config on start up
        public virtual void Initialize() { }
    }
}
