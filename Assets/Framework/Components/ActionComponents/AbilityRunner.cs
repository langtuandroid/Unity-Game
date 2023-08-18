using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using LobsterFramework.Utility;
using LobsterFramework.Utility.BufferedStats;

namespace LobsterFramework.AbilitySystem {
    /// <summary>
    /// Component that provides access to a variety of movesets, it acts as a container and platform for 
    /// different kinds of abilitiess and moves which are provided and fully implemented by associated sub-components.
    /// Requires actionable data to be supplied. 
    /// </summary>
    [AddComponentMenu("AbilityRunner")]
    public class AbilityRunner : MonoBehaviour
    {
        // Callbacks
        public UnityAction<bool> onActionBlocked;
        public UnityAction<bool> onHyperArmored;

        // Execution Info
        internal HashSet<AbilityConfigPair> executing = new();
        private Dictionary<AbilityConfigPair, AbilityConfigPair> jointlyRunning = new();

        // Data
        [HideInInspector] 
        [SerializeField] private AbilityData abilityData;
        [SerializeField] private AbilityData data;
        private Dictionary<string, Ability> availableAbilities;
        private TypeAbilityStatDictionary stats;

        // Animation
        [SerializeField] private Animator animator;
        [SerializeField] private WeaponWielder weaponWielder;
        [SerializeField] private MovementController movementController;
        [SerializeField] private Transform topLevelTransform;
        private (Ability, string) animating;

        // Status
        private BaseOr actionBlocked = new(false);
        private BaseOr hyperArmored = new(false);
        private bool initialized = false;
        
        public bool ActionBlocked {
            get { return actionBlocked.Stat && !HyperArmored; }
        }

        public bool HyperArmored { 
            get { return hyperArmored.Stat; }
        }

