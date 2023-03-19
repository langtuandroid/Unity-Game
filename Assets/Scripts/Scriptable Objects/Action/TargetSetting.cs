using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LobsterFramework.Utility.Groups;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.Action
{
    [CreateAssetMenu(menuName = "Action/TargetSetting")]
    public class TargetSetting : ScriptableObject
    {
        public EntityGroup[] targetGroups;
        public EntityGroup[] ignoreGroups;
        private HashSet<Entity> targets;
        private HashSet<Entity> ignores;

        public bool IsTarget(Entity entity) {
            return targets.Contains(entity) && !ignores.Contains(entity); 
        }

        private void OnEnable()
        {
            targets = new();
            ignores = new();
            foreach (EntityGroup group in targetGroups)
            {
                targets.UnionWith(group);
                group.OnEntityAdded.AddListener((Entity entity) => { targets.Add(entity); });
                group.OnEntityRemoved.AddListener((Entity entity) => { targets.Remove(entity); });
            }
            foreach (EntityGroup group in ignoreGroups)
            {
                ignores.UnionWith(group);
                group.OnEntityAdded.AddListener((Entity entity) => { ignores.Add(entity); });
                group.OnEntityRemoved.AddListener((Entity entity) => { ignores.Remove(entity); });
            }
        }

        private void OnDisable()
        {
            foreach (EntityGroup group in targetGroups)
            {
                group.OnEntityAdded.RemoveListener((Entity entity) => { targets.Add(entity); });
                group.OnEntityRemoved.RemoveListener((Entity entity) => { targets.Remove(entity); });
            }
            foreach (EntityGroup group in ignoreGroups)
            {
                group.OnEntityAdded.RemoveListener((Entity entity) => { ignores.Add(entity); });
                group.OnEntityRemoved.RemoveListener((Entity entity) => { ignores.Remove(entity); });
            }
            targets.Clear();
            ignores.Clear();
        }
    }
}
