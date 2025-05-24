using UnityEngine;

public class GoStartButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnableStart()=> LevelManager.Instance.startButton = true;
}
