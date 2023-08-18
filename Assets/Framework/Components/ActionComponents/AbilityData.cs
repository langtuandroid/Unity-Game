using LobsterFramework.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    [CreateAssetMenu(menuName = "Ability/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        public TypeAbilityStatDictionary stats = new();
        public TypeAbilityDictionary allAbilities = new();
        public Dictionary<string, Ability> availableAbilities = new();

        /// <summary>
        /// Initialize the data containers, called by AbilityRunner on Start to initialize the Abilities.
        /// </summary>
        /// <param name="abilityRunner">The component that operates on this data</param>
        internal void Initialize(AbilityRunner abilityRunner)
        {
            availableAbilities.Clear();
            foreach (Ability ai in allAbilities.Values)
            {
                if (ComponentRequiredAttribute.ComponentCheck(ai.GetType(), abilityRunner.gameObject))
                {
                    ai.abilityRunner = abilityRunner;
                    ai.OnStartUp();
                    availableAbilities[ai.GetType().ToString()] = ai;
                }
            }

            foreach (AbilityStat cmp in stats.Values)
            {
                cmp.Initialize();
            }
        }

        internal void Terminate() {
            foreach (Ability ai in availableAbilities.Values)
            {
                ai.OnTermination();
            }

            foreach (AbilityStat cmp in stats.Values)
            {
                cmp.CleanUp();
            }
        }

        /// <summary>
        /// Save data as assets by adding them to the AssetDataBase
        /// </summary>
        public void SaveContentsAsAsset()
        {
            if (AssetDatabase.Contains(this))
            {
                foreach (AbilityStat cmp in stats.Values)
                {
                    AssetDatabase.AddObjectToAsset(cmp, this);
                }
                foreach (Ability ai in allAbilities.Values)
                {
                    AssetDatabase.AddObjectToAsset(ai, this);
                    ai.SaveConfigsAsAsset();
                }
            }
        }
        
        /// <summary>
        /// Deep copy of action datas. Call this method after duplicate the AbilityData to ensure all of its contents are properly duplicated.
        /// </summary>
        public void CopyActionAsset()
        {
            List<Ability> ais = new();
            int id = GetInstanceID();
            foreach (var kwp in allAbilities)
            {
                Ability ai = Instantiate(kwp.Value);
                ais.Add(ai);
                ai.abilityDataId = id;
            }
            foreach (var ai in ais)
            {
                allAbilities[ai.GetType().ToString()] = ai;
                List<(string, Ability.AbilityConfig)> cfs = new();
                foreach (var kwp in ai.configs)
                {

                    cfs.Add((kwp.Key, Instantiate(kwp.Value)));  
                }
                foreach ((string name, Ability.AbilityConfig config) in cfs)
                {
                    ai.configs[name] = config; 
                    config.pipe = Instantiate(config.pipe);
                } 
            }

            List<AbilityStat> acs = new();
            foreach (var kwp in stats)
            {
                AbilityStat ac = Instantiate(kwp.Value);
                acs.Add(ac);
            }
            foreach (var ac in acs)
            {
                stats[ac.GetType().ToString()] = ac;
            }
        }

        private T GetAbility<T>() where T : Ability
        {
            string type = typeof(T).ToString();
            if (allAbilities.ContainsKey(type))
            {
                return (T)allAbilities[type];
            }
            return default;
        }

        private T GetAbilityStat<T>() where T : AbilityStat
        {
            string type = typeof(T).ToString();
            if (stats.ContainsKey(type))
            {
                return (T)stats[type];
            }
            return default;
        }

        /// <summary>
        /// Check if requirements for ActionComponents by Ability T are satisfied
        /// </summary>
        /// <typeparam name="T"> Type of the Ability to be queried </typeparam>
        /// <returns>The result of the query</returns>
        internal bool AbilityStatsCheck<T>() where T : Ability
        {
            Type type = typeof(T);
            bool f1 = true;

            if (RequireAbilityStatsAttribute.requirement.ContainsKey(type))
            {
                HashSet<Type> ts = RequireAbilityStatsAttribute.requirement[type];

                List<Type> lst1 = new List<Type>();
                foreach (Type t in ts)
                {
                    if (!stats.ContainsKey(t.ToString()))
                    {
                        lst1.Add(t);
                        f1 = false;
                    }
                }
                if (f1)
                {
                    return true;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("Missing AbilityStats: ");
                foreach (Type t in lst1)
                {
                    sb.Append(t.Name);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                Debug.LogError(sb.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called by editor scritps, add ActionComponent of type T to the set of available AbilityStats if not already present, return the status of the operation. <br/>
        /// </summary>
        /// <typeparam name="T">Type of the AbilityStat to be added</typeparam>
        /// <returns>The status of the operation</returns>
        internal bool AddAbilityStat<T>() where T : AbilityStat
        {
            string str = typeof(T).ToString();
            if (stats.ContainsKey(str))
            {
                return false;
            }
            T instance = CreateInstance<T>();
            stats[str] = instance;
            if (AssetDatabase.Contains(this))
            {
                AssetDatabase.AddObjectToAsset(instance, this);
                AssetDatabase.SaveAssets();
            }
            return true;
        }

        internal bool AddAbility<T>() where T : Ability
        {
            if (GetAbility<T>() != default)
            {
                return false;
            }

            if (AbilityStatsCheck<T>())
            {
                T ai = CreateInstance<T>();
                allAbilities.Add(typeof(T).ToString(), ai);
                ai.configs = new();
                ai.name = typeof(T).ToString();
                ai.abilityDataId = GetInstanceID();
                if (AssetDatabase.Contains(this))
                {
                    AssetDatabase.AddObjectToAsset(ai, this);
                    AssetDatabase.SaveAssets();
                }
                return true;
            }
            return false;
        }

        internal bool RemoveAbility<T>() where T : Ability
        {
            Ability ai = GetAbility<T>();
            if (ai != null)
            {
                allAbilities.Remove(typeof(T).ToString());
                AssetDatabase.RemoveObjectFromAsset(ai);
                DestroyImmediate(ai, true);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }

        internal bool RemoveAbilityStat<T>() where T : AbilityStat
        {
            string str = typeof(T).ToString();
            if (stats.ContainsKey(str))
            {
                if (RequireAbilityStatsAttribute.rev_requirement.ContainsKey(typeof(T)))
                {
                    StringBuilder sb = new();
                    sb.Append("Cannot remove AbilityStat: " + typeof(T).ToString() + ", since the following abilities requires it: ");
                    bool flag = false;
                    foreach (Type t in RequireAbilityStatsAttribute.rev_requirement[typeof(T)])
                    {
                        if (allAbilities.ContainsKey(t.ToString()))
                        {
                            flag = true;
                            sb.Append(t.Name);
                            sb.Append(", ");
                        }
                    }
                    if (flag)
                    {
                        sb.Remove(sb.Length - 2, 2);
                        Debug.LogError(sb.ToString());
                        return false;
                    }
                }
            }
            T cmp = GetAbilityStat<T>();
            if (cmp != null)
            {
                stats.Remove(str);
                AssetDatabase.RemoveObjectFromAsset(cmp);
                DestroyImmediate(cmp, true);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }
    }
}

