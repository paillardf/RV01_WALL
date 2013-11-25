using UnityEngine;
using System.Collections;

public class LeftHand : MonoBehaviour {

	public float sensitivityY  = 0.02f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine){
			transform.Translate(0, Input.GetAxis("Mouse Y") * sensitivityY, 0);
		}
	}
}
