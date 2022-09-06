using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[Serializable]
[AddComponentMenu("Actionable")]
public class Actionable : MonoBehaviour
{
    /* Component that provides access to a variety of movesets, it acts as a container and platform for
     * different kinds of actions and moves which are provided and fully implemented by associated sub-components
     
    Properties:
    executing: The current actions being executed
    availableActions: The set of actions being executed
    components: The set of sub-components that provides additional data for action instances

     */
    public TypeActionComponentDictionary components = new();
    public ActionInstanceSet executing = new();
    public TypeActionInstanceDictionary availableActions = new();
    [SerializeField] private ActionQueue actionQueue;

    private void Awake()
    {
        foreach (ActionInstance ai in availableActions.Values) {
            ai.Initialize();
        }

        foreach (ActionComponent ac in components.Values) {
            ac.Initialzie();
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
        foreach (ActionInstance ac in availableActions.Values) {
            ac.HaltActionExecution();
        }
    }

    public bool IsInstanceReady<T>() where T: ActionInstance {
        string type = typeof(T).ToString();
        if (availableActions.ContainsKey(type)) {
            return availableActions[type].IsReady;
        }
        return false;
    }

    public bool AddActionComponent<T>() where T:ActionComponent {
        string str = typeof(T).ToString();
        if (components.ContainsKey(str)) {
            return false;
        }
        components[str] = ScriptableObject.CreateInstance<T>();
        return true;
    }

    public T GetActionComponent<T>() where T : ActionComponent{
        string type = typeof(T).ToString();
        if (components.ContainsKey(type)) {
            return (T)components[type];
        }
        return default(T);
    }

    public bool RemoveActionComponent<T>() where T : ActionComponent {
        string str = typeof (T).ToString();
        if (components.ContainsKey(str)) {
            if (RequireActionComponentAttribute.rev_requirement.ContainsKey(typeof(T))) {
                StringBuilder sb = new();
                sb.Append("Cannot remove action component: " + typeof(T).ToString() + ", since the following action instances requires it: ");
                bool flag = false;
                foreach (Type t in RequireActionComponentAttribute.rev_requirement[typeof(T)]) {
                    if (availableActions.ContainsKey(t.ToString())) {
                        flag = true;
                        sb.Append(t.Name);
                        sb.Append(", ");
                    }
                }
                if (flag) {
                    sb.Remove(sb.Length - 2, 2);
                    Debug.LogError(sb.ToString());
                    return false;
                }
            }
        }
        return components.Remove(str);
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

    public bool AddActionInstance<T>() where T:ActionInstance{
        if (GetActionInstance<T>() != default) {
            return false;
        }
        if (actionQueue == null) {
            Debug.LogError("Action Queue is not set up for the component!");
            return false;
        }

        T ai = ScriptableObject.CreateInstance<T>();
        if (ai.ComponentCheck(this)) {
            availableActions.Add(typeof(T).ToString(), ai);
            ai.queue = actionQueue;
            ai.actionComponent = this;
            return true;
        }
        return false;
    }

    public bool RemoveActionInstance<T>() where T:ActionInstance
    {
        return availableActions.Remove(typeof(T).ToString());
    }
}
