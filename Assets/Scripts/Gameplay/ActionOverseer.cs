using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LobsterFramework.Utility;

namespace LobsterFramework.Action{
    public class ActionOverseer : MonoBehaviour
    {
        /* Contains main queue for executing actions, actions are queueed by their actions in ascending order of their priorities.
         * Actions with low priorities will be executed first to allow further execution of higher priority actions to override/modify their 
         * effects.
         */
        private static Dictionary<int, ActionConfigPair> actionQueue = new();
        private static readonly IdDistributor distributor = new();

        internal static int EnqueueAction(ActionConfigPair pair)
        {
            int id = distributor.GetID();
            actionQueue.Add(id, pair);
            return id;
        }

        internal static bool RemoveAction(int key)
        {
            return actionQueue.Remove(key);
        }

        void LateUpdate()
        {
            List<int> removed = new();
            List<int> keys = actionQueue.Keys.ToList();
            keys.Sort((int k1, int k2) => {
                return actionQueue[k1].instance.CompareTo(actionQueue[k2].instance);
            });
            foreach (int key in keys)
            {
                ActionConfigPair ap = actionQueue[key];
                if (!ap.instance.Execute(ap.config))
                {
                    removed.Add(key);
                }
            }

            foreach (int key in removed)
            {
                actionQueue.Remove(key);
            }
        }
    }
}

