using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum FruitType
{
	Apple,
	Mango,
	Orange,
	gwafa,
	watermelon,
	// Add more as needed!
}

[System.Serializable]
public class FruitWinRequirement
{
	public FruitType fruitType;
	public int requiredCount;
}

[System.Serializable]
public class CalculatePanel
{
	public GameObject calcPanel;
}

public class GameManager : MonoBehaviour
{
	public bool canCulcute = false;
	public GameObject shop;
	public List<CalculatePanel> calculatePanels;

	[Header("Panels & Canvases")]
	public GameObject winPanel;
	public GameObject losePanel;
	public List<Canvas> canvasesToDisable;

	[Header("Hearts (Lives)")]
	public List<Image> hearts;
	public int maxLives = 3;

	[Header("Flash Panel")]
	public Image flashPanel;
	public Color flashColor = new Color(1, 0, 0, 0.5f);
	public float flashDuration = 0.18f;
	public int flashCount = 2;

	[Header("Win Condition")]
	public List<FruitWinRequirement> winRequirements;

	private Dictionary<FruitType, int> collectedFruits = new Dictionary<FruitType, int>();
	private int lives;
	private bool gameEnded = false;

	[Header("Stars (Win Display)")]
	public List<Image> stars;

	public int indexlevel = 0;
	public bool EndGameVar = false;

	public static GameManager Instance { get; private set; }

	public GameObject panelBlockClick;
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		lives = maxLives;
		collectedFruits.Clear();
		if (flashPanel)
			flashPanel.enabled = false;
		if (winPanel != null) winPanel.SetActive(false);
		if (losePanel != null) losePanel.SetActive(false);

		foreach (var heart in hearts)
		{
			heart.enabled = true;
			heart.color = Color.white;
		}

		// Hide all calc panels at start
		ShowAllCalcPanels(false);
	}

	public void CollectFruit(int fruitTypeInt)
	{
		CollectFruit((FruitType)fruitTypeInt);
	}

	public void CollectFruit(FruitType fruitType)
	{
		if (gameEnded) return;

		FruitWinRequirement req = winRequirements.Find(r => r.fruitType == fruitType);
		int alreadyCollected = 0;
		collectedFruits.TryGetValue(fruitType, out alreadyCollected);

		if (req != null && alreadyCollected >= req.requiredCount)
		{
			WrongAnswer();
			return;
		}

		if (!collectedFruits.ContainsKey(fruitType))
			collectedFruits[fruitType] = 0;
		collectedFruits[fruitType]++;

		// Only show calculation panels after all requirements are finished
		if (canCulcute && CheckWinCondition())
		{
			StartCoroutine(ShowCalcPanelsAfterDelay(1.4f));
			return; // Do NOT win game yet, wait for calculation step
		}

		if (!canCulcute && CheckWinCondition())
			WinGame();
	}

	private IEnumerator ShowCalcPanelsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		if (shop != null)
			shop.SetActive(false);
		panelBlockClick.gameObject.SetActive(false);
		ShowAllCalcPanels(true);
	}

	private void ShowAllCalcPanels(bool show)
	{
		if (calculatePanels == null) return;
		foreach (var cp in calculatePanels)
			if (cp != null && cp.calcPanel != null)
				cp.calcPanel.SetActive(show);
	}

	private bool CheckWinCondition()
	{
		foreach (var req in winRequirements)
		{
			int count = 0;
			collectedFruits.TryGetValue(req.fruitType, out count);
			if (count < req.requiredCount)
				return false;
		}
		return true;
	}

	public void OnFinishCalculationPanel()
	{
		// Call this method from a button on your calculation panel after correct answer
		ShowAllCalcPanels(false);
		WinGame();
	}

	public void WrongAnswer()
	{
		if (gameEnded || lives <= 0) return;

		SoundManager.Instance?.PlaySound("wronganswer");

		lives--;
		StartCoroutine(FlashPanel());

		if (lives >= 0 && lives < hearts.Count)
			hearts[lives].color = Color.black;

		if (lives <= 0)
			StartCoroutine(DelayedEndGame(false));
	}

	public void WinGame()
	{
		if (gameEnded) return;
		panelBlockClick.gameObject.SetActive(true);
		ShowStarsBasedOnHearts();
		LevelManager.Instance?.IncreaseLevelOpen(indexlevel);
		StartCoroutine(DelayedEndGame(true));
	}

	private void ShowStarsBasedOnHearts()
	{
		int heartsRemaining = Mathf.Clamp(lives, 0, stars.Count);
		for (int i = 0; i < stars.Count; i++)
			stars[i].enabled = (i < heartsRemaining);
	}

	private IEnumerator DelayedEndGame(bool didWin)
	{
		EndGameVar = true;
		panelBlockClick.gameObject.SetActive(true);
		yield return new WaitForSeconds(2.3f);
		EndGame(didWin);
	}

	private void EndGame(bool didWin)
	{
		gameEnded = true;
		foreach (var canvas in canvasesToDisable)
			if (canvas != null)
				canvas.gameObject.SetActive(false);

		if (didWin && winPanel != null)
			winPanel.SetActive(true);
		else if (!didWin && losePanel != null)
			losePanel.SetActive(true);
	}

	private IEnumerator FlashPanel()
	{
		if (flashPanel == null)
			yield break;

		Color originalColor = flashPanel.color;
		flashPanel.enabled = true;

		for (int i = 0; i < flashCount; i++)
		{
			flashPanel.color = flashColor;
			yield return new WaitForSeconds(flashDuration);
			flashPanel.color = originalColor;
			yield return new WaitForSeconds(flashDuration);
		}
		flashPanel.enabled = false;
	}

	public void ResetGame()
	{
		lives = maxLives;
		gameEnded = false;
		collectedFruits.Clear();

		foreach (var heart in hearts)
		{
			heart.enabled = true;
			heart.color = Color.white;
		}
		if (winPanel != null) winPanel.SetActive(false);
		if (losePanel != null) losePanel.SetActive(false);

		foreach (var canvas in canvasesToDisable)
			if (canvas != null)
				canvas.gameObject.SetActive(true);

		ShowAllCalcPanels(false);
	}
}
