using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public abstract void InitializeFields(GameObject obj);
    public abstract void OnExit();

    public abstract void OnEnter();

    public abstract System.Type Tick();
}
