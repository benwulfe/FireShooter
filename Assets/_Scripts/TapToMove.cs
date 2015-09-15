using UnityEngine;
using System.Collections;
using System;

public class TapToMove : MonoBehaviour {
	//flag to check if the user has tapped / clicked.
	//Set to true on click. Reset to false on reaching destination
	private bool flag = false;
	//destination point
	private Vector3 endPoint;
	//alter this to change the speed of the movement of player / gameobject
	public float duration = 50.0f;

	public bool enableTap = true;

	private const string XKey = "TapToMove_X";
	private const string YKey = "TapToMove_Y";
	private const string ZKey = "TapToMove_Z";

	//vertical position of the gameobject
	private float yAxis;

	private Func<Vector3, Vector3> transformFunc;

	private FirebaseController firebaseController;
	
	void Start(){
		//save the y axis value of gameobject
		yAxis = gameObject.transform.position.y;
		firebaseController = GetComponent<FirebaseController> ();
	}

	public void SetTransform(Func<Vector3, Vector3> transform) {
		this.transformFunc = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
		//check if the screen is touched / clicked
		if(enableTap && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0))))
		{
			//declare a variable of RaycastHit struct
			RaycastHit hit;
			//Create a Ray on the tapped / clicked position
			Ray ray;
			//for unity editor
			#if UNITY_EDITOR
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//for touch device
			#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			#endif

			Vector3 targetEndPoint;
			//Check if the ray hits any collider
			if(Physics.Raycast(ray,out hit))
			{
				//save the click / tap position
				targetEndPoint = hit.point;
				//as we do not want to change the y axis value based on touch position, reset it to original y axis value
				targetEndPoint.y = yAxis;

				firebaseController.BeginSetValue(XKey, (double)targetEndPoint.x);
				firebaseController.BeginSetValue(YKey, (double)targetEndPoint.y);
				firebaseController.BeginSetValue(ZKey, (double)targetEndPoint.z);
			}
		}
		double x, y, z;

		if (firebaseController.TryGetValue (XKey, out x) 
		    && firebaseController.TryGetValue (YKey, out y) 
		    && firebaseController.TryGetValue (ZKey, out z)) {
			Vector3 newEndPoint = new Vector3((float)x,(float)y,(float)z);
			if (transformFunc != null) {
				newEndPoint = transformFunc(newEndPoint);
			}
			if (!Mathf.Approximately(newEndPoint.magnitude, endPoint.magnitude)){
				//set a flag to indicate to move the gameobject
				flag = true;
				endPoint = newEndPoint;
			}
		}
		
		//check if the flag for movement is true and the current gameobject position is not same as the clicked / tapped position
		if(flag && !Mathf.Approximately(gameObject.transform.position.magnitude, endPoint.magnitude)){ //&& !(V3Equal(transform.position, endPoint))){
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, endPoint, 
			                                             50*Time.deltaTime/(duration*(Vector3.Distance(gameObject.transform.position, endPoint))));
		}
		//set the movement indicator flag to false if the endPoint and current gameobject position are equal
		else if(flag && Mathf.Approximately(gameObject.transform.position.magnitude, endPoint.magnitude)) {
			flag = false;
		}
	}
}
