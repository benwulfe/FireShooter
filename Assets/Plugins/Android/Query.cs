using UnityEngine;
using System.Collections;
using System;

public class Query  {
	AndroidJavaObject queryRef;
	EventHandler<ValueChangedEventArgs> valueUpdatedEvent;
	ValueEventListener valueupdateListener;

	protected Query(AndroidJavaObject reference) {
		queryRef = reference;
	}

	protected AndroidJavaObject GetJavaObject() {
		return queryRef;
	}

	public event EventHandler<ValueChangedEventArgs> ValueUpdated {
		add {
			valueUpdatedEvent += value;
			if (valueupdateListener == null) {
				valueupdateListener = new ValueEventListener(this);
				queryRef.Call<AndroidJavaObject>("addValueEventListener", valueupdateListener);
			}
		}
		remove {
			valueUpdatedEvent -= value;
			if (valueUpdatedEvent == null) {
				queryRef.Call<AndroidJavaObject>("removeEventListener", valueupdateListener);
				valueupdateListener = null;
			}
		}
	}

	public event EventHandler<EventArgs> HandlerCanceled;

	void OnValueUpdated(FirebaseError error, DataSnapshot snapshot) {
		EventHandler<ValueChangedEventArgs> handler = valueUpdatedEvent;
		if (handler != null)
		{
			handler(this, new ValueChangedEventArgs() { DataSnapshot = snapshot, Error = error });
		}
	}

	class ValueEventListener : AndroidJavaProxy {
		Query parent;

		public ValueEventListener(Query parent)
			:base("com.firebase.client.ValueEventListener")
		{
			this.parent = parent;
		}

		void onDataChange(AndroidJavaObject dataSnapshot) {
			parent.OnValueUpdated (null, new DataSnapshot (dataSnapshot));
		}

		void onCancelled(AndroidJavaObject error) {
			parent.OnValueUpdated (new FirebaseError(error), null);
		}
	}
}

public class ValueChangedEventArgs : EventArgs
{
	public FirebaseError Error { get; set; }
	public DataSnapshot DataSnapshot { get; set; }
}

