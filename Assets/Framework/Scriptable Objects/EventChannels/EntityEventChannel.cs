using UnityEngine;
using UnityEngine.Events;

namespace LobsterFramework.Utility.EventChannels
{
    public class EntityEventChannel : ScriptableObject
    {
        public UnityAction<Entity> onEventRaised;

        public void RaiseEvent(Entity entity)
        {
            if (onEventRaised != null)
            {
                onEventRaised.Invoke(entity);
            }
        }
    }
}
