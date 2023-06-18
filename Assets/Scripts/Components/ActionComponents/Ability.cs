using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;
using System.Linq;
using LobsterFramework.Utility;

namespace LobsterFramework.AbilitySystem {
    /// <summary>
    /// struct used by ActionOverseer to process action queue
    /// </summary>
    internal struct AbilityConfigPair
    {
        public string config;
        public Ability ability;
        public AbilityConfigPair(Ability ability, string config)
        {
            this.ability = ability;
            this.config = config;
        }

        public bool HaltAbility() { 
            return ability.HaltAbilityExecution(config);
        }
    }

    /// <summary>
    /// An instance of Action that defines the kind of action the parent object can take. <br/>
    /// Each subclass of Ability defines its own ActionConfig and can be runned on multiple instances of its AbilityConfigs.
    /// </summary>
    public abstract class Ability : ScriptableObject
    {
        /* 
         */
        [HideInInspector]
        public AbilityRunner abilityRunner;
        public RefAbilityPriority abilityPriority;

        [HideInInspector]
        [SerializeField] internal StringAbilityConfigDictionary configs = new();

        private HashSet<string> executing = new();

        /// <summary>
        /// Add the config with specified name to this Ability, this should only be called by editor scripts
        /// </summary>
        /// <param name="name">The name of the config to be added</param>
        public bool AddConfig(string name)
        {
            if (HasConfigDefined())
            {
                if (configs.ContainsKey(name))
                {
                    Debug.LogError("The ability config with name '" + name + "' is already added!");
                    return false;
                }
                Type type = GetType();
                string typeString = type.ToString();
                var m = (typeof(Ability)).GetMethod("AddConfigGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
                Type configType = Type.GetType(typeString + "+" + type.Name + "Config");
                MethodInfo method = m.MakeGenericMethod(configType);
                method.Invoke(this, new[] { name });
                return true;
            }
            Debug.LogError("The ability config for '" + GetType().Name + "' is not declared or made public!");
            return false;
        }

        private void AddConfigGeneric<T>(string name) where T : AbilityConfig
        {
            Type type = GetType();
            string typeString = type.ToString();
            if (typeof(T).ToString() != (typeString + "+" + type.Name + "Config"))
            {
                Debug.LogError("Config of type" + typeof(T).ToString() + " cannot be added!");
                return;
            }
            T config = CreateInstance<T>();
            configs.Add(name, config);
            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.AddObjectToAsset(config, this);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Remove the config with specified name if present, this should only be called by editor scripts
        /// </summary>
        /// <param name="name">Name of the config to be removed</param>
        /// <returns>The status of this operation</returns>
        public bool RemoveConfig(string name)
        {
            if (!configs.ContainsKey(name))
            {
                return false;
            }
            AbilityConfig config = configs[name];
            configs.Remove(name);
            DestroyImmediate(config, true);
            AssetDatabase.SaveAssets();
            return true;
        }

        /// <summary>
        /// Callback when the gameobject is about to be disabled or destroyed
        /// </summary>
        public virtual void CleanUp() { }

        // Comparer used to sort action by their priority
        public int CompareByExecutionPriority(Ability other)
        {
            return abilityPriority.Value.executionPriority - other.abilityPriority.Value.executionPriority;
        }

        public int CompareByEnqueuePriority(Ability other)
        {
            return abilityPriority.Value.enqueuePriority - other.abilityPriority.Value.enqueuePriority;
        }

        /// <summary>
        /// Check if the action component and the gameobject satisfies the requirements of the ActionInstance defined by its attributes.
        /// </summary>
        /// <param name="abilityRunner">The actionComponent of the gameobject to be inspected</param>
        /// <returns>Whether the gameobject satisfied the component requirements</returns>
        internal bool ComponentCheck(AbilityRunner abilityRunner)
        {
            Type type = GetType();
            bool f2 = true;
            // Check of gameobject Components
            if (RequireCMPAttribute.requirement.ContainsKey(type))
            {
                HashSet<Type> ts2 = RequireCMPAttribute.requirement[type];

                List<Type> lst2 = new List<Type>();
                foreach (Type t in ts2)
                {
                    if (abilityRunner.GetComponent(t) == null)
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
                    Debug.LogError(sb.ToString(), abilityRunner);
                }
            }
            return f2;
        }

       /// <summary>
       /// Additionaly utility method for skill check that can be imeplemented if the action have additional requirements, this may varies beween different configs
       /// </summary>
       /// <param name="config">The config being queried</param>
       /// <returns></returns>
        protected virtual bool ConditionSatisfied(AbilityConfig config) { return true; }

        /// <summary>
        /// Enqueue the action if its not on cooldown and conditions are satisfied.  <br/>
        /// This method should not be directly called by external modules such as play input or AI. <br/> 
        /// Actionable.EnqueueAction&lt;T&gt;(string configName) shoud be used instead.
        /// </summary>
        /// <param name="config">Name of the config being enqueued</param>
        /// <returns>The result of this operation</returns>
        internal bool EnqueueAbility(string config)
        {
            if (IsReady(config))
            {
                configs[config].accessKey = ActionOverseer.EnqueueAction(new AbilityConfigPair(this, config));
                executing.Add(config);
                AbilityConfig c = configs[config];
                OnEnqueue(c);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Execute the config with the provided name. Assumes the action is already in action queue and is only being called by ActionOverseer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool Execute(string name)
        {
            try
            {
                AbilityConfig config = configs[name];
                bool result = Action(config);
                if (!result || config.isSuspended)
                {
                    config.accessKey = -1;
                    config.isSuspended = false;
                    executing.Remove(name);
                    config.endTime = Time.time;
                    abilityRunner.FinishAnimation();
                    OnActionFinish(config);
                    return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError("Action '" + GetType().ToString() + "' failed to execute with config + '" + name + "':\n " + ex.ToString());
            }
            return false;
        }

        // Get the number of configs currently running
        public int RunningCount() { return executing.Count; }

        /// <summary>
        /// // Suspend the execution of provided action and force it to finish at the current frame
        /// </summary>
        /// <param name="name"> Name of the configuration to terminate </param>
        /// <returns> The status of this operation </returns>
        public bool HaltAbilityExecution(string name)
        {
            if (!configs.ContainsKey(name))
            {
                return false;
            }
            AbilityConfig config = configs[name];
            if (config.accessKey == -1)
            {
                return false;
            }
            ActionOverseer.RemoveAction(config.accessKey);
            config.accessKey = -1;
            config.isSuspended = false;
            executing.Remove(name);
            config.endTime = Time.time;
            OnActionFinish(config);
            return true;
        }

        public void HaltActions()
        {
            foreach (string name in configs.Keys)
            {
                HaltAbilityExecution(name);
            }
        }

        // Callback when the gameobject is being initialized during Start(). Do not initialize action configs in this method! Override This!
        protected virtual void Initialize() { }

        /// <summary>
        /// Check if the provided config is executing, this method will return false if the config is not present
        /// </summary>
        /// <param name="name"> The name of the config to be examined </param>
        /// <returns> True if the specified config is executing, otherwise false </returns>
        public bool IsExecuting(string name)
        {
            return executing.Contains(name);
        }

        // Check if the provided config is ready for execution (to be added to the action queue).
        public bool IsReady(string name)
        {
            if (!configs.ContainsKey(name))
            {
                Debug.LogError(GetType().ToString() + " does not have config with name '" + name + "'", this);
                return false;
            }
            AbilityConfig config = configs[name];
            return !config.IsExecuting && (!config.UseCooldown || !config.OnCooldown) && ConditionSatisfied(config);
        }

        // Called when the parent component is being initialized
        public void OnStartUp()
        {
            if (!HasConfigDefined())
            {
                Debug.LogError("The action config for '" + GetType().ToString() + "' is not declared!");
            }
            Initialize();
            foreach (AbilityConfig config in configs.Values)
            {
                config.Initialize();
            }
        }

        // Called when the parent object is destroyed, this 
        public void OnTermination()
        {
            CleanUp();
            foreach (AbilityConfig config in configs.Values)
            {
                config.Close();
            }
        }

        // Reset, called when the parent component is resetted
        public void ResetStatus()
        {
            CleanUp();
            Initialize();
        }

        public void OnDestroy()
        {

            foreach (string name in configs.Keys.ToList())
            {
                RemoveConfig(name);
            }
        }

        public void SaveConfigsAsAsset()
        {
            foreach (AbilityConfig config in configs.Values)
            {
                AssetDatabase.AddObjectToAsset(config, this);
            }
        }
        /// <summary>
        /// Main body of the ability execution, implement this to create different abilities!
        /// </summary>
        /// <param name="config">The config being executed with</param>
        /// <returns>False if the ability has finished, otherwise true</returns>
        protected abstract bool Action(AbilityConfig config);

        /// <summary>
        /// Callback when the action is finished or halted, override this to clean up temporary data generated during the action.
        /// </summary>
        /// <param name="config">The config being processed</param>
        protected virtual void OnActionFinish(AbilityConfig config) { }

        /// <summary>
        /// Callback when the action is queued, override this to perform initializations of action execution
        /// </summary>
        /// <param name="config">The config being processed</param>
        protected virtual void OnEnqueue(AbilityConfig config) { }

        private bool HasConfigDefined()
        {
            Type type = GetType();
            string typeName = type.Name + "Config";
            foreach (Type innerType in type.GetNestedTypes())
            {
                string inner = innerType.Name;
                if (inner.Equals(typeName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasConfig(string configName) {
            return configs.ContainsKey(configName);
        }

        public void Signal(string configName) {
            if (configs.ContainsKey(configName)) {
                SignalBody(configs[configName]);
            }
        }

        /// <summary>
        /// Override this to implement signal event handler
        /// </summary>
        /// <param name="config">Config to be signaled</param>
        protected virtual void SignalBody(AbilityConfig config) { }

        /// <summary>
        ///  A configuration of the Ability, each configuration has its own settings that affects the execution of the Ability.
        ///  This class should be subclassed inside subclasses of Ability with name 'Ability_Subclass_Name'Config.
        ///  i.e CircleAttack which inherit from Ability must have a nested class named CircleAttackConfig inherited from this class
        /// </summary>
        [Serializable]
        public class AbilityConfig : ScriptableObject
        {
            
            [HideInInspector]
            public int accessKey = -1;
            [HideInInspector]
            public float endTime = 0;

            [SerializeField] private bool useCooldown = true;
            [SerializeField] private float cooldown = 0;
            public bool IsExecuting { get { return accessKey != -1; } }
            public bool OnCooldown { get { return Time.time - endTime < cooldown;}}
            public bool UseCooldown { get { return useCooldown; } }

            /// <summary>
            /// Whether this ability has been halted, set this to true while the ability is running will terminate it
            /// </summary>
            public bool isSuspended = false;

            /// <summary>
            /// Override this to perform any additional setups on destruction of parent object (i.e Remove links to unity action/events)
            /// </summary>
            public virtual void Close() { }

            /// <summary>
            /// Override this to initialize action config on start up
            /// </summary>
            public virtual void Initialize() { }

            /// <summary>
            /// Override this to validate data after making changes in inspector
            /// </summary>
            protected virtual void Validate() { }

            
            protected void OnValidate()
            {
                if (cooldown < 0) {
                    Debug.LogWarning("Cooldown cannot be less than 0!", this);
                    cooldown = 0;
                }
                Validate();
            }
        }
    }
}

