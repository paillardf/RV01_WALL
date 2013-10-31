using UnityEngine;
using System.Collections;

public class TargetPlacement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Mouse2)&&Input.GetKey(KeyCode.LeftControl)){
			RaycastHit hit = new RaycastHit();
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	

			int mask  = Constants.MaskCollision;
	
	
	
			if (Physics.Raycast (ray, out hit ,100.0f, mask)){
				
					transform.position = hit.point;
					
				}
			}
	}
}
