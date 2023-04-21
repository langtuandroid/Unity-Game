using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LobsterFramework.Utility;

namespace LobsterFramework.AbilitySystem{
    public class ActionOverseer : MonoBehaviour
    {
        /* Contains main queue for executing actions, actions are queueed in ascending order of their priorities.
         * Actions with low priorities will be executed first to allow further execution of higher priority actions to override/modify their 
         * effects.
         */
        private static Dictionary<int, AbilityConfigPair> abilityQueue = new();
        private static readonly IdDistributor distributor = new();

        internal static int EnqueueAction(AbilityConfigPair pair)
        {
            int id = distributor.GetID();
            abilityQueue.Add(id, pair);
            return id;
        }

        internal static bool RemoveAction(int key)
        {
            return abilityQueue.Remove(key);
        }

        void LateUpdate()
        {
            List<int> removed = new();
            List<int> keys = abilityQueue.Keys.ToList();
            keys.Sort((int k1, int k2) => {
                return abilityQueue[k1].ability.CompareByExecutionPriority(abilityQueue[k2].ability);
            });
            foreach (int key in keys)
            {
                AbilityConfigPair ap = abilityQueue[key];
                if (!ap.ability.Execute(ap.config))
                {
                    removed.Add(key);
                }
            }

            foreach (int key in removed)
            {
                abilityQueue.Remove(key);
            }
        }
    }
}

