using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

#if UNITY_IOS
internal class DataSnapshotiOSImpl : IDataSnapshot {
	IntPtr dataSnapshotReference;

	public DataSnapshotiOSImpl(IntPtr dataSnapshotReference) {
		this.dataSnapshotReference = dataSnapshotReference;
	}

	private IntPtr GetSnapShotReference() {
		return dataSnapshotReference;
	}

	~DataSnapshotiOSImpl() {
		IntPtr nativeSnapshot = GetSnapShotReference ();
		if (nativeSnapshot != IntPtr.Zero) {
			_DataSnapshotRelease(nativeSnapshot);
		}
	}

	#region Imports
	[DllImport ("__Internal")]
	private static extern float _DataSnapshotGetFloatValue (IntPtr datasnapshot);
	
	[DllImport ("__Internal")]
	private static extern string _DataSnapshotGetStringValue (IntPtr datasnapshot);

	[DllImport ("__Internal")]
	private static extern IntPtr _DataSnapshotGetChild (IntPtr datasnapshot, string path);
	
	[DllImport ("__Internal")]
	private static extern IntPtr _DataSnapshotHasChild (IntPtr datasnapshot, string path);
	
	[DllImport ("__Internal")]
	private static extern IntPtr _DataSnapshotExists (IntPtr datasnapshot);
	
	[DllImport ("__Internal")]
	private static extern string _DataSnapshotGetKey (IntPtr datasnapshot);
	
	[DllImport ("__Internal")]
	private static extern string _DataSnapshotGetPriority (IntPtr datasnapshot);
	
	[DllImport ("__Internal")]
	private static extern IntPtr _DataSnapshotGetRef (IntPtr datasnapshot);

	[DllImport ("__Internal")]
	private static extern string _DataSnapshotGetDictionary (IntPtr datasnapshot);

	[DllImport ("__Internal")]
	private static extern void _DataSnapshotRelease (IntPtr datasnapshot);
	#endregion

	#region IDataSnapshot implementation
	public IDataSnapshot Child (string path)
	{
		return new DataSnapshotiOSImpl (_DataSnapshotGetChild (GetSnapShotReference (), path));
	}

	public bool Exists ()
	{
		return _DataSnapshotExists (GetSnapShotReference ()) != IntPtr.Zero;
	}

	public string GetKey ()
	{
		return _DataSnapshotGetKey (GetSnapShotReference ());
	}

	public object GetPriority ()
	{
		return _DataSnapshotGetPriority (GetSnapShotReference ());
	}

	public IFirebase GetRef ()
	{
		return new FirebaseiOSImpl (_DataSnapshotGetRef (GetSnapShotReference ()));
	}

	public float GetFloatValue ()
	{
		return _DataSnapshotGetFloatValue (GetSnapShotReference ());
	}

	public string GetStringValue ()
	{
		return _DataSnapshotGetStringValue (GetSnapShotReference ());
	}

	public Dictionary<string, object> GetDictionaryValue() {
		string dictionaryJSON = _DataSnapshotGetDictionary (GetSnapShotReference ());
		if (String.IsNullOrEmpty (dictionaryJSON)) {
			return null;
		}
		return MiniJSON.Json.Deserialize(dictionaryJSON) as Dictionary<string,object>;
	}

	public bool HasChild (string path)
	{
		return _DataSnapshotHasChild (GetSnapShotReference (), path) != IntPtr.Zero;
	}
	
	#endregion
}
#endif