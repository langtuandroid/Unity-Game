using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOperation : Operation
{
    [SerializeField] private string str;
    public override void Begin()
    {
        Debug.Log(str);
    }
}
