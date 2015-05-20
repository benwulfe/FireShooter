using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public Boundary boundary = new Boundary { xMax = 6, xMin = -6, zMin = 9, zMax = 14 } ;
	public float tilt;

	private volatile float positionx, positiony, positionz;

	void Start() {
		Firebase.SetAndroidContext ();
		Firebase firebaseObject = new Firebase ("https://incandescent-torch-2575.firebaseio.com/Player0/");
		Firebase positionRef = firebaseObject.Child ("Position");
				
		positionRef.Child ("x").ValueUpdated += (object sender, ValueChangedEventArgs e) => positionx = e.DataSnapshot.GetFloatValue();
		positionRef.Child ("y").ValueUpdated += (object sender, ValueChangedEventArgs e) => positiony = e.DataSnapshot.GetFloatValue();
		positionRef.Child ("z").ValueUpdated += (object sender, ValueChangedEventArgs e) => positionz = e.DataSnapshot.GetFloatValue();
	}

	void FixedUpdate ()
	{
		GetComponent<Rigidbody> ().position = new Vector3 
			(
				Mathf.Clamp (positionx, boundary.xMin, boundary.xMax), 
				0.0f, 
				Mathf.Clamp (5f - positionz + 5, boundary.zMin, boundary.zMax)
		);
	}
}
