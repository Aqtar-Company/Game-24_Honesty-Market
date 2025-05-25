using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class BasketballThrowUIPanel : MonoBehaviour
{
	[Header("Throw Settings")]
	public RectTransform throwFrom;         // The starting UI element
	public RectTransform throwTo;           // The target UI element (basket)
	public RectTransform ballPrefab;        // The UI ball prefab (should be a RectTransform/Image)
	public RectTransform parentPanel;       // The parent panel for UI (RectTransform)

	public float throwDuration = 1f;        // Total duration of the throw
	public float arcHeight = 100f;          // Height of the arc in UI units

	[Header("Bounce Settings")]
	public float bounceHeight = 40f;        // Height of first bounce
	public float bounceDuration = 0.18f;    // Duration of each bounce
	public int bounceCount = 2;             // Number of bounces (will shrink in height each time)
	public float bounceShrink = 0.4f;       // Each bounce height is multiplied by this


	public GameObject paneltoEnable;


	private void OnEnable()
	{
		//paneltoEnable.gameObject.SetActive(false);
	}
	public void ThrowBallInPanel()
	{
		RectTransform ball = Instantiate(ballPrefab, parentPanel);
		ball.SetAsLastSibling();
		paneltoEnable.SetActive(true);
		StartCoroutine(ParabolaThrowWithBounce(ball));
	}

	private IEnumerator ParabolaThrowWithBounce(RectTransform ball)
	{
		Vector2 start = throwFrom.anchoredPosition;
		Vector2 end = throwTo.anchoredPosition;

		float elapsed = 0f;
		while (elapsed < throwDuration)
		{
			float t = elapsed / throwDuration;
			Vector2 pos = Vector2.Lerp(start, end, t);
			float height = 4 * arcHeight * t * (1 - t);
			pos.y += height;
			ball.anchoredPosition = pos;
			elapsed += Time.deltaTime;
			yield return null;
		}
		ball.anchoredPosition = end;

		// Yoyo bounce sequence using DOTween (UI: move up then down, repeat)
		float currentBounceHeight = bounceHeight;
		for (int i = 0; i < bounceCount; i++)
		{
			Vector2 up = end + Vector2.up * currentBounceHeight;
			yield return ball.DOAnchorPos(up, bounceDuration / 2).SetEase(Ease.OutQuad).WaitForCompletion();
			yield return ball.DOAnchorPos(end, bounceDuration / 2).SetEase(Ease.InQuad).WaitForCompletion();
			currentBounceHeight *= bounceShrink; // Reduce bounce height for next
		}

		// Destroy ball after the final bounce
		if(GameManager.Instance.EndGameVar == false)
		paneltoEnable.SetActive(false);
		Destroy(ball.gameObject);
	}
}
