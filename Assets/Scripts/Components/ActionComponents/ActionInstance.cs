using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
     * actionsQueue: The weighed priority queue that stores ActionNames of all of the actions waiting to be executed in order
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
        if (isReady || actionData == null)
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
                return queue.Queue.GetWeight(accessKey);
            }
            return 0;
        }
    }

    public bool EnqueueAction()
    {
        bool result = isReady;
        if (isReady && actionData != null)
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
    public virtual bool ComponentCheck(Actionable actionComponent) {
        return true;
    }

    public bool HaltActionExecution()
    {
        if (accessKey == -1)
        {
            return false;
        }
        queue.Queue.SetWeight(accessKey, 0);
        accessKey = -1;
        return true;
    }

    public ActionData Data {
        get {
            return actionData.Value;
        }
    }
}
