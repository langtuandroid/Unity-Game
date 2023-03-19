using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Action
{
    public abstract class ActionComponent : ScriptableObject
    {
        public void Reset()
        {
            CleanUp();
            Initialize();
        }

        public virtual void Initialize() { }

        public virtual void CleanUp() { }

        public virtual void Update() { }
    }
}
