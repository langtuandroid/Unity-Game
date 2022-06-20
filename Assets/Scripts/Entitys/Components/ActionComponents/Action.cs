using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ActionComponents { 
    Combat,
    Move
}

[RequireComponent(typeof(Entity))]
[Serializable]
public class Action : MonoBehaviour
{
    /* Entity Component that provides access to a variety of movesets, it acts as a container and platform for
     * different kinds of actions and moves which are provided and fully implemented by associated sub-components
     
    Properties:
    executing: The current actions being executed stored as Dictionary<ActionName, AccessKey>
    availableActions: The set of actions being executed stored as Dictionary<ActionName, RemainingDurationInFrames>. 
    components: The set of sub-components that provides implementation to the available actions stored as Dictionary<ClassName, Sub-component>

     */
    [HideInInspector]
    public ActionComponentDictionary components = new ActionComponentDictionary();
    [HideInInspector]
    public StringIntDictionary executing = new StringIntDictionary();
    [HideInInspector]
    public ActionInstanceDictionary availableActions = new ActionInstanceDictionary();

    public void Update()
    {
        Countdown();
        List<string> removed = new List<string>();
        foreach(string name in executing.Keys)
        {
            int duration = executing[name] - 1;
            if (duration < 0)
            {
                removed.Add(name);
            }
        }
        foreach (string name in removed) { 
            executing.Remove(name);
        }
    }
    public bool HasComponent(ActionComponents name) {
        return components.ContainsKey(name);
    }

    public void AddComponent(ActionComponents name, ActionImplementor component) {
        components[name] = component;
        UpdateActions(name);
    }

    public void UpdateActions(ActionComponents name) {
        if (components.ContainsKey(name)) {
            Dictionary<string, ActionInstance> ats = components[name].AvailableActions();
            foreach (string id in ats.Keys)
            {
                availableActions[id] = ats[id];
            }
        }
    }

    public void RemoveComponent(ActionComponents name) {
        if (!components.ContainsKey(name)) {
            return;
        }
        RemoveActions(name);
        components.Remove(name);
    }

    public void RemoveActions(ActionComponents name) {
        if (!components.ContainsKey(name)) { 
            return ;
        }
        foreach (string id in components[name].AvailableActions().Keys) {
            availableActions.Remove(id);
        }
    }

    public void Countdown()
    {
        Dictionary<string, ActionInstance>.KeyCollection keys = availableActions.Keys;
        foreach (string key in keys)
        {
            availableActions[key].CountDown();
        }
    }

    public ActionImplementor FindActionComponent(string name) {
        /* Returns the key to the sub-component that provides this action, 
         *  If such action does not exist, return empty string
         */
        foreach (KeyValuePair<ActionComponents, ActionImplementor> kvp in components) {
            ActionImplementor r = kvp.Value.GetIdentifier(name);
            if (r != default) {
                return r;
            }
        }
        return default;
    }

    public bool EnqueueAction(string name, Dictionary<string, object> kwargs=null)
    {
        /* Place the action with the provided keyword arguments into the action
         * execution queue if the action with 'name' is present in the availableActions
         * and is not on cooldown
         */
        if (!availableActions.ContainsKey(name)) {
            return false;
        }
        ActionImplementor result = FindActionComponent(name);
        if (result == default) {
            throw new System.Exception();
        }
        if (kwargs == null) {
            kwargs = new Dictionary<string, object>();
        }
        kwargs[Setting.HANDLING_COMPONENT] = result;
        if (availableActions[name].EnqueueAction(kwargs)) {
            executing[name] = availableActions[name].duration;
            return true;
        }
        return false;
    }

    public bool HaltAction(string name) {
        /* Stops the execution of the action
         *  Returns the status of this operation
         */
        if (!executing.ContainsKey(name))
        {
            return false;
        }
        return availableActions[name].HaltActionExecution();
    }

    public bool AlterArguments(string name, Dictionary<string, object> args) {
        if (!executing.ContainsKey(name))
        {
            return false;
        }
        return availableActions[name].AlterArguments(args);
    }
}

[Serializable]
public class ActionInstance
{
    /*An action instance
     * 
     * Properties:
     * priority: Determines the order of the action being executed
     * id: Name of the action
     * action: The delegate function of the action implemented in sub-components 
     * counter: Counter of the action cooldown
     * cooldown: Cooldown of the action in seconds
     * duration: Duration of the action in frames
     * isReady: Whether the action is ready to be executed
     * 
     * Static Members:
     * actionsQueue: The weighed priority queue that stores ActionNames of all of the actions waiting to be executed in order
     */
    public int priority;
    public string id;
    public string action;
    public float cooldown;
    public int duration;

    private float counter;
    private bool isReady;
    private int accessKey;
    private int executing;

    private static WeightedPriorityQueue<ActionDelegate> actionQueue = new WeightedPriorityQueue<ActionDelegate>();

    private struct ActionDelegate{
        public Dictionary<string, object> args;
        public ActionInstance action;

        public ActionDelegate(Dictionary<string, object> args, ActionInstance a) { this.args = args;  action = a;}
    }

    public static void ExecuteActions()
    {
        ActionDelegate a;
        while (!(a = actionQueue.Dequeue()).Equals(default(ActionDelegate)))
        {
            ActionExecutor action = ActionDelegates.Executor(a.action.action);
            action(a.args);
            a.action.executing -= 1;
            if (a.action.executing == 0) {
                a.action.accessKey = -1;
                ActionTerminator exit = ActionDelegates.Terminator(a.action.action);
                exit(a.args);
            }
        }
        actionQueue.Reset();
    }
    public ActionInstance(int p, string i, string a, float c, int d)
    {
        priority = p;
        action = a;
        id = i;
        counter = 0;
        cooldown = c;
        duration = d;
        isReady = false;
        accessKey = -1;
        executing = 0;
     }

    public void CountDown() {
        if (isReady) {
            return;
        }
        counter += Time.deltaTime;
        if (counter >= cooldown) {
            counter = 0;
            isReady = true;
        }
    }

    public bool EnqueueAction(Dictionary<string, object> kwargs) {
        bool result = isReady;
        if (isReady) {
            ActionDelegate a = new ActionDelegate(kwargs, this);
            accessKey = actionQueue.Enqueue(a, priority, duration);
            executing = duration;
            isReady = false;
        }
        return result;
    }

    public bool HaltActionExecution() {
        if (accessKey == -1) {
            return false;
        }
        actionQueue.SetWeight(accessKey, 0);
        accessKey = -1;
        return true;
    }

    public bool AlterArguments(Dictionary<string, object> args) {
        if (accessKey == -1) {
            return false;
        }
        actionQueue.SetValue(accessKey, new ActionDelegate(args, this));
        return true;
    }

    public ActionInstance Clone() {
        return new ActionInstance(priority, id, action, cooldown, duration);
    }
}

[Serializable]
public abstract class ActionImplementor {
    public Entity entity;
    public ActionImplementor(Entity e)
    {
        entity = e;
    }
    public abstract Dictionary<string, ActionInstance> AvailableActions();
    public abstract ActionInstance GetAction(string actionName);
    public abstract bool HasAction(string actionName);

    public abstract ActionImplementor GetIdentifier(string actionName);
}
