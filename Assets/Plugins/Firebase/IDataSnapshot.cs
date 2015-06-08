using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IDataSnapshot  {
	IDataSnapshot Child (string path);
	bool HasChild (string path);

	bool Exists ();
	string GetKey ();
	object GetPriority ();
	IFirebase GetRef ();
	float GetFloatValue ();
	string GetStringValue ();
	Dictionary<string, object> GetDictionaryValue();
}
