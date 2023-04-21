using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using LobsterFramework.Utility;

namespace LobsterFramework.AbilitySystem {
    /// <summary>
    /// Component that provides access to a variety of movesets, it acts as a container and platform for 
    /// different kinds of abilitiess and moves which are provided and fully implemented by associated sub-components.
    /// Requires actionable data to be supplied. 
    /// </summary>
    [AddComponentMenu("AbilityRunner")]
    public class AbilityRunner : MonoBehaviour
    {
        
        public UnityAction<bool> onActionBlocked;

        private AbilitySet executing = new();
        [HideInInspector] 
        [SerializeField] private AbilityData abilityData;
        private Dictionary<string, Ability> availableAbilities;
        private TypeAbilityStatDictionary components;
        [SerializeField] private AbilityData data;
        private (string, Ability.AbilityConfig) animatingConfig;
        private Animator animator;
        private StackedBool actionBlocked = new();
        
        public bool ActionBlocked {
            get { return actionBlocked.Stat(); }
        }

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
        public bool EnqueueAbility<T>(string name) where T : Ability
        {
            if (ActionBlocked)
            {
                return false;
            }
            T action = GetAbility<T>();
            if (action == default)
            {
                return false;
            }
            if (action.EnqueueAbility(name))
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
        public bool EnqueueAbility<T>() where T : Ability
        {
            return EnqueueAbility<T>("default");
        }

        /// <summary>
        /// Get the action component of type T attached to this Component if it is present.
        /// </summary>
        /// <typeparam name="T">Type of the ActionComponent being requested</typeparam>
        /// <returns>Return the action component if it is present, otherwise null</returns>
        public T GetActionComponent<T>() where T : AbilityStat
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
        /// <returns>Return the status of this operation</returns>
        public bool HaltAbility<T>(string name) where T : Ability
        {
            Ability ac = GetAbility<T>();
            if (ac != default && executing.Contains(ac))
            {
                ac.HaltAbilityExecution(name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the action instance of type T with config is ready. <br/>
        /// If T is not present or config is not available, return false.
        /// </summary>
        /// <typeparam name="T">Type of the ActionInstance to be queried</typeparam>
        /// <param name="config">Name of the config to be queried</param>
        /// <returns>The result of the query</returns>
        public bool IsAbilityReady<T>(string config) where T : Ability
        {
            string type = typeof(T).ToString();
            if (availableAbilities.ContainsKey(type))
            {
                return availableAbilities[type].IsReady(config);
            }
            return false;
        }

        /// <summary>
        /// A shortcut for IsInstanceReady&ltT&gt("default")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsAbilityReady<T>() where T : Ability
        {
            return IsAbilityReady<T>("default");
        }

        public int BlockAction()
        {
            bool before = actionBlocked.Stat();
            int id = actionBlocked.AddEffector(true);
            if (onActionBlocked != null && before != actionBlocked.Stat())
            {
                onActionBlocked.Invoke(true);
            }
            return id;
        }

        public bool UnblockAction(int id) {
            if (actionBlocked.RemoveEffector(id)) {
                if (actionBlocked.Stat() && onActionBlocked != null) {
                    onActionBlocked.Invoke(false);
                }
                return true;
            }
            return false;
        }

        public void Reset()
        {
            foreach (AbilityStat component in components.Values)
            {
                component.Reset();
            }
            foreach (Ability ac in availableAbilities.Values)
            {
                ac.ResetStatus();
            }
        }

        /// <summary>
        /// Only to be called inside play mode! Save the current actionable data as an asset with specified assetName to the default path.
        /// </summary>
        /// <param name="assetName">Name of the asset to be saved</param>
        public void SaveActionableData(string assetName)
        {
            if (assetName == "") {
                Debug.LogError("Asset name cannot be empty!", this);
                return;
            }
            if (abilityData != null)
            {
                AssetDatabase.CreateAsset(abilityData, "Assets/Resources/Scriptable Objects/Action/ActionableData/" + assetName + ".asset");
                abilityData.SaveContentsAsAsset();
                data = abilityData;
                abilityData = Instantiate(abilityData);
                abilityData.CopyActionAsset();
            }
        }

        /// <summary>
        /// Method to be called by the editor, duplicate the actionable data to allow play mode modification without affecting the asset.
        /// </summary>
        public void SetActionableData()
        {
            if (data != null)
            {
                if (abilityData != null && !AssetDatabase.Contains(abilityData))
                {
                    DestroyImmediate(abilityData, true);
                }
                abilityData = Instantiate(data);
                abilityData.CopyActionAsset();
            }
        }

        private void Awake()
        {
            abilityData = Instantiate(data);
            abilityData.CopyActionAsset();
            abilityData.identifier = GetInstanceID();
        }

        private T GetAbility<T>() where T : Ability
        {
            string type = typeof(T).ToString();
            if (availableAbilities.ContainsKey(type))
            {
                return (T)availableAbilities[type];
            }
            return default;
        }

        private void OnDestroy()
        {
            foreach (Ability ac in abilityData.availableAbilities.Values)
            {
                ac.HaltActions();
                ac.OnTermination();
            }
            foreach (AbilityStat ac in abilityData.stats.Values)
            {
                ac.CleanUp();
            }
        }

        private void Start()
        {
            if (abilityData == null)
            {
                Debug.LogWarning("Actionable Data is not set!");
                return;
            }
            int id = GetInstanceID();
            if (abilityData.identifier != id)
            {
                abilityData = Instantiate(abilityData);
                abilityData.identifier = id;
                abilityData.CopyActionAsset();
            }
            components = abilityData.stats;
            abilityData.Initialize(this);
            availableAbilities = abilityData.availableAbilities;
            TryGetComponent<Animator>(out animator);
        }
        private void Update()
        {
            UpdateComponents();
            List<Ability> removed = new();
            foreach (Ability ac in executing)
            {
                if (ActionBlocked)
                {
                    ac.HaltActions();
                }
                if (ac.RunningCount() == 0)
                {
                    removed.Add(ac);
                }
            }
            foreach (Ability ac in removed)
            {
                executing.Remove(ac);
            }
        }
        private void UpdateComponents()
        {
            foreach (AbilityStat c in components.Values)
            {
                c.Update();
            }
        }

        /// <summary>
        /// Used by action instances to play animation
        /// </summary>
        /// <param name="actionInstance"></param>
        /// <param name="config"></param>
        /// <param name="animation"></param>
        public void RegisterAnimation(string actionInstance, Ability.AbilityConfig config, string animation)
        {
            animatingConfig = (actionInstance, config);
            animator.SetBool("isActing", true);
            animator.Play(animation);
        }

        /// <summary>
        /// Used by ActionInstances to signal that the action/animation has finished. 
        /// </summary>

        public void UnregisterAnimation()
        {
            animatingConfig = ("", null);
            animator.SetBool("isActing", false);
        }

        /// <summary>
        /// Handler for the animation event, this method should not be called by anything other than editing animation event inside editor. 
        /// </summary>
        public void AnimationSignal()
        {
            (string ac, Ability.AbilityConfig config) = animatingConfig;
            availableAbilities[ac].AnimationSignal(config);
        }
    }

}

