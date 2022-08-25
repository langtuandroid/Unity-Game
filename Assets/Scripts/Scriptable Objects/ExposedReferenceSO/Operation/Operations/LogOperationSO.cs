using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Operations/Log")]
public class LogOperationSO : OperationSO
{
    [SerializeField] private string content;
    public override void Operate()
    {
        Debug.Log(content);
    }
}
