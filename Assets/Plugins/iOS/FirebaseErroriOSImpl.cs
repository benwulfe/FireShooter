using UnityEngine;
using System.Collections;

#if UNITY_IOS
public class FirebaseErroriOSImpl : IFirebaseError {
	#region IFirebaseError implementation
	public int GetCode ()
	{
		throw new System.NotImplementedException ();
	}
	public string GetMessage ()
	{
		throw new System.NotImplementedException ();
	}
	public string GetDetails ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
	
}
#endif