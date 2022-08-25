using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    public void Start()
    {
        gameObject.SetActive(false);
    }
    public abstract bool Eval();
}
