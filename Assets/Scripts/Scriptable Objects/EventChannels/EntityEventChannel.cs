using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LobsterFramework.EntitySystem;

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
