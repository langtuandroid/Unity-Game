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
    private bool isReady = false;
    private int accessKey = -1;

    public void CountDown()
    {
        if (isReady)
        {
            return;
        }
        counter += Time.deltaTime;
        if (counter >= Data.Cooldown)
        {
            counter = 0;
            isReady = true;
        }
    }

    public int Executing {
        get {
            if (accessKey != -1) {
                return queue.Queue.GetElementData(accessKey);
            }
            return 0;
        }
    }

    public bool EnqueueAction()
    {
        bool result = isReady;
        if (isReady)
        {
            if (queue == null) {
                Debug.LogWarning("Action Queue is not set up for the instance: " + this.GetType().ToString());
                return false;
            }
            accessKey = queue.Queue.Enqueue(this, Data.Priority, Data.Duration);
            isReady = false;
        }
        return result;
    }

    public virtual void Execute() {
        ExecuteBody();
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
    protected virtual void AssignFields() { }

    public virtual bool ComponentCheck(Actionable actionComponent) {
        bool result = CheckComponent(actionComponent);
        if (result) {
            AssignFields();
        }
        return result;
    }

    private bool CheckComponent(Actionable actionComponent) {
        Type type = GetType();

        bool result = true;
        bool f1 = false;
        bool f2 = false;
        if (RequireActionComponentAttribute.requirement.ContainsKey(type)) {
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
        if (RequireCMPAttribute.requirement.ContainsKey(type)) {
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
        
        if (f1 || f2) { 
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
        queue.Queue.SetElementData(accessKey, 0);
        accessKey = -1;
        return true;
    }

    public ActionData Data {
        get {
            return actionData.Value;
        }
    }
}
