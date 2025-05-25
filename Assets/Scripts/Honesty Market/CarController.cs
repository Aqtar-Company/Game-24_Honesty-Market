using System.Collections;
using UnityEngine;

/// <summary>
/// CarController handles horizontal movement, smooth stopping on trigger, overturn detection, and bounds check.
/// Also supports mobile button input, can resume movement via ResumeMovement(),
/// exposes a singleton Instance for global access,
/// and triggers move sound & particles when moving.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
	/// <summary>Singleton instance of the CarController</summary>
	public static CarController Instance { get; private set; }

	[Header("Movement Settings")]
	[Tooltip("Max horizontal speed")] public float moveSpeed = 3f;
	[Tooltip("Acceleration rate towards target speed")] public float acceleration = 2f;

	[Header("Overturn & Screen Bounds")]
	[Tooltip("Degrees before car considered overturned")] public float overturnThreshold = 45f;

	[Header("Stop Lift")]
	[Tooltip("Upward impulse applied once the car stops")] public float stopLiftAmount = 0.5f;

	[Header("Audio & Effects")]
	public string moveSoundKey = "car_move";      // Key for looping car move sound in SoundManager
	public ParticleSystem moveParticles;          // Assign a ParticleSystem (e.g., dust) as child of the car

	// References
	private Rigidbody2D rb;
	private WheelJoint2D[] wheels;

	// Runtime state
	private float currentSpeed = 0f;
	private bool gameLost = false;
	private bool isPaused = false;
	private bool isStopping = false;
	private bool isMoving = false;

	// Input state
	private float inputDirection = 0f;
	private bool moveLeftInput = false;
	private bool moveRightInput = false;

	void Awake()
	{
		// Singleton setup
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
	}

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		wheels = GetComponents<WheelJoint2D>();

		rb.bodyType = RigidbodyType2D.Dynamic;
		foreach (var w in wheels)
		{
			w.useMotor = true;
		}
	}

	void Update()
	{
		if (gameLost) return;

		if (isPaused)
		{
			inputDirection = 0f;
			HandleMoveEffects(false);
			return;
		}

#if UNITY_ANDROID || UNITY_IOS
        // Use only UI button input for mobile (ignore keyboard/tilt here for clarity)
        // inputDirection is managed by button event functions
#else
		// Desktop: use keyboard (UI buttons still call OnLeftButtonDown/Up)
		float kb = Input.GetAxisRaw("Horizontal");
		if (moveLeftInput) inputDirection = -1f;
		else if (moveRightInput) inputDirection = 1f;
		else inputDirection = kb;
#endif

		// Are we trying to move?
		bool shouldMove = Mathf.Abs(inputDirection) > 0.01f;
		HandleMoveEffects(shouldMove);
	}

	void FixedUpdate()
	{
		if (gameLost) return;

		float targetSpeed = Mathf.Abs(inputDirection) > 0f ? moveSpeed : 0f;
		currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
		rb.linearVelocity = new Vector2(inputDirection * currentSpeed, rb.linearVelocity.y);

		float ang = Mathf.Abs(NormalizeAngle(transform.eulerAngles.z));
		if (ang > overturnThreshold)
		{
			Debug.Log("Car overturned.");
			gameLost = true;
			HandleMoveEffects(false);
		}

		// Screen bounds check
		if (Camera.main != null)
		{
			var renderers = GetComponentsInChildren<Renderer>();
			if (renderers.Length > 0)
			{
				Bounds bounds = renderers[0].bounds;
				for (int i = 1; i < renderers.Length; i++)
					bounds.Encapsulate(renderers[i].bounds);

				Vector3 minVP = Camera.main.WorldToViewportPoint(bounds.min);
				Vector3 maxVP = Camera.main.WorldToViewportPoint(bounds.max);
				if (maxVP.x < 0f || minVP.x > 1f || maxVP.y < 0f || maxVP.y > 1f)
				{
					Debug.Log("Car left the screen.");
					gameLost = true;
					HandleMoveEffects(false);
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("PuzzleTrigger") && !isStopping)
		{
			isStopping = true;
			StartCoroutine(StopCarCoroutine());
		}
	}

	private IEnumerator StopCarCoroutine()
	{
		isPaused = true;
		moveLeftInput = moveRightInput = false;

		// Stop sound and particles immediately
		HandleMoveEffects(false);

		foreach (var w in wheels)
			w.useMotor = false;

		while (currentSpeed > 0.01f)
		{
			currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
			rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * currentSpeed, rb.linearVelocity.y);
			yield return new WaitForFixedUpdate();
		}

		rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
		rb.AddForce(Vector2.up * stopLiftAmount, ForceMode2D.Impulse);
	}

	/// <summary>
	/// Resumes movement after being paused by StopCarCoroutine.
	/// </summary>
	public void ResumeMovement()
	{
		if (!isStopping) return;
		isPaused = false;
		isStopping = false;
		HandleMoveEffects(true);
		foreach (var w in wheels)
			w.useMotor = true;
	}

	/// <summary>
	/// Handles the sound and particle effects for movement.
	/// Call with shouldMove=true to play, shouldMove=false to stop.
	/// </summary>
	private void HandleMoveEffects(bool shouldMove)
	{
		if (shouldMove && !isMoving)
		{
			// Start sound loop if not already playing
			SoundManager.Instance?.LoopSound(moveSoundKey, true);
			// Start particle
			if (moveParticles != null && !moveParticles.isPlaying)
				moveParticles.Play();

			isMoving = true;
		}
		else if (!shouldMove && isMoving)
		{
			// Stop sound loop
			SoundManager.Instance?.LoopSound(moveSoundKey, false);
			// Stop particle
			if (moveParticles != null && moveParticles.isPlaying)
				moveParticles.Play();

			isMoving = false;
		}
	}

	private float NormalizeAngle(float angle)
	{
		while (angle > 180f) angle -= 360f;
		while (angle < -180f) angle += 360f;
		return angle;
	}

	// Mobile button inputs (for UI buttons)
	public void OnLeftButtonDown() { moveLeftInput = true; }
	public void OnLeftButtonUp() { moveLeftInput = false; }
	public void OnRightButtonDown() { moveRightInput = true; }
	public void OnRightButtonUp() { moveRightInput = false; }
}
