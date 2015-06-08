using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

#if UNITY_IOS
internal class FirebaseiOSImpl : QueryiOSImpl, IFirebase {

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
	private static extern void _FirebaseSetFloat (IntPtr firebase, float value);
	
	[DllImport ("__Internal")]
	private static extern void _FirebaseSetPriority (IntPtr firebase, string value);
	#endregion

	#region IFirebase implementation
	public IFirebase Child (string name)
	{
		return CreateNewFirebaseiOSImpl (_FirebaseChild (GetiOSObject(), name));
	}

	public IFirebase GetParent ()
	{
		return CreateNewFirebaseiOSImpl (_FirebaseParent (GetiOSObject ()));
	}

	public IFirebase GetRoot ()
	{
		return CreateNewFirebaseiOSImpl (_FirebaseRoot (GetiOSObject ()));
	}

	public string GetKey ()
	{
		return _FirebaseGetKey (GetiOSObject ());
	}

	public IFirebase Push ()
	{
		return CreateNewFirebaseiOSImpl( _FirebasePush(GetiOSObject ()));
	}

	public void SetValue (string value)
	{
		_FirebaseSetString (GetiOSObject (), value);
	}

	public void SetValue (float value)
	{
		_FirebaseSetFloat (GetiOSObject (), value);
	}

	public void SetPriority (string priority)
	{
		_FirebaseSetPriority (GetiOSObject (), priority);
	}

	#endregion

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
	
}
#endif