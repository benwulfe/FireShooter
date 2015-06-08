using UnityEngine;
using System.Collections;

public class FirebaseDebugLog  {
	private static IFirebase debugLog;

	public static void Initialize(string url) {
		if (debugLog == null) {
			debugLog = Firebase.CreateNew(url);
			Application.logMessageReceived += (condition, stackTrace, type) => 	{
				if (type == LogType.Exception)
				{
					Log(condition + stackTrace);
				}
			};
		}
	}

	public static void Log(string message) {
		if (debugLog != null) {
			debugLog.Push().SetValue(message);
		}
	}
}
