using UnityEngine;
using System.Collections;
using System;

#if UNITY_ANDROID
internal class FirebaseErrorAndroidImpl : IFirebaseError {
	object firebaseErrorRef;
	
	public FirebaseErrorAndroidImpl(object nativeReference) {
		if (nativeReference == null) {
			throw new System.ArgumentNullException ("reference");
		}
		firebaseErrorRef = nativeReference;
	}
	
	
	private AndroidJavaObject GetJavaObject() {
		return (AndroidJavaObject)firebaseErrorRef;
	}
	
	
	public int GetCode() {
		
		return GetJavaObject().Call<int> ("getCode");
		
	}
	
	public string GetMessage() {
		
		return GetJavaObject().Call<string> ("getMessage");
		
	}
	
	public string GetDetails() {
		
		return GetJavaObject().Call<string> ("getDetails");
		
	}
	
	public override string ToString() {
		return "FirebaseError: " + GetMessage();
	}

}
#endif
