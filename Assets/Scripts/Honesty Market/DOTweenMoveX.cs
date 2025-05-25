using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DOTweenMoveXWithSpriteAnim : MonoBehaviour
{
	[Header("Move Settings")]
	public float moveDistance = 5f;
	public float duration = 1.5f;
	public bool isRelative = true;

	[Header("Sprite Animation")]
	public Sprite[] frames;           // List of sprites for animation
	public float frameRate = 10f;     // Frames per second

	private Tween moveTween;
	private Coroutine animRoutine;
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		MoveX();
	}

	public void MoveX()
	{
		// Kill any previous tween or animation
		if (moveTween != null && moveTween.IsActive()) moveTween.Kill();
		if (animRoutine != null) StopCoroutine(animRoutine);

		// Start moving
		moveTween = transform.DOMoveX(
			isRelative ? transform.position.x + moveDistance : moveDistance,
			duration
		).OnComplete(() =>
		{
			// On finish, show first frame
			if (frames != null && frames.Length > 0)
				spriteRenderer.sprite = frames[0];
		});

		// Start sprite animation while moving (if frames exist)
		if (frames != null && frames.Length > 0)
			animRoutine = StartCoroutine(SpriteAnimLoop());
	}

	private IEnumerator SpriteAnimLoop()
	{
		int frame = 0;
		float frameTime = 1f / frameRate;
		float timer = 0;
		while (moveTween != null && moveTween.IsActive())
		{
			timer += Time.deltaTime;
			if (timer >= frameTime)
			{
				frame = (frame + 1) % frames.Length;
				spriteRenderer.sprite = frames[frame];
				timer -= frameTime;
			}
			yield return null;
		}
		// On finish, reset to first frame
		if (frames != null && frames.Length > 0)
			spriteRenderer.sprite = frames[0];
	}

	public void KillTween()
	{
		if (moveTween != null && moveTween.IsActive()) moveTween.Kill();
		if (animRoutine != null) StopCoroutine(animRoutine);
	}
}
