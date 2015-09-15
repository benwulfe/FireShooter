﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using AOT;

#if UNITY_IOS
internal class FirebaseiOSImpl : QueryiOSImpl, IFirebase {
	static object auth_sync = new object ();
	static AuthCallInstanceEntry activeCallEntry;
	static OnAuthSuccessHandler authSuccessHandler = new OnAuthSuccessHandler(onAuthSuccess);
	static OnAuthCancelHandler authFailureHandler = new OnAuthCancelHandler(onAuthCancel);

	public delegate void OnAuthSuccessHandler(long reference, String token, String uid, long expiration);
	public delegate void OnAuthCancelHandler(long reference, int code, String message, String details);
	
	public FirebaseiOSImpl (IntPtr nativeReference)
		:base(nativeReference)
	{
	}

	public static FirebaseiOSImpl CreateNewFirebaseiOSImpl(IntPtr nativeReference) {
		return QueryiOSImpl.GetOrCreateCachedInstance (nativeReference, () => {
			return new FirebaseiOSImpl(nativeReference);
		});
	}
	
	private static IntPtr CreateNativeFirebase (string path)
	{
		return _FirebaseNew (path);
	}

	~FirebaseiOSImpl() {
		IntPtr nativeReference = GetiOSObject ();
		if (nativeReference != IntPtr.Zero) {
			_FirebaseRelease (nativeReference);
		}
	}

	#region Imports
	[DllImport ("__Internal")]
	private static extern IntPtr _FirebaseNew (string path);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseRelease(IntPtr firebase);

	[DllImport ("__Internal")]
	private static extern IntPtr _FirebaseChild (IntPtr firebase, string path);
	
	[DllImport ("__Internal")]
	private static extern IntPtr _FirebaseParent (IntPtr firebase);
	
	[DllImport ("__Internal")]
	private static extern IntPtr _FirebaseRoot (IntPtr firebase);
	
	[DllImport ("__Internal")]
	private static extern string _FirebaseGetKey(IntPtr firebase);
	
	[DllImport ("__Internal")]
	private static extern IntPtr _FirebasePush (IntPtr firebase);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseSetString (IntPtr firebase, string value);

	[DllImport ("__Internal")]
	private static extern void _FirebaseSetJson (IntPtr firebase, string json);

	[DllImport ("__Internal")]
	private static extern void _FirebaseSetFloat (IntPtr firebase, float value);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseSetPriority (IntPtr firebase, string value);

	[DllImport ("__Internal")]
	private static extern void _FirebaseAuthWithCustomToken (IntPtr firebase, string token,
	                                   OnAuthSuccessHandler success, OnAuthCancelHandler cancel,
	                                                         long callback);
	[DllImport ("__Internal")]
	private static extern void _FirebaseAuthAnonymously (IntPtr firebase,
	                               OnAuthSuccessHandler success, OnAuthCancelHandler cancel,
	                                                     long callback);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseAuthWithPassword (IntPtr firebase, string email,
	                                string password,
	                                OnAuthSuccessHandler success, OnAuthCancelHandler cancel,
	                                                      long callback);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseAuthWithOAuthToken (IntPtr firebase, string provider,
	                                  string token,
	                                  OnAuthSuccessHandler success, OnAuthCancelHandler cancel,
	                                  long callback);

	[DllImport ("__Internal")]
	private static extern string _FirebaseGetAuthToken(IntPtr firebase);

	[DllImport ("__Internal")]
	private static extern string _FirebaseGetAuthUid(IntPtr firebase);

	[DllImport ("__Internal")]
	private static extern long _FirebaseGetAuthExpiration (IntPtr firebase);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseUnAuth( IntPtr firebase);

	#endregion

	#region IFirebase implementation
	public IFirebase Child (string name)
	{
		return CreateNewFirebaseiOSImpl (_FirebaseChild (GetiOSObject(), name));
	}

	public IFirebase Parent
	{
		get {
			return CreateNewFirebaseiOSImpl (_FirebaseParent (GetiOSObject ()));
		}
	}

	public IFirebase Root
	{
		get {
			return CreateNewFirebaseiOSImpl (_FirebaseRoot (GetiOSObject ()));
		}
	}

	public string Key
	{
		get {
			return _FirebaseGetKey (GetiOSObject ());
		}
	}

	public IFirebase Push ()
	{
		return CreateNewFirebaseiOSImpl( _FirebasePush(GetiOSObject ()));
	}

	public void SetValue (string value)
	{
		_FirebaseSetString (GetiOSObject (), value);
	}

