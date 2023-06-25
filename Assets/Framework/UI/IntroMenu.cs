using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroMenu : MonoBehaviour
{
    [SerializeField] Scene gameScene;
    [SerializeField] Scene introScene;
    [SerializeField] private SceneEventChannel loadSceneChannel;
    [SerializeField] private SceneEventChannel unloadSceneChannel;

    public void PlayGame() {
        loadSceneChannel.RaiseEvent(gameScene);
        unloadSceneChannel.RaiseEvent(introScene);
    }
}
