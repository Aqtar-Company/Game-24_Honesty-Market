// CollisionOpenPanel.cs
// Attach this script to any GameObject with a Collider2D (or Collider) to open a UI panel when the player collides.

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionOpenPanel : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Drag the UI Panel (GameObject) here to open on collision")]
	public List<GameObject> panelToOpen;
	[Tooltip("The tag used to identify the player object")]
	public string playerTag = "Player";

	private void Awake()
	{
		foreach (GameObject panel in panelToOpen)
		{
			panel.SetActive(false);
		}
	}

	// Use this if your collider is not set as Trigger
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(playerTag))
			OpenPanel();
	}

	// Use this if your collider is set as Trigger
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(playerTag))
			OpenPanel();
	}

	private void OpenPanel()
	{
		foreach (GameObject panel in panelToOpen)
		{
			if (panel != null)
			{
				panel.SetActive(true);
			}
		}
	}
}


