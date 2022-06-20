using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOverseer : MonoBehaviour
{
    void LateUpdate()
    {
        ActionInstance.ExecuteActions();
    }
}
