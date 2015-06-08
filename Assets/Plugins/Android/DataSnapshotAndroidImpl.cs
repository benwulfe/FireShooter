using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
internal class DataSnapshotAndroidImpl : IDataSnapshot  {
	object snapshotNativeReference;
	
	public DataSnapshotAndroidImpl (object reference)
	{
		snapshotNativeReference = reference;
	}
	
	public IDataSnapshot Child (string path)
	{
		return new DataSnapshotAndroidImpl(getJavaObject().Call<AndroidJavaObject>("child", path));
	}
	
	private AndroidJavaObject getJavaObject() {
		return (AndroidJavaObject)snapshotNativeReference;
	}

	public bool Exists ()
	{
		return snapshotNativeReference != null && getJavaObject().Call<bool> ("exists");
	}
	
	public string GetKey ()
	{
		return getJavaObject().Call<string>("getKey");
	}
	
	public object GetPriority ()
	{
		return getJavaObject().Call<object>("getPriority");
	}

	public IFirebase GetRef ()
	{
		return new FirebaseAndroidImpl(getJavaObject().Call<AndroidJavaObject>("getRef"));
	}
	
	public float GetFloatValue ()
	{
		AndroidJavaObject javaObject = getJavaObject().Call<AndroidJavaObject>("getValue");
		return javaObject != null ? javaObject.Call<float> ("floatValue") : 0f;
	}
	
	public string GetStringValue ()
	{
		AndroidJavaObject javaObject = getJavaObject().Call<AndroidJavaObject>("getValue");
		return javaObject != null ? javaObject.Call<string> ("toString") : string.Empty;
	}

	public Dictionary<string, object> GetDictionaryValue() {
		AndroidJavaObject valueObject = getJavaObject().Call<AndroidJavaObject>("getValue");
		if (valueObject == null) {
			return null;
		}
		AndroidJavaObject json = new AndroidJavaObject ("org.json.JSONObject", valueObject);
		return MiniJSON.Json.Deserialize(json.Call<string> ("toString")) as Dictionary<string,object>;
	}

	public bool HasChild (string path)
	{
		return getJavaObject().Call<bool>("hasChild", path);
	}
}
#endif