        public Transform TopLevelTransform { get { return topLevelTransform; } }    

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
                executing.Add(new AbilityConfigPair(action, name));
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
        /// Enqueue two abilities of different types together with the second one being guaranteed to <br/>
        /// terminate no later than the first one.
        /// </summary>
        /// <typeparam name="T"> The type of the first ability </typeparam>
        /// <typeparam name="V"> The type of the second ability </typeparam>
        /// <param name="config1"> Configuration for the first ability </param>
        /// <param name="config2"> Configuration for the second ability </param>
        /// <returns></returns>
        public bool EnqueueAbilitiesInJoint<T, V>(string config1, string config2) 
            where T : Ability 
            where V : Ability
        {
            T a1 = GetAbility<T>();
            V a2 = GetAbility<V>();

            // The two abilities must both be present and not the same
            if (a1 == default || a2 == default || a1 == a2) {
                return false; 
            }

            if (a1.IsReady(config1) && a2.IsReady(config2)) {
                EnqueueAbility<T>(config1);
                EnqueueAbility<V>(config2);
                AbilityConfigPair p1 = new AbilityConfigPair(a1, config1);
                AbilityConfigPair p2 = new AbilityConfigPair(a2, config2);
                jointlyRunning[p1] = p2;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shorthand for EnqueueAbilitiesInJoint&lt;T, V&gt;("default", "default")
        /// </summary>
        /// <typeparam name="T"> The type of the first ability </typeparam>
        /// <typeparam name="V"> The type of the second ability </typeparam>
        /// <returns></returns>
        public bool EnqueueAbilitiesInJoint<T, V>()
           where T : Ability
           where V : Ability
        {
            return EnqueueAbilitiesInJoint<T, V>("default", "default");
        }

        /// <summary>
        /// Get the ability stat of type T attached to this Component if it is present.
        /// </summary>
        /// <typeparam name="T">Type of the AbilityStat being requested</typeparam>
        /// <returns>Return the ability stat if it is present, otherwise null</returns>
        public T GetAbilityStat<T>() where T : AbilityStat
        {
            string type = typeof(T).ToString();
            if (stats.ContainsKey(type))
            {
                return (T)stats[type];
            }
            return default(T); 
        }

        /// <summary>
        /// Get the ability pipe of specified ability and configuration
        /// </summary>
        /// <typeparam name="T">The type of Ability to pipe to.</typeparam>
        /// <param name="configName">The name of the ability configuration to pipe to</param>
        /// <returns> The pipe that connects to the specified ability and configuration if it exists, otherwise return null. </returns>
        public Ability.AbilityPipe GetAbilityPipe<T>(string configName="default") where T : Ability {
            T ability = GetAbility<T>();
            if (ability != null) {
                return ability.GetAbilityPipe(configName);
            }
            return default;
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
            AbilityConfigPair pair = new(ac, name);
            if (executing.Contains(pair))
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
        /// <typeparam name="T">Type of the Ability to be queried</typeparam>
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
        /// A shortcut for IsInstanceReady&lt;T&gt;("default").
        /// </summary>
        /// <typeparam name="T">Type of the Ability to be queried</typeparam>
        /// <returns> The result of the query </returns>
        public bool IsAbilityReady<T>() where T : Ability
        {
            return IsAbilityReady<T>("default");
        }

        /// <summary>
        /// Add an effector to block actions, actions will be blocked if there's at least 1 effector.
        /// An event will be sent to all subscribers on action blocked.
        /// </summary>
        /// <returns>The id of the newly added effector</returns>
        public int BlockAction()
        {
            bool before = ActionBlocked;
            int id = actionBlocked.AddEffector(true);
            if (onActionBlocked != null && before != ActionBlocked)
            {
                onActionBlocked.Invoke(true);
            }
            return id;
        }

        /// <summary>
        /// Remvoe the specified effector that blocks the action if it exists.
        /// An event will be sent to subscribers on action unblocked.
        /// </summary>
        /// <param name="id">The id of the effector to be removed</param>
        /// <returns>On successfully removal return true, otherwise false</returns>
        public bool UnblockAction(int id) {
            bool before = ActionBlocked;
            if (actionBlocked.RemoveEffector(id)) {
                if (ActionBlocked != before && onActionBlocked != null) {
                    onActionBlocked.Invoke(false);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add an effector to provide hyperarmor for the object. While hyperarmored, abilities cannot be interrupted other than death.
        /// Hyperarmor will exist if there's at least 1 effector.
        /// An event will be sent to subscribers on hyperarmored.
        /// </summary>
        /// <returns>The id of the newly adde effector</returns>
        public int HyperArmor() {
            bool before = hyperArmored.Stat;
            int id = hyperArmored.AddEffector(true);
            if (onHyperArmored != null && before != hyperArmored.Stat)
            {
                onHyperArmored.Invoke(true);
            }
            return id;
        }

        /// <summary>
        /// Remove the specified effector for hyperarmor if it exists
        /// An event will be sent to subscribers on dishyperarmored
        /// </summary>
        /// <param name="id">The id of the effector to be removed</param>
        /// <returns>On successfully removal return true, otherwise false</returns>
        public bool DisArmor(int id) {
            if (hyperArmored.RemoveEffector(id))
            {
                if (!hyperArmored.Stat && onHyperArmored != null)
                {
                    onHyperArmored.Invoke(false);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reset the status of all abilities and their configs to their initial state
        /// </summary>

        public void Reset()
        {
            if (stats == null || availableAbilities == null) {
                return;
            }
            foreach (AbilityStat component in stats.Values)
            {
                component.Reset();
            }
            foreach (Ability ac in availableAbilities.Values)
            {
                ac.ResetStatus();
            }
        }

        /// <summary>
        /// Only to be called inside play mode! Save the current ability data as an asset with specified assetName to the default path.
        /// </summary>
        /// <param name="assetName">Name of the asset to be saved</param>
        public void SaveAbilityData(string assetName)
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
         
        private T GetAbility<T>() where T : Ability
        {
            string type = typeof(T).ToString();
            if (availableAbilities.ContainsKey(type))
            {
                return (T)availableAbilities[type];
            }
            return default;
        }

        private void OnDisable()
        {
            foreach (AbilityConfigPair pair in executing) {
                pair.HaltAbility();
            }
            abilityData.Terminate();
        }

        private void OnEnable()
        {
            if (!initialized) { return; }
            abilityData.Initialize(this);
        }

        /// <summary>
        /// The Start Method of AbilityRunner needs to be runned at high priority as it needs to duplicate the ability data asset for other script refer to in their Start() method
        /// </summary>
        private void Start()
        {
            if (data == null)
            {
                Debug.LogWarning("Ability Data is not set!", gameObject);
                return;
            }
            abilityData = Instantiate(data); 
            abilityData.CopyActionAsset();
            stats = abilityData.stats;
            abilityData.Initialize(this);
            availableAbilities = abilityData.availableAbilities;
        }
        private void Update()
        {
            stats.Values.ToList().ForEach(value => value.Update());
            List<AbilityConfigPair> removed = new();
            foreach (AbilityConfigPair ap in executing)
            {
                Ability ac = ap.ability;
                if (ActionBlocked)
                {
                    ac.HaltAbilities();
                }
                if (!ac.IsExecuting(ap.config))
                {
                    removed.Add(ap);
                    if (jointlyRunning.ContainsKey(ap)) { 
                        jointlyRunning[ap].HaltAbility();
                        jointlyRunning.Remove(ap);
                    }
                }
            }
            foreach (AbilityConfigPair ap in removed)
            {
                executing.Remove(ap);
                if (IsAnimating(ap.ability, ap.config)) { FinishAnimation(); }
            }
        }


        /// <summary>
        /// Check if the specified ability config pair is playing animation. 
        /// </summary>
        /// <param name="ability">The ability to be queried</param>
        /// <param name="configName">The config of the ability to be queried</param>
        /// <returns>Return true if the ability and the config are both valid and present and are playing animation, otherwise false</returns>
        private bool IsAnimating(Ability ability, string configName) { 
            return animating == (ability, configName);
        }

        public bool IsAnimating()
        {
            return animating != default;
        }

        public Animator Animator { get { return animator; } }
        public MovementController MovementController { get {  return movementController; } }

        /// <summary>
        /// Used by abilities to initiate animations, will override any currently running animations by other abilities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <param name="animation"></param>
        public void StartAnimation<T>(string configName, string animation, float speed=1) where T : Ability
        {
            Ability ability = GetAbility<T>();
            if (ability == null) {
                return;
            }
            if (!ability.HasConfig(configName)) {
                return;
            }
            if (animating != default) {
                animating.Item1.InterruptAnimation(animating.Item2);
            }
            animator.speed = speed;
            animating = (ability, configName);
            animator.CrossFade(animation, 0.1f, -1, 0.1f);
        }

        /// <summary>
        /// End ability animations
        /// </summary>

        private void FinishAnimation()
        {
            if(animator == null) { return; }
            if(animating == default) { return; }
            animating = default;
            if(weaponWielder != null)
            {
                weaponWielder.PlayDefaultWeaponAnimation();
            }
        }

        /// <summary>
        /// Send a signal to the specified ability. 
        /// </summary>
        public void Signal<T>(string configName="default") where T : Ability
        {
            Ability ability = GetAbility<T>();
            if (ability == null) {
                return;
            }
            ability.Signal(configName, false);
        }

        /// <summary>
        /// Used by animation events to send signals
        /// </summary>
        public void AnimationSignal() { 
            if(animating == default) { return; }
            animating.Item1.Signal(animating.Item2, true);
        }

        public void AnimationEnd() {
            if (animating == default) { return; }
            animating.Item1.HaltAbilityExecution(animating.Item2);
        }
    }

}

