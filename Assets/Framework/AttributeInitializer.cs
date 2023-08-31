using System;
using UnityEngine;
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
       
        public static void Initialize(Assembly assembly)
        {
            AddAbilityMenu(assembly);
            AddAbilityStatMenu(assembly);
            AddComponentRequirement(assembly); 
            AddAbilityStatRequirement(assembly);
            AddInteractions(assembly);
            AddWeaponStatMenu(assembly);
            AddWeaponArtMenu(assembly);
            AddWeaponStatRequirement(assembly);
        }

        private static void AddAbilityMenu(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                AddAbilityMenuAttribute info = type.GetCustomAttribute<AddAbilityMenuAttribute>();
                if (info != null) {
                    info.AddAbility(type);
                }
            }
        }

        private static void AddAbilityStatMenu(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                AddAbilityStatMenuAttribute info = type.GetCustomAttribute<AddAbilityStatMenuAttribute>();
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }

        private static void AddComponentRequirement(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                ComponentRequiredAttribute info = type.GetCustomAttribute<ComponentRequiredAttribute>(true);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }
        private static void AddAbilityStatRequirement(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (RequireAbilityStatsAttribute info in type.GetCustomAttributes<RequireAbilityStatsAttribute>(true)) {
                    info.Init(type);
                }
            }
        }

        private static void AddWeaponStatMenu(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (AddWeaponStatMenuAttribute info in type.GetCustomAttributes<AddWeaponStatMenuAttribute>())
                {
                    info.Init(type);
                }
            }
        }

        private static void AddWeaponArtMenu(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (AddWeaponArtMenuAttribute info in type.GetCustomAttributes<AddWeaponArtMenuAttribute>())
                {
                    info.Init(type);
                }
            }
        }

        private static void AddWeaponStatRequirement(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (RequireWeaponStatAttribute info in type.GetCustomAttributes<RequireWeaponStatAttribute>())
                {
                    info.Init(type);
                }
            }
        }

        private static void AddInteractions(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
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
