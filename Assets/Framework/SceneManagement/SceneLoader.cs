using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the scene loading and unloading.
/// </summary>
public class SceneLoader : MonoBehaviour
{
	[SerializeField] private SceneEventChannel loadChannel;
	[SerializeField] private SceneEventChannel unloadChannel;

    private void Start()
    {
		loadChannel.OnEventRaised += LoadScene;
		unloadChannel.OnEventRaised += UnloadScene;
    }

	private void LoadScene(Scene scene) {
		SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
	}

	private void UnloadScene(Scene scene) {
		SceneManager.UnloadSceneAsync(scene.ToString());
	}
}
