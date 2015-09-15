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
	FirebaseController firebaseController;
	private float oldx;
	private int count;

	void Start() {
		firebaseController = GetComponent<FirebaseController> ();
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			firebaseController.SetPath ("https://incandescent-torch-2575.firebaseio.com/IPhone/");
		} else {
			firebaseController.SetPath ("https://incandescent-torch-2575.firebaseio.com/Android/");
		}

		firebaseController.HandleCall ("Shot", (str) => {
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource> ().Play ();				
		});

		InvokeRepeating ("RepeatingFunction", .1f, .1f);
	}

	void Update ()
	{
		if (Time.time > nextFire) {
			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
			     || (Input.GetMouseButtonDown(0))) {
				nextFire = Time.time + fireRate;
				firebaseController.RemoteCall("Shot", "null");
			}
		}
	}

	void RepeatingFunction ()
	{
		if (gameObject.transform.position.x > oldx) {
			GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 0f, -30);
		} else if (gameObject.transform.position.x < oldx) {
			GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 0f, 30f);
		} else if (GetComponent<Rigidbody> ().rotation.eulerAngles.z  != 0) {
			GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 0f, 0f);
		}
		count = (count + 1) % 5;
		if (count == 0) {
			oldx = gameObject.transform.position.x;
		}
	}
	
}