	public void SetValue (IDictionary<string, object> value) {
		string jsonString = MiniJSON.Json.Serialize (value);
		_FirebaseSetJson (GetiOSObject (), jsonString);
	}

	public void SetValue (float value)
	{
		_FirebaseSetFloat (GetiOSObject (), value);
	}

	public void SetPriority (string priority)
	{
		_FirebaseSetPriority (GetiOSObject (), priority);
	}
	
	public void AuthWithCustomToken (string token, Action<AuthData> onSuccess, Action<FirebaseError> onError)
	{
		lock (auth_sync) {
			long newnumber = activeCallEntry != null ? (activeCallEntry.Instance + 1) % long.MaxValue - 1 : 0;
			activeCallEntry = new AuthCallInstanceEntry() { Instance = newnumber, OnSuccess = onSuccess, OnError = onError};
			_FirebaseAuthWithCustomToken(GetiOSObject(), token, authSuccessHandler, authFailureHandler, activeCallEntry.Instance);
		}
	}
	
	public void AuthAnonymously (Action<AuthData> onSuccess, Action<FirebaseError> onError)
	{
		lock (auth_sync) {
			long newnumber = activeCallEntry != null ? (activeCallEntry.Instance + 1) % long.MaxValue - 1 : 0;
			activeCallEntry = new AuthCallInstanceEntry() { Instance = newnumber, OnSuccess = onSuccess, OnError = onError};
			_FirebaseAuthAnonymously(GetiOSObject(), authSuccessHandler, authFailureHandler, activeCallEntry.Instance);
		}
	}
	
	public void AuthWithPassword (string email, string password, Action<AuthData> onSuccess, Action<FirebaseError> onError)
	{
		lock (auth_sync) {
			long newnumber = activeCallEntry != null ? (activeCallEntry.Instance + 1) % long.MaxValue - 1 : 0;
			activeCallEntry = new AuthCallInstanceEntry() { Instance = newnumber, OnSuccess = onSuccess, OnError = onError};
			_FirebaseAuthWithPassword(GetiOSObject(), email, password, authSuccessHandler, authFailureHandler, activeCallEntry.Instance);
		}
	}
	
	public void AuthWithOAuthToken (string provider, string token, Action<AuthData> onSuccess, Action<FirebaseError> onError)
	{
		lock (auth_sync) {
			long newnumber = activeCallEntry != null ? (activeCallEntry.Instance + 1) % long.MaxValue - 1 : 0;
			activeCallEntry = new AuthCallInstanceEntry() { Instance = newnumber, OnSuccess = onSuccess, OnError = onError};
			_FirebaseAuthWithOAuthToken(GetiOSObject(), provider, token, authSuccessHandler, authFailureHandler, activeCallEntry.Instance);
		}
	}

	public void UnAuth ()
	{
		_FirebaseUnAuth (GetiOSObject ());
	}

	public AuthData Auth {
		get {
			return new AuthData(_FirebaseGetAuthToken(GetiOSObject()), _FirebaseGetAuthUid(GetiOSObject()),
			                    _FirebaseGetAuthExpiration(GetiOSObject()));
		}
	}

	#endregion

	[MonoPInvokeCallbackAttribute(typeof(OnAuthSuccessHandler))]
	static void onAuthSuccess(long reference, String token, String uid, long expiration) {
		Action<AuthData> callback = null;
		lock (auth_sync) {
			if (activeCallEntry != null && activeCallEntry.Instance == reference) {
				callback = activeCallEntry.OnSuccess;
				activeCallEntry = null;
			}
		}
		if (callback != null) {
			callback(new AuthData(token, uid, expiration));
		}
	}
	
	[MonoPInvokeCallbackAttribute(typeof(OnAuthCancelHandler))]
	static void onAuthCancel(long reference, int code, String message, String details) {
		Action<FirebaseError> callback = null;
		lock (auth_sync) {
			if (activeCallEntry != null && activeCallEntry.Instance == reference) {
				callback = activeCallEntry.OnError;
				activeCallEntry = null;
			}
		}
		if (callback != null) {
			callback(new FirebaseError(code, message, details));
		}
	}


	public class Factory : IFirebaseFactory
	{
		#region IFirebaseFactory implementation
		public IFirebase TryCreate (string path)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return CreateNewFirebaseiOSImpl (CreateNativeFirebase(path));
			}
			return null;
		}
		#endregion
	}

	class AuthCallInstanceEntry {
		public long Instance { get; set; }
		public Action<AuthData> OnSuccess { get; set; }
		public Action<FirebaseError> OnError { get; set;}
	}
}
#endif