using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Groups/Entity Group")]
public class EntityGroup : ScriptableObject, IEnumerable<Entity>, IList<Entity>
{
    [SerializeField] private List<Entity> group = new();
    public UnityEvent<Entity> OnEntityAdded = new();
    public UnityEvent<Entity> OnEntityRemoved = new();

    public int Count => ((ICollection<Entity>)group).Count;

    public bool IsReadOnly => ((ICollection<Entity>)group).IsReadOnly;

    public Entity this[int index] { get => ((IList<Entity>)group)[index]; set => ((IList<Entity>)group)[index] = value; }

    public void Add(Entity entity) {
        OnEntityAdded.Invoke(entity);
        group.Add(entity);
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        return ((IEnumerable<Entity>)group).GetEnumerator();
    }

    public bool Remove(Entity entity) {
        if (group.Remove(entity)) { 
            OnEntityRemoved.Invoke(entity);
            return true;
        }
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return group.GetEnumerator();
    }

    public int IndexOf(Entity item)
    {
        return ((IList<Entity>)group).IndexOf(item);
    }

    public void Insert(int index, Entity item)
    {
        ((IList<Entity>)group).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ((IList<Entity>)group).RemoveAt(index);
    }

    public void Clear()
    {
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
}
