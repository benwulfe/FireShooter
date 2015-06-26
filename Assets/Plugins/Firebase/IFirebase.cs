using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public interface IFirebase : IQuery
{
	IFirebase Child (string name);
	IFirebase GetParent ();
	IFirebase GetRoot ();
	string GetKey ();
	IFirebase Push ();
	void SetValue (string value);
	void SetValue (float value);
	void SetValue (IDictionary<string, object> value);
	void SetPriority (string priority);
}

