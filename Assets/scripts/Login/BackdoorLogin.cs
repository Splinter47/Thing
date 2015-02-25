using UnityEngine;
using System.Collections;

public class BackdoorLogin : MonoBehaviour {
	
	public GameObject backDoor;

	double accelerometerUpdateInterval = 1.0 / 60.0;
	// The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
	double lowPassKernelWidthInSeconds = 1.0;
	// This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! ;)
	double shakeDetectionThreshold = 2.0;

	private float lowPassFilterFactor; 
	private Vector3 lowPassValue = Vector3.zero;
	private Vector3 acceleration;
	private Vector3 deltaAcceleration;

	private bool shaking = false;
	private bool fourFingers = false;

	void Start(){
		lowPassFilterFactor = (float)(accelerometerUpdateInterval / lowPassKernelWidthInSeconds);
		shakeDetectionThreshold *= shakeDetectionThreshold;	
		lowPassValue = Input.acceleration;
	}


	// Update is called once per frame
	void Update(){
	
		acceleration = Input.acceleration;	
		lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
		deltaAcceleration = acceleration - lowPassValue;

		if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold){
			// Perform "shaking actions" here
			Debug.Log("Shake event detected at time "+Time.time);
			shaking = true;
		}else{
			shaking = false;
		}
	
	
		// 4 finger measure
		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++;
		}

		if (fingerCount > 3){
			print("User has " + fingerCount + " finger(s) touching the screen");
			fourFingers = true;
		}else{
			fourFingers = false;
		}


		if(fourFingers && shaking){
			backDoor.SetActive(true);
		}
	}

	public void backDoorLogin(){
		Application.LoadLevel(1);
	}
	
}
