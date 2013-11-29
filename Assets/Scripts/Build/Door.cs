using UnityEngine;
using System.Collections;

public class Door : AbstractObject {
	override
	public void rotate(){
	
		switch(rotationState){
			case 0:
				currentRotation = Quaternion.Euler(0,90,0);	
			break;
			case 1:
				currentRotation = Quaternion.Euler(0,0,0);	
			break;
		}
		rotationState++;
		rotationState=rotationState%2;
		transform.rotation = currentRotation;	
	}
	
	RaycastHit hit = new RaycastHit();
	
	override
	public bool hasBedRock(){
		
		return checkBedRock();
	}
	
	
	public bool checkBedRock(){
		Vector3 up = new Vector3(0 , 0.03f , 0);
		if (Physics.Raycast (transform.position+up, Vector3.down,out hit, 3, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position+up, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
	   		if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			return true;
	   			
	   		}
		}
		return false;
	}
}
