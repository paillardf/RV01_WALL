﻿using UnityEngine;
using System.Collections;

public class RightHand : MonoBehaviour {


	public Transform leftHand;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine){
			transform.LookAt(leftHand);
		}

	}
}