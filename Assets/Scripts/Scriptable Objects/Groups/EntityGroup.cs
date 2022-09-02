using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Groups/Entity Group")]
public class EntityGroup : ScriptableObject, IEnumerable<Entity>
{
    [SerializeField] private List<Entity> group = new();
    public UnityEvent<Entity> OnEntityAdded = new();
    public UnityEvent<Entity> OnEntityRemoved = new();
    

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
}
