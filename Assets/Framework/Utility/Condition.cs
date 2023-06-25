using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework.Utility
{
    public abstract class Condition : MonoBehaviour
    {
        public abstract bool Eval();
    }
}
