using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AbilitySystem
{
    public abstract class AbilityStat : ScriptableObject
    {
        public void Reset()
        {
            CleanUp();
            Initialize();
        }

        /// <summary>
        /// Initialize the component
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Callback to clean up dangling strings before the parent operator is destoyed
        /// </summary>
        public virtual void CleanUp() { }

        /// <summary>
        /// Callback to update internal data on each frame
        /// </summary>
        public virtual void Update() { }
    }
}
