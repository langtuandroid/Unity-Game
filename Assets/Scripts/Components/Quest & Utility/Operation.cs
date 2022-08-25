using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operation : MonoBehaviour
{
    // Override this to implement custom behavior
    public abstract void Begin();

    public void Start()
    {
        gameObject.SetActive(false);
    }
}
