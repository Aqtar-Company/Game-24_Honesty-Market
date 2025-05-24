using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBarController : MonoBehaviour
{
	public Animator animator;       // Assign your Animator
	public Button targetButton;     // Assign your Button

	void Start()
	{
		targetButton.gameObject.SetActive(false); // Hide button at the start
		StartCoroutine(WaitForAnimationToEnd());
	}

	private IEnumerator WaitForAnimationToEnd()
	{
		// Get the current playing animation state
		AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
		float animationLength = animState.length;

		// Wait for the animation duration
		yield return new WaitForSeconds(animationLength);

		// Activate the button after the animation finishes
		targetButton.gameObject.SetActive(true);
	}
}
