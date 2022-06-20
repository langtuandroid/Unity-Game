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

    void Start()
    {
        Application.targetFrameRate = Setting.TARGET_FRAME_RATE;
        QualitySettings.vSyncCount = 0;
        ResourceStorage.LoadResource();
        exitChannel.OnEventRaised += ExitGame;
    }

    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit!");
    }
}
