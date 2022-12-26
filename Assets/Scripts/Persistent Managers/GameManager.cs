using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.Events;


public enum Scene { 
    IntroMenu,
    Gameplay
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private VoidEventChannel exitChannel;

    private Dictionary<UnityAction, float> delegates;

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
        delegates = new();
    }

    public void Update()
    {
        List<UnityAction> remove = new();
        List<UnityAction> keys = new(delegates.Keys);
        foreach (UnityAction ac in keys) {
            delegates[ac] -= Time.deltaTime;
            if (delegates[ac] <= 0) {
                ac.Invoke();
                remove.Add(ac);
            }
        }
        foreach (UnityAction ac in remove) {
            delegates.Remove(ac);
        }
    }

    public static void ExecuteDelegate(UnityAction method, float time) {
        instance.delegates[method] = time;
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
