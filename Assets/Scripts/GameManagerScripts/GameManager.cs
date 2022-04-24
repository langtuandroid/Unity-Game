using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResourceStorage.LoadResource();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ActionInstance.ExecuteActions();
        Entity.UpdateEntityStatus();
    }
}
