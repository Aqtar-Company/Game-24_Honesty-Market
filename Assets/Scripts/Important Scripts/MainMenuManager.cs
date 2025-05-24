using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public static MainMenuManager Instance { get; private set; }

	public Button[] levelButtons;

	private bool hasLoadedBefore = false;

	void Awake()
	{
		
	}

	void Start()
	{
		UnlockLevels();
	}

	void OnDestroy()
	{
	}



	public void LoadScene(string sceneName)
	{
		if (sceneName == "MainMenu" && hasLoadedBefore)
		{
			SceneManager.LoadScene(sceneName);
			return;
		}
	}

	public void UnlockLevels()
	{
		//if (GameDataManager.Instance == null)
		{
#if UNITY_EDITOR
			Debug.LogError("❌ GameDataManager is missing!");
#endif
			return;
		}

		//int unlockedLevel = GameDataManager.Instance.GetUnlockedLevel();
		for (int i = 0; i < levelButtons.Length; i++)
		{
			//levelButtons[i].interactable = (i + 1 <= unlockedLevel);
		}
	}
}
