using UnityEngine;
using System.Collections;
using System;

public interface IFirebaseError {
	int GetCode();
	string GetMessage ();
	string GetDetails();
}
