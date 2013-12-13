using UnityEngine;
using System.Collections;

public class LeftHand : SixenseObjectController {

	public float sensitivityY  = 0.02f;

	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine){
			SixenseInput.Controller controller = SixenseInput.GetController(SixenseHands.LEFT);
			if(controller != null) {
				UpdateObject(controller);
			}
			else {
				transform.Translate(0, Input.GetAxis("Mouse Y") * sensitivityY, 0);
			}
		}
	}
}
