using UnityEngine;
using System.Runtime.InteropServices;

public class PlatformUIHandler : MonoBehaviour
{
	[Header("UI Panels or Buttons")]
	public GameObject mobileUI;
	public GameObject desktopUI;

	[DllImport("__Internal")]
	private static extern string GetUserAgent();

	void Start()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
        string ua = GetUserAgent();
#if UNITY_EDITOR
        Debug.Log("User Agent: " + ua);
#endif

        if (IsMobile(ua))
        {
            ShowMobileUI();
        }
        else
        {
            ShowDesktopUI();
        }
#else
		// Default to desktop in Editor
		ShowDesktopUI();
#endif
	}

	bool IsMobile(string ua)
	{
		string lowerUA = ua.ToLower();
		return lowerUA.Contains("iphone") || lowerUA.Contains("android") ||
			   lowerUA.Contains("ipad") || lowerUA.Contains("mobile");
	}

	void ShowMobileUI()
	{
		if (mobileUI) mobileUI.SetActive(true);
		if (desktopUI) desktopUI.SetActive(false);
#if UNITY_EDITOR
		Debug.Log("📱 Mobile UI active.");
#endif
	}

	void ShowDesktopUI()
	{
		if (mobileUI) mobileUI.SetActive(false);
		if (desktopUI) desktopUI.SetActive(true);
#if UNITY_EDITOR
		Debug.Log("🖥️ Desktop UI active.");
#endif
	}
}
