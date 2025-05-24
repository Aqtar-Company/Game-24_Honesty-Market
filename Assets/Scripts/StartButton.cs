using UnityEngine;

public class StartButton : MonoBehaviour
{
	public GameObject buttonStart;
	public GameObject panelLevel;

	public void UnActiveButton()
	{
		buttonStart.SetActive(false);
		panelLevel.SetActive(true);
	}
}
