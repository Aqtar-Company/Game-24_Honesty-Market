using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
	[Tooltip("The build index of the level represented by this button.")]
	public int levelIndex = 0;

	[Header("UI References")]
	public GameObject lockIcon;    // shows when locked
	public GameObject unlockIcon;  // shows when unlocked
	public Image starImage;      // shows once level is passed (not just unlocked)

	private Button btn;

	void Start()
	{
		btn = GetComponent<Button>();
		if (LevelManager.Instance == null)
		{
#if UNITY_EDITOR
			Debug.LogWarning("No LevelManager instance found in scene.");
#endif
			return;
		}

		bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelIndex);
		btn.interactable = isUnlocked;

		// lock vs unlock icons
		if (lockIcon != null) lockIcon.SetActive(!isUnlocked);
		if (unlockIcon != null) unlockIcon.SetActive(isUnlocked);

		// show the star if this level has actually been passed
		bool isPassed = levelIndex < LevelManager.Instance.HighestUnlockedLevel;
		if (starImage != null)
			starImage.enabled = isPassed;
	}

	public void OnButtonClicked()
	{
		if (LevelManager.Instance == null) return;

		if (LevelManager.Instance.IsLevelUnlocked(levelIndex))
			SceneManager.LoadScene(levelIndex);
#if UNITY_EDITOR
		else
			Debug.Log($"Level {levelIndex} is locked.");
#endif
	}
}
