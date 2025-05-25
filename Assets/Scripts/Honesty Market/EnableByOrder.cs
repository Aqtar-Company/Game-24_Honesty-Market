using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnableByOrder : MonoBehaviour
{
	[Header("Objects to Enable")]
	public List<GameObject> objectsToEnable;

	public float waitBeforeEnable = 2.3f; // Seconds to wait before enabling

	private int currentIndex = 0;
	private bool isWaiting = false;

	private void Awake()
	{
		foreach (GameObject obj in objectsToEnable)
		{
			obj.SetActive(false);
		}
	}

	/// <summary>
	/// Call this to enable the next object, with a wait before enabling.
	/// If this script's GameObject may be inactive, use the static helper below!
	/// </summary>
	public void EnableNextObject()
	{
		if (!isWaiting && currentIndex < objectsToEnable.Count)
		{
			StartCoroutine(WaitAndEnable());
		}
	}

	private IEnumerator WaitAndEnable()
	{
		isWaiting = true;
		yield return new WaitForSeconds(waitBeforeEnable);

		if (currentIndex < objectsToEnable.Count)
		{
			objectsToEnable[currentIndex].SetActive(true);
			currentIndex++;
		}

		isWaiting = false;
	}

	/// <summary>
	/// Optional: Reset all objects to inactive and start index over.
	/// </summary>
	public void ResetAll()
	{
		StopAllCoroutines();
		foreach (GameObject obj in objectsToEnable)
		{
			obj.SetActive(false);
		}
		currentIndex = 0;
		isWaiting = false;
	}

	/// <summary>
	/// Static utility to run an enable coroutine from any always-active runner GameObject.
	/// Usage: EnableByOrder.RunEnableCoroutine(runner, objectsList, delay, index)
	/// </summary>
	public static void RunEnableCoroutine(MonoBehaviour runner, List<GameObject> objects, float delay, int startIndex = 0)
	{
		if (runner != null && runner.gameObject.activeInHierarchy)
		{
			runner.StartCoroutine(EnableSequence(objects, delay, startIndex));
		}
	}

	private static IEnumerator EnableSequence(List<GameObject> objects, float delay, int startIndex)
	{
		for (int i = startIndex; i < objects.Count; i++)
		{
			yield return new WaitForSeconds(delay);
			if (objects[i] != null)
				objects[i].SetActive(true);
		}
	}
}
