using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private VoidEventChannel resumeChannel;
    [SerializeField] private SceneEventChannel loadChannel;
    [SerializeField] private SceneEventChannel unloadChannel;
    [SerializeField] private BoolEventChannel enableChannel;

    private void Start()
    {
        enableChannel.OnEventRaised += gameObject.SetActive;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        enableChannel.OnEventRaised -= gameObject.SetActive;
    }

    public void ResumeGame() { // Resume button
        resumeChannel.RaiseEvent();
    }

    public void ExitToMenu() { // Exit Button
        resumeChannel.RaiseEvent();
        loadChannel.RaiseEvent(Scene.IntroMenu);
        unloadChannel.RaiseEvent(Scene.Gameplay);
    }
}
