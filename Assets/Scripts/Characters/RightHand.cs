using UnityEngine;
using System.Collections;

public class RightHand : SixenseObjectController {


	public Transform leftHand;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine){
			SixenseInput.Controller controller = SixenseInput.GetController(SixenseHands.RIGHT);
			if(controller != null) {
				UpdateObject(controller);
			}
			transform.LookAt(leftHand);
		}

	}
}
