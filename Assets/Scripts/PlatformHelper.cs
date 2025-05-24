using System.Runtime.InteropServices;
using UnityEngine;

public static class PlatformHelper
{
#if UNITY_WEBGL && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern string GetUserAgent();
#endif

	private static string userAgent;

	public static bool IsMobile()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		if (string.IsNullOrEmpty(userAgent))
		{
			userAgent = GetUserAgent();
		}
		string lower = userAgent.ToLower();
		return lower.Contains("iphone") || lower.Contains("android") ||
		       lower.Contains("ipad") || lower.Contains("mobile");
#else
		// Default to PC in editor
		return false;
#endif
	}
}
