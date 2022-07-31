using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private BoolEventChannel enableChannel;
    [SerializeField] private Quest quest;
    private void Start()
    {
        enableChannel.OnEventRaised += gameObject.SetActive;
    }

    private void OnDestroy()
    {
        enableChannel.OnEventRaised -= gameObject.SetActive;
    }

    public void EnableQuest() { 
        quest.Enable();
    }
}
