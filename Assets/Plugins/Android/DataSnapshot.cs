using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataSnapshot  {
	AndroidJavaObject snapshotReference;
	
	public DataSnapshot (AndroidJavaObject reference)
	{
		snapshotReference = reference;
	}

	public DataSnapshot	Child (string path)
	{
		return new DataSnapshot(snapshotReference.Call<AndroidJavaObject>("child", path));
	}

	public bool Exists ()
	{
		return snapshotReference != null && snapshotReference.Call<bool> ("exists");
	}

	public IEnumerable GetChildren ()
	{
		return snapshotReference.Call<IEnumerable>("getChildren");
	}

	public long GetChildrenCount ()
	{
		return snapshotReference.Call<long>("getChildrenCount");
	}

	public string GetKey ()
	{
		return snapshotReference.Call<string>("getKey");
	}

	public Object GetPriority ()
	{
		return snapshotReference.Call<Object>("getPriority");
	}

	public Firebase GetRef ()
	{
		return new Firebase(snapshotReference.Call<AndroidJavaObject>("getRef"));
	}

	public float GetFloatValue ()
	{
		AndroidJavaObject javaObject = snapshotReference.Call<AndroidJavaObject>("getValue");
		return javaObject != null ? javaObject.Call<float> ("floatValue") : 0f;
	}

	public string GetStringValue ()
	{
		AndroidJavaObject javaObject = snapshotReference.Call<AndroidJavaObject>("getValue");
		return javaObject != null ? javaObject.Call<string> ("toString") : string.Empty;
	}
	
	public bool HasChild (string path)
	{
		return snapshotReference.Call<bool>("hasChild", path);
	}
						
	public bool HasChildren()
	{
		return snapshotReference.Call<bool>("hasChildren");
	}
}
