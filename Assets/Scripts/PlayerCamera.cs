using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType {Follow, Scriptable};

public class PlayerCamera : MonoBehaviour {

	public GameObject cameraSubject;
	public float followBuffer = 3f;
	public CameraType cameraType = CameraType.Follow;
	
	private Vector3 positionTarget;

	private void Start() {
		if (cameraSubject == null) {
			cameraSubject = GameObject.FindGameObjectWithTag("Player");
		}
	}

	private void FixedUpdate () {
		// Default camera type "Follow", will follow the camera subject smoothly.
		// Alt camera type "Scriptable", will not automatically update the camera.
		// Scriptable can be used for manually animating the camera i.e. in a cutscene
		if (cameraType == CameraType.Follow) {
			Vector3 subjectPos = cameraSubject.transform.position;
			positionTarget = subjectPos + new Vector3(0, 0, -10);
			float dist = (transform.position - positionTarget).magnitude;

			transform.position = Vector3.MoveTowards(transform.position, positionTarget, Time.deltaTime * followBuffer * dist);
		}
	}

	public void MoveToCameraSubject() {
		transform.position = cameraSubject.transform.position + new Vector3(0, 0, -10);
	}
}
