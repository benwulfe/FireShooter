using UnityEngine;
using System.Collections;
using System;

#if UNITY_ANDROID
internal class QueryAndroidImpl : IQuery {
	AndroidJavaObject queryRef;
	EventHandler<ChangedEventArgs> valueUpdatedEvent, childAddedEvent;
	ValueEventListener valueupdateListener;
	ChildEventListener childAddedListener;

	protected QueryAndroidImpl(AndroidJavaObject nativeReference) {
		queryRef = nativeReference;
	}
	
	protected AndroidJavaObject GetJavaObject() {
		return queryRef;
	}

	public event EventHandler<ChangedEventArgs> ValueUpdated {
		add {
			valueUpdatedEvent += value;
			
			if (valueupdateListener == null) {
				valueupdateListener = new ValueEventListener(this);
				GetJavaObject().Call<AndroidJavaObject>("addValueEventListener", valueupdateListener);
			}
			
		}
		remove {
			valueUpdatedEvent -= value;
			
			if (valueUpdatedEvent == null) {
				
				GetJavaObject().Call<AndroidJavaObject>("removeEventListener", valueupdateListener);
				valueupdateListener = null;
			}
			
		}
	}

	public event EventHandler<ChangedEventArgs> ChildAdded {
		add {
			childAddedEvent += value;
			
			if (childAddedListener == null) {
				childAddedListener = new ChildEventListener(this);
				GetJavaObject().Call<AndroidJavaObject>("addChildEventListener", childAddedListener);
			}
			
		}
		remove {
			childAddedEvent -= value;
			
			if (childAddedEvent == null) {
				GetJavaObject().Call<AndroidJavaObject>("removeEventListener", childAddedListener);
				childAddedListener = null;
			}
		}
	}

	
	public event EventHandler<ErrorEventArgs> Error;
	
	void OnValueUpdated(DataSnapshotAndroidImpl snapshot) {
		EventHandler<ChangedEventArgs> handler = valueUpdatedEvent;
		if (handler != null)
		{
			handler(this, new ChangedEventArgs() { DataSnapshot = snapshot });
		}
	}

	void OnChildAdded(DataSnapshotAndroidImpl snapshot) {
		EventHandler<ChangedEventArgs> handler = childAddedEvent;
		if (handler != null)
		{
			handler(this, new ChangedEventArgs() { DataSnapshot = snapshot });
		}
	}
	
	class ValueEventListener : AndroidJavaProxy {
		QueryAndroidImpl parent;
		
		public ValueEventListener(QueryAndroidImpl parent)
			:base("com.firebase.client.ValueEventListener")
		{
			this.parent = parent;
		}
		
		void onDataChange(AndroidJavaObject dataSnapshot) {
			parent.OnValueUpdated (new DataSnapshotAndroidImpl (dataSnapshot));
		}
		
		void onCancelled(AndroidJavaObject error) {
//			parent.OnValueUpdated (new FirebaseErrorAndroidImpl(error), null);
		}
	}

	class ChildEventListener : AndroidJavaProxy {
		QueryAndroidImpl parent;
		
		public ChildEventListener(QueryAndroidImpl parent)
			:base("com.firebase.client.ChildEventListener")
		{
			this.parent = parent;
		}

		void onCancelled(AndroidJavaObject error) {
		}

		void onChildAdded(AndroidJavaObject snapshot, string previousChildName) {
			parent.OnChildAdded(new DataSnapshotAndroidImpl(snapshot));
		}

		void onChildChanged(AndroidJavaObject snapshot, string previousChildName) {
		}

		void onChildMoved(AndroidJavaObject snapshot, string previousChildName) {
		}

		void onChildRemoved(AndroidJavaObject snapshot) {
		}
	}
}
#endif


