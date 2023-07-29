using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;

namespace LobsterFramework.Utility.Groups
{
    [CreateAssetMenu(menuName = "Groups/Entity Group")]
    public class EntityGroup : ScriptableObject, IEnumerable<Entity>, ISet<Entity>
    {
        private HashSet<Entity> group = new();
        public UnityEvent<Entity> OnEntityAdded = new();
        public UnityEvent<Entity> OnEntityRemoved = new();

        public int Count => ((ICollection<Entity>)group).Count;

        public bool IsReadOnly => ((ICollection<Entity>)group).IsReadOnly;

        public IEnumerator<Entity> GetEnumerator()
        {
            return ((IEnumerable<Entity>)group).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)group).GetEnumerator();
        }

        public bool Add(Entity item)
        {
            if (((ISet<Entity>)group).Add(item)) { 
                OnEntityAdded.Invoke(item);
                return true;
            }
            return false;
        }

        public void ExceptWith(IEnumerable<Entity> other)
        {
            ((ISet<Entity>)group).ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<Entity> other)
        {
            ((ISet<Entity>)group).IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<Entity> other)
        {
            return ((ISet<Entity>)group).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<Entity> other)
        {
            return ((ISet<Entity>)group).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<Entity> other)
        {
            return ((ISet<Entity>)group).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<Entity> other)
        {
            return ((ISet<Entity>)group).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<Entity> other)
        {
            return ((ISet<Entity>)group).Overlaps(other);
        }

        public bool SetEquals(IEnumerable<Entity> other)
        {
            return ((ISet<Entity>)group).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<Entity> other)
        {
            ((ISet<Entity>)group).SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<Entity> other)
        {
            ((ISet<Entity>)group).UnionWith(other);
        }

        void ICollection<Entity>.Add(Entity item)
        {
            ((ICollection<Entity>)group).Add(item);
        }

        public void Clear()
        {
            foreach(Entity entity in group)
            {
                OnEntityRemoved.Invoke(entity);
            }
            ((ICollection<Entity>)group).Clear();
        }

        public bool Contains(Entity item)
        {
            return ((ICollection<Entity>)group).Contains(item);
        }

        public void CopyTo(Entity[] array, int arrayIndex)
        {
            ((ICollection<Entity>)group).CopyTo(array, arrayIndex);
        }

        public bool Remove(Entity item)
        {
            if (((ICollection<Entity>)group).Remove(item)) { 
                OnEntityRemoved.Invoke(item); return true;
            }
            return false;
        }
    }
}
