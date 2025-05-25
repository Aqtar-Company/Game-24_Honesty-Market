using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ControlDragAndDrop — A UI drag-and-drop manager using structs to map each draggable to its target drop zone.
/// Compatible with mouse and touch (WebGL mobile & desktop).
/// Attach this script to a Canvas GameObject and populate entries in the Inspector.
/// Drop zones only need a RectTransform (and optionally a Graphic). No EventTrigger required on zones.
/// Use EventTrigger on draggables to call BeginDrag(), Drag(), and EndDrag().
/// When all items are placed, disables them and the drop zones, shows a completion image,
/// hides the items panel, then after a delay re-enables car movement via the referenced CarController.
/// </summary>
public class ControlDragAndDrop : MonoBehaviour
{
	[System.Serializable]
	public struct DraggableEntry
	{
		[Tooltip("The UI element to drag.")]
		public RectTransform draggable;
		[Tooltip("The target zone for this draggable.")]
		public RectTransform dropZone;

		[HideInInspector] public Vector2 originalPosition;
		[HideInInspector] public Transform originalParent;
	}

	[Header("Setup Entries")]
	public DraggableEntry[] entries;
	public bool finished = false; // Set to true when all items are correctly plac	
	[Header("Settings")]
	[Tooltip("Distance threshold (in world units) for snapping to zone.")]
	public float snapThreshold = 50f;

	[Header("Completion UI")]
	[Tooltip("UI Panel containing all draggable items (will be hidden on completion)")]
	public GameObject itemsPanel;
	[Tooltip("Image to display once all items are correctly placed.")]
	public Image completionImage;
	public GameObject bridge;

	[Tooltip("Delay in seconds before car can move again.")]
	public float resumeDelay = 2f;

	private Canvas parentCanvas;
	private RectTransform currentDrag;
	private int currentIndex = -1;
	private Vector2 pointerOffset;

	void Awake()
	{
		parentCanvas = GetComponentInParent<Canvas>();
		if (parentCanvas == null)
			Debug.LogError("[ControlDragAndDrop] Must be under a Canvas.");

		// Cache original states
		for (int i = 0; i < entries.Length; i++)
		{
			var entry = entries[i];
			entry.originalPosition = entry.draggable.anchoredPosition;
			entry.originalParent = entry.draggable.parent;
			entries[i] = entry;
		}

		// Hide completion image initially
		if (completionImage != null)
			completionImage.gameObject.SetActive(false);
	}

	/// <summary> Call this on PointerDown via EventTrigger, passing the entry index. </summary>
	public void BeginDrag(int index)
	{
		if (index < 0 || index >= entries.Length) return;
		currentIndex = index;
		currentDrag = entries[index].draggable;
		currentDrag.SetAsLastSibling();

		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentCanvas.transform as RectTransform,
			Input.mousePosition,
			parentCanvas.worldCamera,
			out Vector2 localMousePos);
		pointerOffset = currentDrag.anchoredPosition - localMousePos;
	}

	/// <summary> Call this on Drag via EventTrigger. </summary>
	public void Drag()
	{
		if (currentDrag == null) return;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentCanvas.transform as RectTransform,
			Input.mousePosition,
			parentCanvas.worldCamera,
			out Vector2 localMousePos);
		currentDrag.anchoredPosition = localMousePos + pointerOffset;
	}

	/// <summary> Call this on PointerUp via EventTrigger. </summary>
	public void EndDrag()
	{
		if (currentDrag == null || currentIndex < 0) return;

		var entry = entries[currentIndex];
		float dist = Vector3.Distance(
			currentDrag.position,
			entry.dropZone.position);

		if (dist <= snapThreshold)
		{
			currentDrag.SetParent(entry.dropZone, false);
			currentDrag.localPosition = Vector3.zero;
			currentDrag.localScale = Vector3.one; // <-- This line ensures scale is 1
		}
		else
		{
			currentDrag.SetParent(entry.originalParent, false);
			currentDrag.anchoredPosition = entry.originalPosition;
		}

		currentDrag = null;
		currentIndex = -1;
		if (!finished)
		{
			CheckCompletion();
		}else
		{
			CheckCompletionFinished();
		}
		
	}


	private void CheckCompletion()
	{
		foreach (var entry in entries)
		{
			if (entry.draggable.parent != entry.dropZone)
				return;
		}
			

		foreach (var entry in entries)
		{
			entry.draggable.gameObject.SetActive(false);
			entry.dropZone.gameObject.SetActive(false);
		}

		if (completionImage != null)
			completionImage.gameObject.SetActive(true);
		bridge.gameObject.SetActive(true);

		if (CarController.Instance != null)
			StartCoroutine(ResumeCarAfterDelay());
	}

	private IEnumerator ResumeCarAfterDelay()
	{
		yield return new WaitForSeconds(resumeDelay);
		CarController.Instance?.ResumeMovement();
		itemsPanel.SetActive(false);
	}
	private void CheckCompletionFinished()
	{
		
			if (entries[0].draggable.parent != entries[0].dropZone)
				return;


		foreach (var entry in entries)
		{
			entry.draggable.gameObject.SetActive(false);
			entry.dropZone.gameObject.SetActive(false);
		}

		if (completionImage != null)
			completionImage.gameObject.SetActive(true);
		bridge.gameObject.SetActive(true);

		if (CarController.Instance != null)
			StartCoroutine(ResumeCarAfterDelay());
	}

}
