using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Camera playerCamera;
	public float  rotationSpeed = 5;

	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine) {
			float angle;

			SixenseInput.Controller controller = SixenseInput.GetController(SixenseHands.RIGHT);
			if(controller != null) {
				angle = controller.JoystickX;
			}
			
			else {
				angle = Input.GetAxis("HorizontalCamera");
			}
			if(angle != 0) {
				gameObject.transform.Rotate(Vector3.up, angle * rotationSpeed);
			}
		}
	}
}
