using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Quest quest;

    public void EnableQuest() { 
        quest.Enable();
    }
}
