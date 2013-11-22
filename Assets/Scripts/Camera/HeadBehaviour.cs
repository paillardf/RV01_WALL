using UnityEngine;
using System.Collections;

public class HeadBehaviour : MonoBehaviour {

	public Camera camera;
	
	void LateUpdate () {
		Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.red);
		Debug.DrawRay(transform.position, transform.forward, Color.green);
		transform.localRotation = camera.transform.localRotation;
		// Fixes the head orientation
		transform.localRotation = new Quaternion(-transform.localRotation.y, transform.localRotation.z, -transform.localRotation.x, transform.localRotation.w);
	}
}
