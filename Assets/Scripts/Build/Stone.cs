using UnityEngine;
using System.Collections;

public class Stone : AbstractObject {
	
	override
	 public void rotate(){
		switch(rotationState){
			case 0:
				currentRotation = Quaternion.Euler(0,90,0);	
			break;
			case 1:
				currentRotation = Quaternion.Euler(0,0,0);	
				//currentRotation = Quaternion.Euler(0,0,90);	
			break;
			//case 2:
			//	currentRotation = Quaternion.Euler(0,0,0);	
			//break;
		}
		rotationState++;
		rotationState=rotationState%2;
		transform.rotation = currentRotation;	
	}
	
	RaycastHit hit = new RaycastHit();
	
	override
	public bool hasBedRock(){
		
		
		if (Physics.Raycast (transform.position, Vector3.down,out hit, 2, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
	   		if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			return true;
	   			
	   		}
		}
		return false;
	}
	
}
