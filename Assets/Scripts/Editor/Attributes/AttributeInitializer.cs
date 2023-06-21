using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Interaction;

namespace LobsterFramework.Utility
{
    /// <summary>
    /// Initializes all of the custom attributes of namespace LobsterFramework
    /// </summary>
    public class AttributeInitializer
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void Initialize()
        {
            AddAbilityMenu();
            AddComponentRequirement();
            AddAbilityStatRequirement();
            AddInteractions();
        }

        private static void AddAbilityMenu()
        {
            foreach (Type type in typeof(GameManager).Assembly.GetTypes())
            {
                AddAbilityMenuAttribute info = type.GetCustomAttribute<AddAbilityMenuAttribute>(false);
                if (info != null) {
                    info.AddAbility(type);
                }
            }
        }

        private static void AddComponentRequirement() {
            foreach (Type type in typeof(GameManager).Assembly.GetTypes())
            {
                ComponentRequiredAttribute info = type.GetCustomAttribute<ComponentRequiredAttribute>(false);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }
        private static void AddAbilityStatRequirement() {
            foreach (Type type in typeof(GameManager).Assembly.GetTypes())
            {
                RequireAbilityStatsAttribute info = type.GetCustomAttribute<RequireAbilityStatsAttribute>(false);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }

        private static void AddInteractions() {
            foreach (Type type in typeof(GameManager).Assembly.GetTypes())
            {
                InteractionAttribute info = type.GetCustomAttribute<InteractionAttribute>(false);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }
    }
}
