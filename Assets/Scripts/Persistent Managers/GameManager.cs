using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;


public enum Scene { 
    IntroMenu,
    Gameplay
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private VoidEventChannel exitChannel;

    public static Coroutine BeginCoroutine(IEnumerator method) {
        return instance.StartCoroutine(method);
    }

    public static void EndCoroutine(Coroutine coroutine) {
        instance.StopCoroutine(coroutine);
    }

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        Application.targetFrameRate = Setting.TARGET_FRAME_RATE;
        QualitySettings.vSyncCount = 0;
        ResourceStorage.LoadResource();
        exitChannel.OnEventRaised += ExitGame;
    }

    public void Update()
    {
    }

    private void LateUpdate()
    {
    }

    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit!");
    }
}
