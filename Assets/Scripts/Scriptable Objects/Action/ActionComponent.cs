using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionComponent : ScriptableObject
{
    public virtual void Initialzie() { }

    public virtual void Update() { }
}
