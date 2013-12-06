﻿using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class CameraController : MonoBehaviour {
	public Camera playerCamera;
	public float  rotationSpeed = 5;

	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine) {
			float angle;
			if(MiddleVR.VRDeviceMgr != null) {
				angle = MiddleVR.VRDeviceMgr.GetAxis("RazerHydra.JoyStick1.Axis").GetValue(0);
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
