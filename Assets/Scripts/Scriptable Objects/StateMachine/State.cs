using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class State : DescriptionBaseSO
{
    public abstract void InitializeFields(GameObject obj);
    public abstract void OnExit();

    public abstract void OnEnter();

    public abstract System.Type Tick();
}
