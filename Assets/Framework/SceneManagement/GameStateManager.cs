using UnityEngine;
using UnityEngine.InputSystem;

namespace LobsterFramework
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private SceneEventChannel loadSceneChannel;
        [SerializeField] private SceneEventChannel unloadSceneChannel;
        [SerializeField] private VoidEventChannel gamePauseChannel;
        [SerializeField] private VoidEventChannel gameResumeChannel;
        [SerializeField] private VoidEventChannel playerDeathChannel;
        [SerializeField] private VoidEventChannel playerRespawnChannel;

        [Header("Menus")]
        [SerializeField] private RectTransform gameplayMenu;
        [SerializeField] private RectTransform pauseMenu;
        [SerializeField] private RectTransform respawnMenu;

        [Header("Variables")]
        [SerializeField] private VarBool gamePause;

        private void OnDestroy()
        {
            gamePauseChannel.OnEventRaised -= PauseGame;
            playerDeathChannel.OnEventRaised -= OnPlayerDeath;
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            gamePauseChannel.OnEventRaised += PauseGame;
            playerDeathChannel.OnEventRaised += OnPlayerDeath;
            ResumeGame();
        }

        public void OnPlayerDeath()
        {
            Debug.Log("Waiting for Respawn!");
            gameplayMenu.gameObject.SetActive(false);
            respawnMenu.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        public void RespawnPlayer()
        {
            Debug.Log("Respawning Player!");
            playerRespawnChannel.RaiseEvent();
            gameplayMenu.gameObject.SetActive(true);
            respawnMenu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Pause(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (!gamePause.Value)
                {
                    
                    PauseGame();
                }
                else
                {
                    
                    ResumeGame();
                }
            }
        }

        public void PauseGame()
        {
            Debug.Log("Game Pause!");
            pauseMenu.gameObject.SetActive(true);
            gameplayMenu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0f;
            gamePause.Value = true;
        }

        public void ResumeGame()
        {
            Debug.Log("Game Resume!");
            gameplayMenu.gameObject.SetActive(true);
            pauseMenu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            gamePause.Value = false;
            gameResumeChannel.RaiseEvent();
        }

        public void ExitToMenu()
        {
            Time.timeScale = 1;
            gamePause.Value = false;
            unloadSceneChannel.RaiseEvent(Scene.Gameplay);
            loadSceneChannel.RaiseEvent(Scene.IntroMenu);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
