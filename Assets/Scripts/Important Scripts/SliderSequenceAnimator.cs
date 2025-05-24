using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class SliderSequenceAnimator : MonoBehaviour
{
	[Header("Slider Settings")]
	public GameObject sliderObject;
	public Slider slider;
	[Range(0f, 1f)] public float targetSliderValue = 1f;
	[Tooltip("Duration for the slider fill (seconds)")]
	public float sliderDuration = 1f;

	[Header("Panel Fade Settings")]
	public List<Image> panelImage;
	[Tooltip("Duration for panel fade (seconds)")]
	public float fadeDuration = 1f;

	[Header("Crash Object Settings")]
	public List<CrashObject> crashObjects = new();
	[Tooltip("Distance to offset crash objects at start (units)")]
	public float crashOffsetDistance = 2000f;

	[Header("Start Button")]
	public Button startButton;                   // reference to your Button component
	public Image startButtonImage;               // the Image on that button

	[Header("Background Images")]
	[Tooltip("Images to fade out once loading finishes")]
	public List<Image> backgroundsToFadeOut;
	[Tooltip("Images to fade in alongside the Start button")]
	public List<Image> backgroundsToFadeIn;

	private void Start()
	{
		// initialize loading
		slider.value = 0f;
		sliderObject?.SetActive(true);

		// panels start transparent
		SetAlpha(panelImage, 0f);

		// backgrounds: out = 1, in = 0
		SetAlpha(backgroundsToFadeOut, 1f);
		SetAlpha(backgroundsToFadeIn, 0f);

		// start button image starts invisible & non-interactable
		if (startButtonImage != null)
			startButtonImage.color = startButtonImage.color.WithAlpha(0f);
		if (startButton != null)
			startButton.interactable = false;

		// position crash objects off-screen
		foreach (var c in crashObjects) c?.Setup(crashOffsetDistance);

		RunAllAnimations();
	}

	private void RunAllAnimations()
	{
		Sequence s = DOTween.Sequence();

		// 1) Fill slider
		if (slider != null)
			s.Join(DOTween
				.To(() => slider.value, x => slider.value = x, targetSliderValue, sliderDuration)
				.OnComplete(OnLoadingComplete)
			);

		// 2) panel overlays fade in
		foreach (var img in panelImage)
			s.Join(img.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine));

		// 3) crash-object animations
		foreach (var c in crashObjects)
			if (c.target != null)
				s.Join(c.AnimateCrashTween());
	}

	private void OnLoadingComplete()
	{
		// swap & fade all at once
		Sequence s = DOTween.Sequence();

		// fade old backgrounds out
		foreach (var img in backgroundsToFadeOut)
			s.Join(img.DOFade(0f, fadeDuration).SetEase(Ease.InOutSine));

		// fade new backgrounds in
		foreach (var img in backgroundsToFadeIn)
			s.Join(img.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine));

		// fade the button’s image in
		if (startButtonImage != null)
			s.Join(startButtonImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine));

		s.OnComplete(() =>
		{
			// disable the loading UI
			sliderObject?.SetActive(false);

			// make the button clickable
			if (startButton != null)
				startButton.interactable = true;
		});
	}

	// helper to set a list of Images’ alpha
	private void SetAlpha(List<Image> images, float a)
	{
		foreach (var img in images)
			if (img != null)
				img.color = img.color.WithAlpha(a);
	}


	[System.Serializable]
	public class CrashObject
	{
		public Transform target;
		public CrashDirection direction;
		public float totalDuration = 1f;
		public float delay = 0f;
		public Ease moveEase = Ease.InExpo;

		[HideInInspector] public Vector3 originalPosition;
		[HideInInspector] public Vector3 originalScale;
		private float offsetDistance;

		public void Setup(float distance)
		{
			offsetDistance = distance;
			if (target == null) return;
			originalPosition = target.position;
			originalScale = target.localScale;
			Vector3 offset = direction switch
			{
				CrashDirection.Top => Vector3.up * offsetDistance,
				CrashDirection.Down => Vector3.down * offsetDistance,
				CrashDirection.Left => Vector3.left * offsetDistance,
				CrashDirection.Right => Vector3.right * offsetDistance,
				_ => Vector3.zero
			};
			target.position = originalPosition + offset;
			target.localScale = originalScale;
		}

		public Tween AnimateCrashTween()
		{
			float moveTime = totalDuration * 0.6f;
			float squashTime = totalDuration - moveTime;

			return DOTween.Sequence()
				.AppendInterval(delay)
				.Append(target.DOMove(originalPosition, moveTime).SetEase(moveEase))
				.Append(
					DOTween.Sequence()
						.Append(target.DOScaleY(originalScale.y * 0.6f, squashTime * 0.375f).SetEase(Ease.OutQuad))
						.Join(target.DOScaleX(originalScale.x * 1.2f, squashTime * 0.375f).SetEase(Ease.OutQuad))
						.Append(target.DOScale(originalScale, squashTime * 0.625f).SetEase(Ease.OutElastic))
				);
		}
	}

	public enum CrashDirection { Top, Down, Left, Right }
}

// extension method to change alpha conveniently
public static class ColorExtensions
{
	public static Color WithAlpha(this Color c, float a) =>
		new Color(c.r, c.g, c.b, a);
}
