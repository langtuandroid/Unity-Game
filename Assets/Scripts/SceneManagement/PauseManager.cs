using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannel gamePauseChannel;
    [SerializeField] private VoidEventChannel gameResumeChannel;
    [SerializeField] private VarBool gamePause;
    [SerializeField] private BoolEventChannel gameUIChannel;
    [SerializeField] private BoolEventChannel pauseMenuChannel;

    private void OnDestroy()
    {
        gamePauseChannel.OnEventRaised -= PauseGame;
        gameResumeChannel.OnEventRaised -= ResumeGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        gamePauseChannel.OnEventRaised += PauseGame;
        gameResumeChannel.OnEventRaised += ResumeGame;
        gameUIChannel.RaiseEvent(true);
        pauseMenuChannel.RaiseEvent(false);
    }

    public void PauseOrResume() {
        if (!gamePause.Value)
        {
            gamePauseChannel.RaiseEvent();
        }
        else {
            gameResumeChannel.RaiseEvent();
        }
    }

    private void PauseGame()
    {
        Debug.Log("Game Pause!");
        Time.timeScale = 0f;
        gameUIChannel.RaiseEvent(false);
        pauseMenuChannel.RaiseEvent(true);
        gamePause.Value = true;
    }

    private void ResumeGame()
    {
        Debug.Log("Game Resume!");
        Time.timeScale = 1;
        gamePause.Value = false;
        gameUIChannel.RaiseEvent(true);
        pauseMenuChannel.RaiseEvent(false);
    }
}
