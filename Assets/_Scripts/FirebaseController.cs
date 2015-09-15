using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FirebaseController : MonoBehaviour {
	private const string ServerTimeAtChange = "ServerTimeAtChange";

	private IFirebase firebaseObject, dataRef, timeSync;
	private bool initialized = false;
	private Queue<RPCItem> invokeQueue = new Queue<RPCItem>();
	private object sync = new object ();
	private Dictionary<string, IFirebase> firebases = new Dictionary<string, IFirebase> ();
	private volatile Dictionary<string, object> incomingDataMap = null;
	private long timeOffset = long.MinValue;
	private static readonly DateTime Epoch = new DateTime (1970, 1, 1);

	private Dictionary<string, object> currentDataMap = new Dictionary<string, object >();
	private Dictionary<string, object> dirtyDataMap = new Dictionary<string, object >();
	
	public string path;
	public bool isNPC;

	struct RPCItem {
		public Action<string> Method;
		public string Value;
	}

	void Start() {
		Initialize ();
	}

	public void SetPath(string path) {
		this.path = path;
		Initialize ();
	}

	public void RemoteCall(string objectName, string value) {
		if (firebaseObject != null) {
			IFirebase child;
			lock(sync) {
				if (!firebases.TryGetValue(objectName, out child)) {
					child = firebaseObject.Child(objectName); 
					firebases.Add(objectName, child);
				}
			}
			child.Push().SetValue(value);
		}
	}

	public void HandleCall(string objectName, Action<string> callback) {
		if (firebaseObject != null) {
			IFirebase child;
			lock(sync) {
				if (!firebases.TryGetValue(objectName, out child)) {
					child = firebaseObject.Child(objectName); 
					firebases.Add(objectName, child);
				}
			}
			Debug.Log ("hooking childadded for " + child.Key);
			child.ChildAdded += (object sender, ChangedEventArgs e) => {
				lock(sync) {
					invokeQueue.Enqueue(new RPCItem() { Method = callback, Value = e.DataSnapshot.StringValue});
				}
			};
		}
	}

	void Initialize() {
		if (!initialized && !string.IsNullOrEmpty (path)) {
			initialized = true;

			firebaseObject = Firebase.CreateNew (path);
			if (firebaseObject != null) {
				FirebaseDebugLog.Initialize(path + "/DebugLog");
				dataRef = firebaseObject.Child ("DataSync");

				if (isNPC) {
					dataRef.ValueUpdated += (object sender, ChangedEventArgs e) => {
						incomingDataMap = e.DataSnapshot.DictionaryValue;
					};
				}
			}

			Uri pathUri = new Uri(path);
			string offsetPath = pathUri.GetLeftPart(System.UriPartial.Authority) + @"/.info/serverTimeOffset";
			timeSync = Firebase.CreateNew(offsetPath);
			if (timeSync != null) {
				timeSync.ValueUpdated += (object sender, ChangedEventArgs e) => {
					timeOffset = (long)e.DataSnapshot.FloatValue;
				};
			}
		}
	}

	public object GetValue(string key) {
		return currentDataMap [key];
	}

	public bool TryGetValue<T>(string key, out T value) {
		object objValue = null;
		if (currentDataMap.TryGetValue (key, out objValue)) { 
		    if (typeof(T).IsAssignableFrom (objValue.GetType ())) {
				value = (T)objValue;
				return true;
			}
			else if (objValue.GetType() == typeof(Int64) && typeof(T) == typeof(double)) {
				Int64 intValue = (Int64) objValue;
				objValue = (double)intValue;
				value = (T)objValue; //TODO
				return true;
			}
			else FirebaseDebugLog.Log("bad type: " + objValue.GetType().FullName);
		}
		value = default(T);
		return false;
	}

	public void BeginSetValue(string key, object value) {
		dirtyDataMap [key] = value;
	}

	private bool IsOffsetValid() {
		return timeOffset != long.MinValue;
	}

	private long GetCurrentServerEpochTime() {
		TimeSpan t = DateTime.UtcNow - Epoch;
		return (long)t.TotalMilliseconds + (IsOffsetValid() ? timeOffset : 0);
	}

	public bool GetUpdateAge(ref long estimatedAge) {
		if (currentDataMap.ContainsKey (ServerTimeAtChange) && IsOffsetValid()) {
			estimatedAge = GetCurrentServerEpochTime () - (long)currentDataMap [ServerTimeAtChange];
			return true;
		}
		return false;
	}

//	float test = 1.0f;
	void LateUpdate() {
		if (dirtyDataMap.Count > 0) {
			foreach(KeyValuePair<string, object> kvp in dirtyDataMap) {
				currentDataMap[kvp.Key] = kvp.Value;
			}
			if (IsOffsetValid()) {
				currentDataMap[ServerTimeAtChange] = GetCurrentServerEpochTime();
			}
			if (dataRef != null) {
				dataRef.SetValue (currentDataMap);
			}
//			dataRef.SetValue(test++);
			dirtyDataMap.Clear();
		}
	}

	void FixedUpdate ()
	{
		RPCItem[] rpcItems = null;
		lock (sync) {
			rpcItems = new RPCItem[invokeQueue.Count];
			invokeQueue.CopyTo (rpcItems, 0);
			invokeQueue.Clear ();
		}
		if (rpcItems != null) {
			foreach (RPCItem item in rpcItems) {
				item.Method (item.Value);
			}
		}

		if (incomingDataMap != null && incomingDataMap != currentDataMap) {
			currentDataMap = incomingDataMap;
		}
	}
}
