using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeImage : MonoBehaviour
{
	public Image fadeImage; // The UI Image used for fading
	public float fadeDuration = 0.5f; // Duration of fade in/out
	public float waitTime = 3f; // Time to wait before fade out
	public int nextSceneIndex = 1; // Scene to load after fade sequence

	void Start()
	{
		if (fadeImage == null)
		{
#if UNITY_EDITOR
			Debug.LogError("FadeImage: No Image assigned! Assign an Image component.");
#endif
			return;
		}

		fadeImage.gameObject.SetActive(true); // Ensure the fade image is active
		StartCoroutine(FadeSequence());
	}

	IEnumerator FadeSequence()
	{
		yield return StartCoroutine(Fade(0f, 1f, fadeDuration)); // Fade In
		yield return new WaitForSeconds(waitTime);
		yield return StartCoroutine(Fade(1f, 0f, fadeDuration)); // Fade Out

		// Ensure the scene index is valid before loading
		if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(nextSceneIndex);
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("FadeImage: Invalid nextSceneIndex! Check your Build Settings.");
#endif
		}
	}

	IEnumerator Fade(float startAlpha, float targetAlpha, float duration)
	{
		float elapsed = 0f;
		Color color = fadeImage.color;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
			fadeImage.color = color;
			yield return null;
		}

		color.a = targetAlpha;
		fadeImage.color = color;
	}
}
