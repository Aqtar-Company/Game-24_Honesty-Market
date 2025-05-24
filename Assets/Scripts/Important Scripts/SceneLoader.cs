using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// Handles scene transitions and ensures all active DOTween tweens are killed before changing scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
	/// <summary>
	/// Kills all active tweens in the scene to prevent lingering animations.
	/// </summary>
	private void ClearAllTweens()
	{
		// Kill all tweens without completing them.
		DOTween.KillAll();
		// If you want to complete active tweens before killing, use:
		// DOTween.KillAll(complete: true);
	}

	/// <summary>
	/// Loads a scene by its index in Build Settings.
	/// </summary>
	public void LoadSceneByIndex(int sceneIndex)
	{
		ClearAllTweens();
		if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(sceneIndex);
#if UNITY_EDITOR
			Debug.Log($"➡️ Loading Scene Index: {sceneIndex}");
#endif
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("❌ Invalid scene index. Make sure the scene is added to Build Settings.");
#endif
		}
	}

	/// <summary>
	/// Reloads the currently active scene.
	/// </summary>
	public void ReloadCurrentScene()
	{
		ClearAllTweens();
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(currentSceneIndex);
#if UNITY_EDITOR
		Debug.Log($"🔄 Reloading Scene Index: {currentSceneIndex}");
#endif
	}

	/// <summary>
	/// Loads the next scene in Build Settings.
	/// </summary>
	public void LoadNextScene()
	{
		ClearAllTweens();
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		int nextSceneIndex = currentSceneIndex + 1;

		if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(nextSceneIndex);
#if UNITY_EDITOR
			Debug.Log($"➡️ Loading Next Scene Index: {nextSceneIndex}");
#endif
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogWarning("🔚 No more scenes in Build Settings to load.");
#endif
		}
	}
}
