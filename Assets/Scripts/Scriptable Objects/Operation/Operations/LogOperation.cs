using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Operations/Log")]
public class LogOperation : Operation
{
    [SerializeField] private string content;
    protected override void Execute()
    {
        Debug.Log(content);
    }
}
