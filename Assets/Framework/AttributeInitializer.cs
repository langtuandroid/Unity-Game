using System;
using UnityEngine;
using System.Reflection;
using LobsterFramework.AbilitySystem;
using LobsterFramework.Interaction;

namespace LobsterFramework.Utility
{
    /// <summary>
    /// Initializes all of the custom attributes of LobsterFramework for all assemblies
    /// </summary>
    public class AttributeInitializer
    {
        public static void Initialize(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            AddAbilityMenu(types);
            AddAbilityStatMenu(types);
            AddComponentRequirement(types); 
            AddAbilityStatRequirement(types);
            AddInteractions(types);
            AddWeaponStatMenu(types);
            AddWeaponArtMenu(types);
            AddWeaponStatRequirement(types);
        }

        private static void AddAbilityMenu(Type[] types)
        {
            foreach (Type type in types)
            {
                AddAbilityMenuAttribute info = type.GetCustomAttribute<AddAbilityMenuAttribute>();
                if (info != null) {
                    info.AddAbility(type);
                }
            }
        }

        private static void AddAbilityStatMenu(Type[] types) {
            foreach (Type type in types)
            {
                AddAbilityStatMenuAttribute info = type.GetCustomAttribute<AddAbilityStatMenuAttribute>();
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }

        private static void AddComponentRequirement(Type[] types) {
            foreach (Type type in types)
            {
                ComponentRequiredAttribute info = type.GetCustomAttribute<ComponentRequiredAttribute>(true);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }
        private static void AddAbilityStatRequirement(Type[] types) {
            foreach (Type type in types)
            {
                foreach (RequireAbilityStatsAttribute info in type.GetCustomAttributes<RequireAbilityStatsAttribute>(true)) {
                    info.Init(type);
                }
            }
        }

        private static void AddWeaponStatMenu(Type[] types) {
            foreach (Type type in types)
            {
                foreach (AddWeaponStatMenuAttribute info in type.GetCustomAttributes<AddWeaponStatMenuAttribute>())
                {
                    info.Init(type);
                }
            }
        }

        private static void AddWeaponArtMenu(Type[] types) {
            foreach (Type type in types)
            {
                foreach (AddWeaponArtMenuAttribute info in type.GetCustomAttributes<AddWeaponArtMenuAttribute>())
                {
                    info.Init(type);
                }
            }
        }

        private static void AddWeaponStatRequirement(Type[] types) {
            foreach (Type type in types)
            {
                foreach (RequireWeaponStatAttribute info in type.GetCustomAttributes<RequireWeaponStatAttribute>())
                {
                    info.Init(type);
                }
            }
        }

        private static void AddInteractions(Type[] types) {
            foreach (Type type in types)
            {
                InteractionAttribute info = type.GetCustomAttribute<InteractionAttribute>();
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }
    }
}
