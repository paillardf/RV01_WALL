using UnityEngine;
using System.Collections;

public class HeadBehaviour : MonoBehaviour {

	public Camera playerCamera;
	
	void LateUpdate () {
		transform.localRotation = playerCamera.transform.localRotation;
		// Fixes the head orientation
		transform.localRotation = new Quaternion(-transform.localRotation.y, transform.localRotation.z, -transform.localRotation.x, transform.localRotation.w);
	}
}
