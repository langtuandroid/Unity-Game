using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour 
{
    public void PlayGame() {
        Scene intro = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(intro);
        SceneManager.LoadScene("GameScene");
    }
}
