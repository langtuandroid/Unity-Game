using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    public abstract class AbilityStat : ScriptableObject
    {
        public void Reset()
        {
            OnClose();
            Initialize();
        }

        /// <summary>
        /// Initialize the component
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Callback before the parent operator is disabled
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// Callback to update internal data on each frame
        /// </summary>
        public virtual void Update() { }
    }
}
