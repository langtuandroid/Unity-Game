using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Text;

public abstract class ActionInstance : ScriptableObject
{
    /*An action instance
     * 
     * Properties:
     * priority: Determines the order of the action being executed
     * counter: Counter of the action cooldown
     * cooldown: Cooldown of the action in seconds
     * duration: Duration of the action in frames
     * isReady: Whether the action is ready to be executed
     * 
     * Static Members:
     * actionsQueue: The weighed priority queue that manages actions waiting to be executed
     */
    [HideInInspector]
    public ActionQueue queue;
    [HideInInspector]
    public Actionable actionComponent;
    [SerializeField] protected RefActionData actionData;

    private float counter = 0;
    private int accessKey = -1;

    public bool OnCooldown { get; private set; }

    public bool IsReady { get { return !OnCooldown && ConditionSatisfied(); } }

    public int Executing {
        get {
            if (accessKey != -1) {
                return queue.Queue.GetElementData(accessKey);
            }
            return 0;
        }
    }

    // Override this to imeplement additional Skill Ready Check
    public virtual bool ConditionSatisfied() { return true; }

    // To be overriden
    protected virtual void OnEnqueue() { }

    public void CountDown()
    {
        if (OnCooldown)
        {
            counter += Time.deltaTime;
            if (counter >= Data.Cooldown)
            {
                counter = 0;
                OnCooldown = false;
            }
        }
    }

    public bool EnqueueAction()
    {
        if (IsReady)
        {
            if (queue == null) {
                Debug.LogWarning("Action Queue is not set up for the instance: " + GetType().ToString());
                return false;
            }
            accessKey = queue.Queue.Enqueue(this, Data.Duration, Data.Priority);
            OnEnqueue();
            OnCooldown = true;
        }
        return false;
    }

    public virtual void Execute() {
        try
        {
            ExecuteBody();
        }
        catch (Exception ex) {
            Debug.LogError(ex.ToString());
        }
        if (!queue.Queue.ContainsKey(accessKey))
        {
            accessKey = -1;
            Terminate();
        }
    }

   // Override This!
    protected abstract void ExecuteBody();

    // Override This!

    protected virtual void Terminate() { }

    // Override This!
    public virtual void Initialize() { }

    public virtual bool ComponentCheck(Actionable actionComponent) {
        Type type = GetType();

        bool result = true;
        bool f1 = false;
        bool f2 = false;
        if (RequireActionComponentAttribute.requirement.ContainsKey(type))
        {
            HashSet<Type> ts = RequireActionComponentAttribute.requirement[type];
            MethodInfo m = typeof(Actionable).GetMethod("GetActionComponent");

            List<Type> lst1 = new List<Type>();
            foreach (Type t in ts)
            {
                MethodInfo mm = m.MakeGenericMethod(t);
                if (mm.Invoke(actionComponent, null) == null)
                {
                    f1 = true;
                    lst1.Add(t);
                }
            }

            if (f1)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Missing ActionComponent: ");
                foreach (Type t in lst1)
                {
                    sb.Append(t.Name);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                Debug.LogError(sb.ToString());
            }
        }

        // Check of gameobject Components
        if (RequireCMPAttribute.requirement.ContainsKey(type))
        {
            HashSet<Type> ts2 = RequireCMPAttribute.requirement[type];

            List<Type> lst2 = new List<Type>();
            foreach (Type t in ts2)
            {
                if (actionComponent.GetComponent(t.ToString()) == null)
                {
                    f2 = true;
                    lst2.Add(t);
                }
            }
            if (f2)
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

        if (f1 || f2)
        {
            result = false;
        }

        return result;
    }

    public bool HaltActionExecution()
    {
        if (accessKey == -1)
        {
            return false;
        }
        queue.Queue.Remove(accessKey);
        accessKey = -1;
        return true;
    }

    public ActionData Data {
        get {
            return actionData.Value;
        }
    }
}
