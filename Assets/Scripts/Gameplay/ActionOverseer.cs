using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionOverseer : MonoBehaviour
{
    /* Contains main queue for executing actions, actions are queueed by their actions in ascending order of their priorities.
     * Actions with low priorities will be executed first to allow further execution of higher priority actions to override/modify their 
     * effects.
     */
    private static Dictionary<int, ActionInstance> actionQueue = new();
    private static readonly IdDistributor distributor = new();

    public static int EnqueueAction(ActionInstance instance) {
        int id = distributor.GetID();
        actionQueue.Add(id, instance);
        return id;
    }

    public static bool RemoveAction(int key) {
        return actionQueue.Remove(key);
    }

    void LateUpdate()
    {
        List<int> removed = new();
        List<int> keys = actionQueue.Keys.ToList();
        keys.Sort((int k1, int k2) => {
            return actionQueue[k1].CompareTo(actionQueue[k2]);
        });
        foreach (int key in keys) { 
            ActionInstance instance = actionQueue[key];
            if (!instance.Execute()) {
                removed.Add(key);
            }
        }

        foreach (int key in removed) {
            actionQueue.Remove(key);
        }
    }
}
