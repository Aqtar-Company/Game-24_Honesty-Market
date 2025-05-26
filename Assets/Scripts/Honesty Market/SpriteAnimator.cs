using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
	[Tooltip("List your animation frames here, in order.")]
	public Sprite[] frames;

	[Tooltip("Frames per second.")]
	public float framesPerSecond = 12f;

	[Tooltip("Loop the animation when it reaches the end of the specified range?")]
	public bool loop = true;

	[Header("Custom Range (0-based indices)")]
	[Tooltip("Inclusive start frame index. Must be >= 0.")]
	public int startFrameIndex = 0;
	[Tooltip("Inclusive end frame index. Set to -1 to use last frame.")]
	public int endFrameIndex = -1;

	[Header("Non-loop Repeat Settings")]
	[Tooltip("How many times to replay the animation when loop = false.")]
	[Range(1, 10)]
	public int repeatCount = 3;

	[Header("Sprite Hide")]
	[Tooltip("Hide sprite when disabled?")]
	public bool hideWhenDisabled = false;

	[Header("Finish Event")]
	[Tooltip("This GameObject will be activated when animation completes (loop=false, all repeats finished).")]
	public GameObject activateOnFinish;

	private SpriteRenderer spriteRenderer;
	private int currentFrame;
	private int actualEndFrame;
	private float timer;

	// tracks how many times we've completed a non-loop pass
	private int nonLoopPlayCount = 0;
	public int NonLoopPlayCount => nonLoopPlayCount;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		StartAnimation();
	}

	void OnDisable()
	{
		// Optionally hide the sprite when disabled
		if (hideWhenDisabled && spriteRenderer != null)
		{
			spriteRenderer.sprite = null;
		}
		// Reset the state for next enable
		timer = 0f;
		nonLoopPlayCount = 0;
	}

	private void StartAnimation()
	{
		if (frames == null || frames.Length == 0)
		{
			enabled = false;
			return;
		}

		// Clamp our range
		startFrameIndex = Mathf.Clamp(startFrameIndex, 0, frames.Length - 1);
		actualEndFrame = (endFrameIndex < 0 || endFrameIndex >= frames.Length)
			? frames.Length - 1
			: endFrameIndex;
		actualEndFrame = Mathf.Max(actualEndFrame, startFrameIndex);

		// Reset state
		currentFrame = startFrameIndex;
		timer = 0f;
		nonLoopPlayCount = 0;
		spriteRenderer.sprite = frames[currentFrame];
		enabled = true;

		// Deactivate the finish object if needed at the start
		if (activateOnFinish != null)
			activateOnFinish.SetActive(false);
	}

	void Update()
	{
		if (frames == null || frames.Length == 0) return;

		timer += Time.deltaTime;
		float frameTime = 1f / framesPerSecond;

		if (timer >= frameTime)
		{
			timer -= frameTime;
			currentFrame++;

			if (currentFrame > actualEndFrame)
			{
				if (loop)
				{
					currentFrame = startFrameIndex;
				}
				else
				{
					nonLoopPlayCount++;
					if (nonLoopPlayCount < repeatCount)
					{
						// replay again
						currentFrame = startFrameIndex;
					}
					else
					{
						// done repeating: hold final frame
						currentFrame = actualEndFrame;
						enabled = false;

						// ACTIVATE OBJECT ON FINISH
						if (activateOnFinish != null)
							activateOnFinish.SetActive(true);
					}
				}
			}

			spriteRenderer.sprite = frames[currentFrame];
		}
	}

	/// <summary>
	/// Manually restart the animation at any time.
	/// </summary>
	public void Play()
	{
		StartAnimation();
	}

	/// <summary>
	/// Pause the animation (holds current frame).
	/// </summary>
	public void Stop()
	{
		enabled = false;
	}

	/// <summary>
	/// Jump to a specific frame within the valid range.
	/// </summary>
	public void GoToFrame(int index)
	{
		if (frames == null || frames.Length == 0) return;
		index = Mathf.Clamp(index, startFrameIndex, actualEndFrame);
		currentFrame = index;
		spriteRenderer.sprite = frames[currentFrame];
	}

	// ── EDITOR SUPPORT ──
	void OnValidate()
	{
		if (frames != null && frames.Length > 0)
		{
			startFrameIndex = Mathf.Clamp(startFrameIndex, 0, frames.Length - 1);
			endFrameIndex = (endFrameIndex < 0 || endFrameIndex >= frames.Length)
				? frames.Length - 1
				: endFrameIndex;
		}
		repeatCount = Mathf.Max(1, repeatCount);
	}
	// ─────────────────────
}
