using UnityEngine;
using UnityEngine.InputSystem;

namespace LobsterFramework
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private SceneEventChannel loadSceneChannel;
        [SerializeField] private SceneEventChannel unloadSceneChannel;

        public void ExitToMenu()
        {
            Time.timeScale = 1;
            unloadSceneChannel.RaiseEvent(Scene.Gameplay);
            loadSceneChannel.RaiseEvent(Scene.IntroMenu);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
