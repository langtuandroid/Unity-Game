using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using LobsterFramework.Utility;

public enum Scene { 
    IntroMenu,
    Gameplay
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance {get {return instance;}}

    [SerializeField] private VoidEventChannel exitChannel;
    // Game Settings
    [field: SerializeField] public int TARGET_FRAME_RATE { get; private set; }

    // Attack info duration (seconds)
    [field: SerializeField] public float EXPIRE_ATTACK_TIME { get; private set; }

    //
    [field: SerializeField] public float POSTURE_BROKEN_DAMAGE_MODIFIER { get; private set; }
    [field: SerializeField] public float POSTURE_BROKEN_DURATION { get; private set; }
    [field: SerializeField] public float SUPPRESS_REGEN_DURATION { get; private set; }
    [field: SerializeField] public string TAG_ENTITY { get; private set; }
    [field: SerializeField] public string TAG_PROJECTILE { get; private set; }
    [field: SerializeField] public string TAG_PLAYER { get; private set; }
    [field: SerializeField] public string TAG_INTERACTABLE { get; private set; }

    [field: SerializeField] public string TAG_INTERACTOR { get; private set; }

    private Dictionary<UnityAction, float> delegates;

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
        Application.targetFrameRate = TARGET_FRAME_RATE;
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

    /// <summary>
    /// Execute UnityAction after specified seconds of delay in the Update Loop. If delay is set to 0, action will be executed on next frame
    /// </summary>
    /// <param name="action">Action to be executed</param>
    /// <param name="timeDelay"></param>
    public static void ExecuteDelegate(UnityAction action, float timeDelay) {
        if (timeDelay < 0) {
            timeDelay = 0;
        }
        instance.delegates[action] = timeDelay;
    }

    private void LateUpdate()
    {
    }

    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit!");
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void InitializeAttributes() { 
        foreach(Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
            AttributeInitializer.Initialize(assembly);
        }
    }
}
