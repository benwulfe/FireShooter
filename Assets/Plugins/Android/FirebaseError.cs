using UnityEngine;
using System.Collections;
using System;

public class FirebaseError {
	AndroidJavaObject firebaseErrorRef;

	public FirebaseError(AndroidJavaObject reference) {
		if (reference == null) {
			throw new System.ArgumentNullException ("reference");
		}
		firebaseErrorRef = reference;
	}
	
	public int GetCode() {
		return firebaseErrorRef.Call<int> ("getCode");;
	}

	public string GetMessage() {
		return firebaseErrorRef.Call<string> ("getMessage");
	}

	public string GetDetails() {
		return firebaseErrorRef.Call<string> ("getDetails");
	}

	public override string ToString() {
		return "FirebaseError: " + GetMessage();
	}
}
