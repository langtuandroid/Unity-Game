using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Scene { 
    IntroMenu,
    Gameplay
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannel exitChannel;
    [SerializeField] private QuestManager questManager;
    void Start()
    {
        Application.targetFrameRate = Setting.TARGET_FRAME_RATE;
        QualitySettings.vSyncCount = 0;
        ResourceStorage.LoadResource();
        exitChannel.OnEventRaised += ExitGame;
    }

    private void LateUpdate()
    {
        questManager.ProcessQuest();
    }

    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit!");
    }

    public bool SwitchGameState() {
        return true;
    }
}
