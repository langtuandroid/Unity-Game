using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Action : Entity.EntityComponent, Entity.IEntityComponentUpdate
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

    public Action(Entity e) : base(e){

    }

    public void Update()
    {
        foreach (KeyValuePair<string, IActionImplementor> kvp in components) {
            kvp.Value.Countdown();
        }
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
        Dictionary<string, ActionInstance> ats = component.AvailableActions();
        foreach (string id in ats.Keys) {
            availableActions[id] = ats[id];
        }
    }

    public string FindActionComponent(string name) {
        /* Returns the key to the sub-component that provides this action, 
         *  If such action does not exist, return empty string
         */
        foreach (KeyValuePair<string, IActionImplementor> kvp in components) {
            if (kvp.Value.AvailableActions().ContainsKey(name)) {
                return kvp.Key;
            }
        }
        return "";
    }

    public bool EnqueueAction(string name, Dictionary<string, object> kwargs=null)
    {
        string result = FindActionComponent(name);
        if (result != "")
        {
            if (kwargs == null) {
                kwargs = new Dictionary<string, object>();
            }
            kwargs["_component"] = components[result];
            if (components[result].GetAction(name).EnqueueAction(kwargs)) {
                executing[name] = availableActions[name].duration;
                return true;
            }
            return false;
        }
        return false;
    }
}

public delegate void ActionExecutor(Dictionary<string, object> args);
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
    public float cooldown;
    public int duration;

    private float counter;
    private bool isReady;

    private static WeightedPriorityQueue<ActionDelegate> actionQueue = new WeightedPriorityQueue<ActionDelegate>();

    private class ActionDelegate{
        public Dictionary<string, object> args;
        public ActionExecutor action;
    }

    public static void ExecuteActions()
    {
        while (actionQueue.Size() > 0)
        {
            ActionDelegate a = actionQueue.Dequeue();
            a.action(a.args);
        }
    }
    public ActionInstance(int p, string i, ActionExecutor a, float c, int d)
    {
        priority = p;
        action = a;
        id = i;
        counter = 0;
        cooldown = c;
        duration = d;
        if (cooldown == 0) {
            cooldown = -1; // No cooldown
        }
        isReady = false;
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
            ActionDelegate a = new ActionDelegate();
            a.args = kwargs;
            a.action = action;
            actionQueue.Enqueue(a, priority, duration);
            isReady = false;
        }
        return result;
    }
}

public interface IActionImplementor { 
    public Dictionary<string, ActionInstance> AvailableActions();
    public ActionInstance GetAction(string actionName);
    public bool HasAction(string actionName);

    public void Countdown();
}
