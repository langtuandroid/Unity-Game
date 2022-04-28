using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : Entity.EntityComponent, Entity.IEntityComponentUpdate
{
    /* Entity Component that provides access to a variety of movesets, it acts as a container and platform for
     * different kinds of actions and moves which are provided and fully implemented by associated sub-components
     
    Properties:
    executing: The current actions being executed stored as Dictionary<ActionName, AccessKey>
    availableActions: The set of actions being executed stored as Dictionary<ActionName, RemainingDurationInFrames>. 
    components: The set of sub-components that provides implementation to the available actions stored as Dictionary<ClassName, Sub-component>

     */
    private Dictionary<string, IActionImplementor> components = new Dictionary<string, IActionImplementor>();
    private Dictionary<string, int> executing = new Dictionary<string, int>();
    private Dictionary<string, ActionInstance> availableActions = new Dictionary<string, ActionInstance>();

    public Action(Entity e) : base(e){ e.SetEntityComponent(Setting.COMPONENT_ACTION, this); }

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
    public bool HasComponent(string name) {
        return components.ContainsKey(name);
    }

    public void AddComponent(string name, IActionImplementor component) {
        components[name] = component;
        UpdateActions(name);
    }

    public void UpdateActions(string name) {
        if (components.ContainsKey(name)) {
            Dictionary<string, ActionInstance> ats = components[name].AvailableActions();
            foreach (string id in ats.Keys)
            {
                availableActions[id] = ats[id];
            }
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

    public IActionImplementor FindActionComponent(string name) {
        /* Returns the key to the sub-component that provides this action, 
         *  If such action does not exist, return empty string
         */
        foreach (KeyValuePair<string, IActionImplementor> kvp in components) {
            IActionImplementor r = kvp.Value.GetIdentifier(name);
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
        IActionImplementor result = FindActionComponent(name);
        if (result == default) {
            throw new System.Exception();
        }
        if (kwargs == null) {
            kwargs = new Dictionary<string, object>();
        }
        kwargs["_component"] = result;
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

public delegate void ActionExecutor(Dictionary<string, object> args);
public delegate void ActionTerminator(Dictionary<string, object> args);
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
    public ActionExecutor action;
    public ActionTerminator exit;
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
            a.action.action(a.args);
            a.action.executing -= 1;
            if (a.action.executing == 0) {
                a.action.accessKey = -1;
                a.action.exit(a.args);
            }
        }
        actionQueue.Reset();
    }
    public ActionInstance(int p, string i, ActionExecutor a, ActionTerminator e, float c, int d)
    {
        priority = p;
        action = a;
        exit = e; 
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
        return new ActionInstance(priority, id, action, exit, cooldown, duration);
    }
}

public interface IActionImplementor { 
    public Dictionary<string, ActionInstance> AvailableActions();
    public ActionInstance GetAction(string actionName);
    public bool HasAction(string actionName);

    public IActionImplementor GetIdentifier(string actionName);
}
