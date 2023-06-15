using UnityEngine;
using UnityEngine.InputSystem;

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
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void PauseOrResume() {
        if (!gamePause.Value)
        {
            PauseGame();
        }
        else {
            ResumeGame();
        }
    }

    public void OnPlayerDeath() {
        Debug.Log("Waiting for Respawn!");
        gameplayMenu.gameObject.SetActive(false);
        respawnMenu.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void RespawnPlayer() {
        Debug.Log("Respawning Player!");
        playerRespawnChannel.RaiseEvent();
        gameplayMenu.gameObject.SetActive(true);
        respawnMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started) {
            PauseOrResume();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Game Pause!");
        Time.timeScale = 0f;
        pauseMenu.gameObject.SetActive(true);
        gameplayMenu.gameObject.SetActive(false);
        gamePause.Value = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resume!");
        Time.timeScale = 1;
        gamePause.Value = false;
        gameplayMenu.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        gameResumeChannel.RaiseEvent();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitToMenu() {
        Time.timeScale = 1;
        gamePause.Value = false;
        unloadSceneChannel.RaiseEvent(Scene.Gameplay);
        loadSceneChannel.RaiseEvent(Scene.IntroMenu);
        Cursor.lockState = CursorLockMode.None;
    }
}
