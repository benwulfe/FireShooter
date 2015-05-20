using UnityEngine;
using System.Collections;
using System;


public class Firebase : Query  {
	static bool initialized = false;

    public Firebase(string path)
		: base(new AndroidJavaObject ("com.firebase.client.Firebase", path))
	{
	}

	public Firebase(AndroidJavaObject reference)
		:base(reference)
	{
	}

	public static void SetAndroidContext() {
		if (!initialized) {
			try {
				Debug.Log ("Firebase: attempting to initialize");
				AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"); 
				AndroidJavaObject currentContext = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
				AndroidJavaClass firebaseClass = new AndroidJavaClass ("com.firebase.client.Firebase"); 
				firebaseClass.CallStatic ("setAndroidContext", currentContext);
				initialized = true;
				Debug.Log ("Firebase: set Context!");
			} catch (Exception e) {
				Debug.Log (e.ToString ());
			}
		}
	}
	
	public Firebase Child(string name) {
		return new Firebase(GetJavaObject().Call<AndroidJavaObject>("child", name));
	}
	
	public Firebase GetParent() {
		return new Firebase(GetJavaObject().Call<AndroidJavaObject>("getParent"));
	}

	public Firebase GetRoot() {
		return new Firebase(GetJavaObject().Call<AndroidJavaObject>("getRoot"));
	}
	
	public string GetKey() {
		return GetJavaObject().Call<string>("getKey");
	}

	public Firebase Push() {
		return new Firebase(GetJavaObject().Call<AndroidJavaObject>("push"));
	}

	public void SetValue(string value) {
		GetJavaObject().Call ("setValue", value);
	}

	public void SetValue(string value, string priority, Action<FirebaseError, Firebase> listener) {
		GetJavaObject().Call ("setValue", value, priority, new CompletionListener(listener));
	}

	public void SetValue(float value) {
		GetJavaObject().Call ("setValue", new AndroidJavaObject ("java.lang.Float", value));
	}

	public void SetValue(float value, string priority, Action<FirebaseError, Firebase> listener) {
		GetJavaObject().Call ("setValue", new AndroidJavaObject ("java.lang.Float", value), priority, new CompletionListener(listener));
	}

	public void SetPriority(string priority) {
		GetJavaObject().Call ("setPriority", priority);
	}

	public void SetPriority(string priority, Action<FirebaseError,Firebase> listener) {
		GetJavaObject().Call ("setPriority", priority, new CompletionListener(listener));
	}

	class CompletionListener : AndroidJavaProxy {
		private Action<FirebaseError,Firebase> completionListener;

		public CompletionListener(Action<FirebaseError,Firebase> listener)
			:base("com.firebase.client.Firebase$CompletionListener")
		{
			completionListener = listener;
		}

		void onComplete(AndroidJavaObject error, AndroidJavaObject reference) {
			if (completionListener != null) {
				completionListener (error != null ? new FirebaseError (error) : null, new Firebase (reference));
			}
		} 
	}
}
