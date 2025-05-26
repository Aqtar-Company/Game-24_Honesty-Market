using UnityEngine;

/// <summary>
/// Disables two target GameObjects when this GameObject is disabled.
/// </summary>
public class DisableOnOwnerDisable : MonoBehaviour
{
	[Tooltip("First object to disable when this is disabled.")]
	public GameObject objectToDisable1;
	[Tooltip("Second object to disable when this is disabled.")]
	public GameObject objectToDisable2;

	private void OnDisable()
	{
		if (objectToDisable1 != null)
			objectToDisable1.SetActive(false);

		if (objectToDisable2 != null)
			objectToDisable2.SetActive(false);
	}
}
