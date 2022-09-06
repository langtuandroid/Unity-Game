using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class State
{
    public abstract void InitializeFields(GameObject obj);
    public abstract void OnExit();

    public abstract void OnEnter();

    public abstract System.Type Tick();
}
