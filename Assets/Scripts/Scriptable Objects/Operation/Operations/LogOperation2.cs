using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOperation2 : Operation
{
    protected override void Execute()
    {
        Debug.Log("Logging Operation 2");
    }
}
