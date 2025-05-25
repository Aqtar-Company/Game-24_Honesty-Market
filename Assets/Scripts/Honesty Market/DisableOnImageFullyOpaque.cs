using UnityEngine;
using UnityEngine.UI;

public class DisableOnImageFullyOpaque : MonoBehaviour
{
	public Image imageToWatch;          // The image whose alpha you want to check
	public GameObject objectToDisable;  // The object to disable when image alpha is 1

	private bool hasDisabled = false;

	void Update()
	{
		if (!hasDisabled && imageToWatch != null && objectToDisable != null)
		{
			if (imageToWatch.color.a >= 0.99f) // Close enough to 1 for UI float accuracy
			{
				objectToDisable.SetActive(false);
				hasDisabled = true; // Make sure it only happens once
			}
		}
	}
}
