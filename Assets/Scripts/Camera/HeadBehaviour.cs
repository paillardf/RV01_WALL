using UnityEngine;
using System.Collections;

public class HeadBehaviour : MonoBehaviour {

	public Camera camera;

	void LateUpdate () {
		transform.rotation = camera.transform.rotation;
	}
}
