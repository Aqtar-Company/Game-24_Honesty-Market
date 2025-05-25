using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ConversationManager : MonoBehaviour
{
	[System.Serializable]
	public class CanvasPanelImage
	{
		public Canvas canvas;                   // The canvas object
		public Image panelGroup;                // The panel as an Image
		public List<Image> imageGroup;          // Images to fade, in order
	}

	public CanvasPanelImage[] steps;            // Steps in order
	public float fadeDuration = 0.5f;
	public float waitBetween = 0.15f;

	private bool running = false;

	// 1. Disable all objects at Awake (even if active in the scene)
	void Awake()
	{
		foreach (var step in steps)
		{
			if (step.canvas) step.canvas.gameObject.SetActive(false);
			if (step.panelGroup) step.panelGroup.gameObject.SetActive(false);
			if (step.imageGroup != null)
				foreach (var img in step.imageGroup)
					if (img) img.gameObject.SetActive(false);
		}
	}

	// 2. Call this to trigger the sequence (from a collider/trigger)
	public void StartConversation()
	{
		if (!running)
			StartCoroutine(ShowConversationRoutine());
	}

	private IEnumerator ShowConversationRoutine()
	{
		running = true;

		foreach (var step in steps)
		{
			// ---- Activate Canvas ----
			step.canvas.gameObject.SetActive(true);

			// ---- Activate and Fade In Panel ----
			step.panelGroup.gameObject.SetActive(true);
			var panelColor = step.panelGroup.color;
			panelColor.a = 0f;
			step.panelGroup.color = panelColor;
			yield return step.panelGroup.DOFade(1f, fadeDuration).WaitForCompletion();
			yield return new WaitForSeconds(waitBetween);

			// ---- Activate and Fade In Images One by One ----
			foreach (var img in step.imageGroup)
			{
				img.gameObject.SetActive(true);
				var c = img.color;
				c.a = 0f;
				img.color = c;
				yield return img.DOFade(1f, fadeDuration).WaitForCompletion();
				yield return new WaitForSeconds(waitBetween);
			}

			// Optional: Hide canvas before next step (remove if you want both to stay)
			//step.canvas.gameObject.SetActive(false);
		}

		running = false;
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
			StartConversation();
	}
}


