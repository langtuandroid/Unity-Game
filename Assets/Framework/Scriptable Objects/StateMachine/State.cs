using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.AI
{
    public abstract class State : ScriptableObject
    {
        [HideInInspector]
        public AIController controller;
        public abstract void InitializeFields(GameObject obj);
        public abstract void OnExit();

        public abstract void OnEnter();

        public abstract System.Type Tick();
    }
}
