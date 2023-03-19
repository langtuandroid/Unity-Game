using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LobsterFramework.Action;
using LobsterFramework.Utility;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.Action {
    /// <summary>
    /// Component that provides access to a variety of movesets, it acts as a container and platform for 
    /// different kinds of actions and moves which are provided and fully implemented by associated sub-components.
    /// Requires actionable data to be supplied. 
    /// </summary>
    [AddComponentMenu("Actionable")]
    [RequireComponent(typeof(Entity))]
    public class Actionable : MonoBehaviour
    {
        private ActionSet executing = new();
        [HideInInspector] 
        [SerializeField] private ActionableData actionableData;
        private Dictionary<string, ActionInstance> availableActions;
        private TypeActionComponentDictionary components;
        [SerializeField] private ActionableData data;
        private Entity entity;
        private (string, ActionInstance.ActionConfig) animatingConfig;
        private Animator animator;

        /// <summary>
        /// Add the action with specified type (T) and config (name) to the executing queue, return the status of this operation. <br/>
        /// For this operation to be successful, the following must be satisfied: <br/>
        /// 1. The entity must not be action blocked. (i.e Not affected by stun effect) <br/>
        /// 2. The specified action of Type (T) as well as the config (name) must be present in the actionable data.<br/>
        /// 3. The precondition of the specified action must be satisfied as well as the cooldown of the config if the action uses cooldowns. <br/>
        /// 4. The specified action must not be currently running or enqueued. <br/>
        /// 5. There must be no other actions with higher enqueue priority in the same action group executing by the entity. <br/>
        /// <br/>
        /// Note that this method should only be called inside Update(), calling it elsewhere will have unpredictable results.
        /// </summary>
        /// <typeparam name="T">Type of the Action to be enqueued</typeparam>
        /// <param name="name">Name of the configuration to be enqueued</param>
        /// <returns></returns>
        public bool EnqueueAction<T>(string name) where T : ActionInstance
        {
            if (entity.ActionBlocked)
            {
                return false;
            }
            T action = GetActionInstance<T>();
            if (action == default)
            {
                return false;
            }
            if (action.EnqueueAction(name))
            { 
                executing.Add(action);
                return true;
            }
            return false;
        }


        /// <summary>
        /// A shortcut for enqueuing the action with default config, see EnqueueAction&lt;T&gt;(string name) for more details 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool EnqueueAction<T>() where T : ActionInstance
        {
            return EnqueueAction<T>("default");
        }

        public T GetActionComponent<T>() where T : ActionComponent
        {
            string type = typeof(T).ToString();
            if (components.ContainsKey(type))
            {
                return (T)components[type];
            }
            return default(T);
        }

        /// <summary>
        /// Stops the execution of the action and returns the status of this operation
        /// </summary>
        /// <typeparam name="T">Type of the action to be halted</typeparam>
        /// <param name="name">Name of the config of T to be halted</param>
        /// <returns></returns>
        public bool HaltAction<T>(string name) where T : ActionInstance
        {
            ActionInstance ac = GetActionInstance<T>();
            if (ac != default && executing.Contains(ac))
            {
                ac.HaltActionExecution(name);
                return true;
            }
            return false;
        }

        public bool IsInstanceReady<T>(string config) where T : ActionInstance
        {
            string type = typeof(T).ToString();
            if (availableActions.ContainsKey(type))
            {
                return availableActions[type].IsReady(config);
            }
            return false;
        }

        public bool IsInstanceReady<T>() where T : ActionInstance
        {
            return IsInstanceReady<T>("default");
        }

        public void Reset()
        {
            foreach (ActionComponent component in components.Values)
            {
                component.Reset();
            }
            foreach (ActionInstance ac in availableActions.Values)
            {
                ac.ResetStatus();
            }
        }

        public void SaveActionableData(string assetName)
        {
            if (assetName == "") {
                Debug.LogError("Asset name cannot be empty!", this);
                return;
            }
            if (actionableData != null)
            {
                AssetDatabase.CreateAsset(actionableData, "Assets/Resources/Scriptable Objects/Action/ActionableData/" + assetName + ".asset");
                actionableData.SaveContentsAsAsset();
                data = actionableData;
                actionableData = Instantiate(actionableData);
                actionableData.CopyActionAsset();
            }
        }

        public void SetActionableData()
        {
            if (data != null)
            {
                if (actionableData != null && !AssetDatabase.Contains(actionableData))
                {
                    DestroyImmediate(actionableData, true);
                }
                actionableData = Instantiate(data);
                actionableData.CopyActionAsset();
            }
        }

        private void Awake()
        {
            actionableData = Instantiate(data);
            actionableData.CopyActionAsset();
            actionableData.identifier = GetInstanceID();
        }

        private T GetActionInstance<T>() where T : ActionInstance
        {
            string type = typeof(T).ToString();
            if (availableActions.ContainsKey(type))
            {
                return (T)availableActions[type];
            }
            return default;
        }

        private void OnDestroy()
        {
            foreach (ActionInstance ac in actionableData.availableActions.Values)
            {
                ac.HaltActions();
                ac.OnTermination();
            }
            foreach (ActionComponent ac in actionableData.components.Values)
            {
                ac.CleanUp();
            }
        }

        private void Start()
        {
            if (actionableData == null)
            {
                Debug.LogWarning("Actionable Data is not set!");
                return;
            }
            int id = GetInstanceID();
            if (actionableData.identifier != id)
            {
                actionableData = Instantiate(actionableData);
                actionableData.identifier = id;
                actionableData.CopyActionAsset();
            }
            components = actionableData.components;
            actionableData.Initialize(this);
            availableActions = actionableData.availableActions;
            entity = GetComponent<Entity>();
            TryGetComponent<Animator>(out animator);
        }
        private void Update()
        {
            UpdateComponents();
            List<ActionInstance> removed = new();
            foreach (ActionInstance ac in executing)
            {
                if (entity.ActionBlocked)
                {
                    ac.HaltActions();
                }
                if (ac.RunningCount() == 0)
                {
                    removed.Add(ac);
                }
            }
            foreach (ActionInstance ac in removed)
            {
                executing.Remove(ac);
            }
        }
        private void UpdateComponents()
        {
            foreach (ActionComponent c in components.Values)
            {
                c.Update();
            }
        }

        public void RegisterAnimation(string actionInstance, ActionInstance.ActionConfig config, string animation)
        {
            animatingConfig = (actionInstance, config);
            animator.SetBool("isActing", true);
            animator.Play(animation);
        }

        public void UnregisterAnimation()
        {
            animatingConfig = ("", null);
            animator.SetBool("isActing", false);
        }

        public void AnimationSignal()
        {
            (string ac, ActionInstance.ActionConfig config) = animatingConfig;
            availableActions[ac].AnimationSignal(config);
        }
    }

}

