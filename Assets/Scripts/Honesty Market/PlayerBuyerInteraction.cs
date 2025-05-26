using UnityEngine;
using System.Collections;

public class PlayerBuyerInteraction : MonoBehaviour
{
	[Header("References")]
	public Transform player;                // Player Transform
	public SpriteRenderer playerRenderer;   // Player SpriteRenderer
	public Transform buyer;                 // Buyer Transform
	public SpriteRenderer buyerRenderer;    // Buyer SpriteRenderer

	[Header("Sprites")]
	public Sprite playerIdleSprite;
	public Sprite playerInteractSprite;
	public Sprite buyerIdleSprite;
	public Sprite buyerInteractSprite;

	[Header("Movement")]
	public float moveSpeed = 3f;
	public float stopDistance = 1f;      // How close to stop before buyer

	[Header("Timings")]
	public float interactDelay = 0.4f;   // Wait at buyer before swapping sprites
	public float afterSwapDelay = 0.5f;  // Wait after swapping before returning

	private Vector3 playerStartPos;
	private bool isPlaying = false;

	void OnEnable()
	{
		CarController.Instance.isStopping = true;
		CarController.Instance.StopCar();
		if (!isPlaying)
			StartCoroutine(InteractionSequence());
	}

	IEnumerator InteractionSequence()
	{
		isPlaying = true;

		// Wait one second before starting the sequence
		yield return new WaitForSeconds(3f);

		// Save start position
		playerStartPos = player.position;

		// 1. Move player to buyer (stop before by stopDistance)
		Vector3 target = buyer.position;
		Vector3 direction = (buyer.position - player.position).normalized;
		target -= direction * stopDistance;

		// Face right if not already
		if (player.localScale.x < 0)
			player.localScale = new Vector3(-player.localScale.x, player.localScale.y, player.localScale.z);

		// Move to target
		while (Vector3.Distance(player.position, target) > 0.01f)
		{
			player.position = Vector3.MoveTowards(player.position, target, moveSpeed * Time.deltaTime);
			yield return null;
		}

		// 2. Wait, then swap sprites for both
		yield return new WaitForSeconds(interactDelay);
		if (playerInteractSprite != null)
			playerRenderer.sprite = playerInteractSprite;
		if (buyerInteractSprite != null)
			buyerRenderer.sprite = buyerInteractSprite;

		// 3. Wait, then flip player X and move back to start
		yield return new WaitForSeconds(afterSwapDelay);

		// Flip X (face left)
		player.localScale = new Vector3(-Mathf.Abs(player.localScale.x), player.localScale.y, player.localScale.z);

		// Move back to start
		while (Vector3.Distance(player.position, playerStartPos) > 0.01f)
		{
			player.position = Vector3.MoveTowards(player.position, playerStartPos, moveSpeed * Time.deltaTime);
			yield return null;
		}

		// 4. Disable both GameObjects after a short delay
		yield return new WaitForSeconds(0.2f);
		player.gameObject.SetActive(false);
		buyer.gameObject.SetActive(false);
		CarController.Instance?.ResumeMovement();
		isPlaying = false;
	}
}
