using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOperation : Operation
{
    protected override void Execute()
    {
        Debug.Log("Logging Operation 1");
    }
}
