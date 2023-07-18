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
       
        public static void Initialize(Assembly assembly)
        {
            AddAbilityMenu(assembly);
            AddAbilityStatMenu(assembly);
            AddComponentRequirement(assembly); 
            AddAbilityStatRequirement(assembly);
            AddInteractions(assembly);
        }

        private static void AddAbilityMenu(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                AddAbilityMenuAttribute info = type.GetCustomAttribute<AddAbilityMenuAttribute>(false);
                if (info != null) {
                    info.AddAbility(type);
                }
            }
        }

        private static void AddAbilityStatMenu(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                AddAbilityStatMenuAttribute info = type.GetCustomAttribute<AddAbilityStatMenuAttribute>(false);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }

        private static void AddComponentRequirement(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                ComponentRequiredAttribute info = type.GetCustomAttribute<ComponentRequiredAttribute>(false);
                if (info != null)
                {
                    info.Init(type);
                }
            }
        }
        private static void AddAbilityStatRequirement(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (RequireAbilityStatsAttribute info in type.GetCustomAttributes<RequireAbilityStatsAttribute>()) {
                    info.Init(type);
                }
            }
        }

        private static void AddInteractions(Assembly assembly) {
            foreach (Type type in assembly.GetTypes())
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
