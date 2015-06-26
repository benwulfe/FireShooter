using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_ANDROID
internal class FirebaseAndroidImpl : QueryAndroidImpl, IFirebase
{
	static bool initialized = false;
	
	public FirebaseAndroidImpl (string path)
		: base(CreateNativeFirebase(path))
	{
	}

	public FirebaseAndroidImpl (AndroidJavaObject nativeReference)
		:base(nativeReference)
	{
	}
	
	private static AndroidJavaObject CreateNativeFirebase (string path)
	{
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
		return new AndroidJavaObject ("com.firebase.client.Firebase", path);
	}
	
	public IFirebase Child (string name)
	{
		return new FirebaseAndroidImpl(GetJavaObject().Call<AndroidJavaObject>("child", name));
	}
	
	public IFirebase GetParent ()
	{
		return new FirebaseAndroidImpl(GetJavaObject().Call<AndroidJavaObject>("getParent"));
	}
	
	public IFirebase GetRoot ()
	{
		return new FirebaseAndroidImpl(GetJavaObject().Call<AndroidJavaObject>("getRoot"));
	}
	
	public string GetKey ()
	{
		return GetJavaObject().Call<string>("getKey");
	}
	
	public IFirebase Push ()
	{
		return new FirebaseAndroidImpl(GetJavaObject().Call<AndroidJavaObject>("push"));
	}
	
	public void SetValue (string value)
	{
		GetJavaObject().Call ("setValue", value);
	}
	
	public void SetValue (string value, string priority, Action<IFirebaseError, IFirebase> listener)
	{
		GetJavaObject().Call ("setValue", value, priority, new CompletionListener(listener));
	}
	
	public void SetValue (float value)
	{
		GetJavaObject().Call ("setValue", new AndroidJavaObject ("java.lang.Float", value));
	}

	public void SetValue (IDictionary<string, object> value) {
		string jsonString = MiniJSON.Json.Serialize (value);
		AndroidJavaObject jsonObject = GetObjectMapper ().Call<AndroidJavaObject> ("readValue", jsonString, GetObjectClass ());
		GetJavaObject().Call ("setValue", jsonObject);
	}
	
	public void SetValue (float value, string priority, Action<IFirebaseError, IFirebase> listener)
	{
		GetJavaObject().Call ("setValue", new AndroidJavaObject ("java.lang.Float", value), priority, new CompletionListener(listener));
	}
	
	public void SetPriority (string priority)
	{
		GetJavaObject().Call ("setPriority", priority);
	}
	
	public void SetPriority (string priority, Action<IFirebaseError,IFirebase> listener)
	{
		GetJavaObject().Call ("setPriority", priority, new CompletionListener(listener));
	}
	
	
	class CompletionListener : AndroidJavaProxy {
		private Action<IFirebaseError,IFirebase> completionListener;
		
		public CompletionListener(Action<IFirebaseError,IFirebase> listener)
			:base("com.firebase.client.Firebase$CompletionListener")
		{
			completionListener = listener;
		}
		
		void onComplete(AndroidJavaObject error, AndroidJavaObject reference) {
			if (completionListener != null) {
				completionListener (error != null ? new FirebaseErrorAndroidImpl (error) : null, new FirebaseAndroidImpl (reference));
			}
		} 
	}

	public class Factory : IFirebaseFactory
	{
		#region IFirebaseFactory implementation
		public IFirebase TryCreate (string path)
		{
			if (Application.platform == RuntimePlatform.Android) {
				return new FirebaseAndroidImpl (path);
			}
			return null;
		}
		#endregion
	}
}
#endif
