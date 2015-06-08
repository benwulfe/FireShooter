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
		if (Application.platform == RuntimePlatform.Android) {
			firebaseController.SetPath ("https://incandescent-torch-2575.firebaseio.com/Android/");
		} else {
			firebaseController.SetPath ("https://incandescent-torch-2575.firebaseio.com/IPhone/");
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

			foreach (Touch touch in Input.touches) {
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
					nextFire = Time.time + fireRate;
					firebaseController.RemoteCall("Shot", "null");
				}
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
