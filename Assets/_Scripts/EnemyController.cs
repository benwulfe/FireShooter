using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public GameObject shot;
	public Transform shotSpawn;
	FirebaseController firebaseController;

	private float oldx;
	private int count = 0;

	void Start() {
		firebaseController = GetComponent<FirebaseController> ();

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			firebaseController.SetPath ("https://incandescent-torch-2575.firebaseio.com/Android/");
		} else {
			firebaseController.SetPath ("https://incandescent-torch-2575.firebaseio.com/IPhone/");
		}

		firebaseController.HandleCall("Shot", (str) => {
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		});

		InvokeRepeating("RepeatingFunction", .1f, .1f);
	}

	void RepeatingFunction ()
	{
		if (gameObject.transform.position.x > oldx) {
			GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 180f, 30f);
		} else if (gameObject.transform.position.x < oldx) {
			GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 180f, -30f);
		} else if (GetComponent<Rigidbody> ().rotation.eulerAngles.z  != 0) {
			GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 180f, 0f);
		}
		count = (count + 1) % 3;
		if (count == 0) {
			oldx = gameObject.transform.position.x;
		}
	}
}
