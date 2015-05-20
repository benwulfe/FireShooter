using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Boundary boundary;
	
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	
	private float nextFire;
	private Firebase positionRefx, positionRefy, positionRefz;

	void Update ()
	{
		if (Time.time > nextFire) {

			foreach (Touch touch in Input.touches) {
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
					nextFire = Time.time + fireRate;
					Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
					GetComponent<AudioSource>().Play ();				}
			}
		}
	}
	
	void FixedUpdate ()
	{
		GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody> ().velocity.x * -tilt);

		if (positionRefx == null) {
			Firebase.SetAndroidContext ();
			Firebase firebaseObject = new Firebase ("https://incandescent-torch-2575.firebaseio.com/Player0/");
			Firebase positionRef = firebaseObject.Child ("Position");
			Firebase movementRef = firebaseObject.Child ("Movement");
					
			positionRefx = positionRef.Child ("x");
			positionRefy = positionRef.Child ("y");
			positionRefz = positionRef.Child ("z");
		}
		
		positionRefx.SetValue (GetComponent<Rigidbody> ().position.x);
		positionRefy.SetValue (GetComponent<Rigidbody> ().position.y);
		positionRefz.SetValue (GetComponent<Rigidbody> ().position.z);
	}
}
