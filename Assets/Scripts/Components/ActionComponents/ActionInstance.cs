using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Text;

public abstract class ActionInstance : ScriptableObject, IComparable<ActionInstance>
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
    public Actionable actionComponent;
    [SerializeField] protected RefActionData actionData;

    private float cdCounter = 0;
    private int accessKey = -1;

    public bool OnCooldown { get; private set; }

    public bool IsExecuting { get { return accessKey != -1; } }

    public bool IsReady { get { return !IsExecuting && !OnCooldown && ConditionSatisfied(); } }

    // Override this to imeplement additional Skill Ready Check
    public virtual bool ConditionSatisfied() { return true; }

    // Callback when the action is queued
    protected virtual void OnEnqueue() { }

    // Update cooldown
    public void CountDown()
    {
        if (OnCooldown)
        {
            cdCounter += Time.deltaTime;
            if (cdCounter >= Data.Cooldown)
            {
                cdCounter = 0;
                OnCooldown = false;
            }
        }
    }

    public bool EnqueueAction()
    {
        if (IsReady)
        {
            accessKey = ActionOverseer.EnqueueAction(this);
            OnEnqueue();
            OnCooldown = true;
        }
        return false;
    }

    public virtual bool Execute() {
        try
        {
            bool result = ExecuteBody();
            if (!result) {
                accessKey = -1;
                Terminate();
            }
            return result;
        }
        catch (Exception ex) {
            Debug.LogError("Action failed to execute: " + ex.ToString());
        }
        return false;
    }

   // Main body of the action execution. Override This!
    protected abstract bool ExecuteBody();

    // Callback when the action is finished. Override This!
    protected virtual void Terminate() { }

    // Callback when the gameobject is being initialized during Start(). Override This!
    public virtual void Initialize() { }

    // Override This!

    public virtual void CleanUp() { }

    public void ResetStatus() {
        CleanUp();
        Initialize();
    }

    public bool ComponentCheck(Actionable actionComponent) {
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

    public bool HaltActionExecution()
    {
        if (accessKey == -1)
        {
            return false;
        }
        ActionOverseer.RemoveAction(accessKey);
        accessKey = -1;
        return true;
    }

    public int CompareTo(ActionInstance other)
    {
        return actionData.Value.Priority - other.actionData.Value.Priority;
    }

    public ActionData Data {
        get {
            return actionData.Value;
        }
    }
}
