using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
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
    /// Abilities defines the kind of actions the parent object can make. <br/>
    /// Each subclass of Ability defines its own AbilityConfig and can be runned on multiple instances of its AbilityConfigs.
    /// </summary>
    public abstract class Ability : ScriptableObject
    {
        [HideInInspector]
        public AbilityRunner abilityRunner;

        public RefAbilityPriority abilityPriority;

        [HideInInspector]
        [SerializeField] internal StringAbilityConfigDictionary configs = new();
        [HideInInspector]
        [SerializeField] internal StringAbilityPipeDictionary pipes = new();

        private HashSet<string> executing = new();

        /// <summary>
        /// Add the config with specified name to this Ability, this should only be called by editor scripts
        /// </summary>
        /// <param name="name">The name of the config to be added</param>
        internal bool AddConfig(string name)
        {
            if (HasConfigPipeDefined())
            {
                if (configs.ContainsKey(name))
                {
                    Debug.LogError("The ability config with name '" + name + "' is already added!");
                    return false;
                }
                Type type = GetType();
                var m = (typeof(Ability)).GetMethod("AddConfigGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
                Type configType = type.GetNestedType(type.Name + "Config");
                Type pipeType = type.GetNestedType(type.Name + "Pipe");
                MethodInfo method = m.MakeGenericMethod(configType, pipeType);
                method.Invoke(this, new[] { name });
                return true;
            }
            Type t = GetBaseConfigType();
            Type t2 = GetBasePipeType();
            Debug.LogError("The ability config or pipe for '" + GetType().Name + "' is not declared, inheriting from " + t.Name + " / " + t2.Name + " or made public!");
            return false;
        }

        private void AddConfigGeneric<T, V>(string name) where T : AbilityConfig where V : AbilityPipe
        {
            Type type = GetType();
            if (typeof(T).Name != (type.Name + "Config"))
            {
                Debug.LogError("Config of type" + typeof(T).ToString() + " cannot be added!");
                return;
            }
            T config = CreateInstance<T>();
            configs.Add(name, config);
            AbilityPipe pipe = CreateInstance<V>();
            config.name = this.name + "-" + name;
            pipe.name = this.name + "=" + name;
            pipe.Construct(config);
            pipes.Add(name, pipe);
            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.AddObjectToAsset(config, this);
                AssetDatabase.AddObjectToAsset(pipe, this);
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
            AbilityPipe pipe = pipes[name];
            configs.Remove(name);
            pipes.Remove(name);
            DestroyImmediate(pipe, true);
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
       /// Additionaly utility method for skill check that can be imeplemented if the ability have additional requirements, this may varies beween different configs
       /// </summary>
       /// <param name="config">The config being queried</param>
       /// <returns></returns>
        protected virtual bool ConditionSatisfied(AbilityConfig config) { return true; }

        /// <summary>
        /// Enqueue the ability if its not on cooldown and conditions are satisfied.  <br/>
        /// This method should not be directly called by external modules such as play input or AI. <br/> 
        /// AbilityRunner.EnqueueAbility&lt;T&gt;(string configName) shoud be used instead.
        /// </summary>
        /// <param name="configName">Name of the config being enqueued</param>
        /// <returns>The result of this operation</returns>
        internal bool EnqueueAbility(string configName)
        {
            if (IsReady(configName))
            {
                configs[configName].accessKey = ActionOverseer.EnqueueAction(new AbilityConfigPair(this, configName));
                executing.Add(configName);
                AbilityConfig config = configs[configName];
                AbilityPipe pipe = pipes[configName];
                OnEnqueue(config, pipe, configName);
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
                AbilityPipe pipe = pipes[name];
                bool result = Action(config, pipe);
                if (!result || config.isSuspended)
                {
                    config.accessKey = -1;
                    config.isSuspended = false;
                    executing.Remove(name);
                    config.endTime = Time.time;
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
        /// Suspend the execution of provided action and force it to finish at the current frame
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

        /// <summary>
        /// Halt the execution of all configs
        /// </summary>
        public void HaltAbilities()
        {
            foreach (string name in configs.Keys)
            {
                HaltAbilityExecution(name);
            }
        }

        /// <summary>
        /// Callback when the gameobject is being initialized during Start()
        /// </summary>
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
            if (!HasConfigPipeDefined())
            {
                Debug.LogError("The ability config or pipe for '" + GetType().ToString() + "' is not declared!");
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

        internal void OnDestroy()
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
        protected abstract bool Action(AbilityConfig config, AbilityPipe pipe);

        /// <summary>
        /// Callback when the action is finished or halted, override this to clean up temporary data generated during the action.
        /// </summary>
        /// <param name="config">The config being processed</param>
        protected virtual void OnActionFinish(AbilityConfig config) { }

        /// <summary>
        /// Callback when the animation of the ability is interrupted by other abilities. Useful when abilities relies on animation events.
        /// </summary>
        /// <param name="config"></param>
        protected virtual void OnAnimationInterrupt(AbilityConfig config) { }

        /// <summary>
        /// Interrupt the animation of the currently animating AbilityConfig pair
        /// </summary>
        /// <param name="configName"></param>
        internal void InterruptAnimation(string configName) {
            if (!configs.ContainsKey(configName)) { return; }
            AbilityConfig config = configs[configName];
            OnAnimationInterrupt(config);
        }
        /// <summary>
        /// Callback when the ability is added to the action executing queue
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configName"></param>
        protected virtual void OnEnqueue(AbilityConfig config, AbilityPipe pipe, string configName) { }

        private Type GetBaseConfigType() {
            Type type = GetType().BaseType;
            while (type != typeof(Ability)) {
                Type t = type.GetNestedType(type.Name + "Config");
                if (t != null && t.IsSubclassOf(typeof(AbilityConfig))) { 
                    return t;
                }
                type = type.BaseType;
            }
            return typeof(AbilityConfig);
        }

        private Type GetBasePipeType()
        {
            Type type = GetType().BaseType;
            while (type != typeof(Ability))
            {
                Type t = type.GetNestedType(type.Name + "Pipe");
                if (t != null && t.IsSubclassOf(typeof(AbilityPipe)))
                {
                    return t;
                }
                type = type.BaseType;
            }
            return typeof(AbilityPipe);
        }

        private bool HasConfigPipeDefined()
        {
            Type type = GetType();
            string typeName = type.Name + "Config";
            string pipeName = type.Name + "Pipe";
            Type configType = GetBaseConfigType();
            Type pipeType = GetBasePipeType();
            bool config = false;
            bool pipe = false;

            foreach (Type innerType in type.GetNestedTypes())
            {
                string inner = innerType.Name;
                if (inner.Equals(typeName) && innerType.IsSubclassOf(configType))
                {
                    config = true;
                } else if (inner.Equals(pipeName) && innerType.IsSubclassOf(pipeType)) {
                    pipe = true;
                }
                if (config && pipe) {
                    return true;
                }
            }
            return false;
        }

        public bool HasConfig(string configName) {
            return configs.ContainsKey(configName);
        }

        public void Signal(string configName, bool isAnimation) {
            if (configs.ContainsKey(configName)) {
                Signal(configs[configName], isAnimation);
            }
        }

        /// <summary>
        /// Override this to implement signal event handler
        /// </summary>
        /// <param name="config">Config to be signaled</param>
        protected virtual void Signal(AbilityConfig config, bool isAnimation) { }

        public AbilityPipe GetAbilityPipe(string configName)
        {
            if (configs.ContainsKey(configName)) {
                return pipes[configName];
            }
            return default;
        }

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
            protected internal virtual void Close() { }

            /// <summary>
            /// Override this to initialize action config on start up
            /// </summary>
            protected internal virtual void Initialize() { }

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

        [Serializable]
        public class AbilityPipe : ScriptableObject {
            protected AbilityConfig config;
            public void Construct(AbilityConfig config) { this.config = config; Construct(); }
            public virtual void Construct() { }
        }
    }
}

