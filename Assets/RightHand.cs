using UnityEngine;
using System.Collections;

public class RightHand : MonoBehaviour {


	public Transform leftHand;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(leftHand);
	}
}
