using UnityEngine;
using System.Collections;

public class Wood : AbstractObject {
	
	override
	 public void rotate(){
		switch(rotationState){
			case 0:
				currentRotation = Quaternion.Euler(0,90,0);	
			break;
			case 1:
				currentRotation = Quaternion.Euler(90,0,0);	
			break;
			case 2:
				currentRotation = Quaternion.Euler(0,0,90);	
				
			break;
		}
		rotationState++;
		rotationState=rotationState%3;
		transform.rotation = currentRotation;	
	}
	
	RaycastHit hit = new RaycastHit();
	
	override
	public bool hasBedRock(){
		
		return checkBedRock()||checkNeighbours();
	}
		
	public bool checkBedRock(){
		if (Physics.Raycast (transform.position, Vector3.down,out hit, 2, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
	   		if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			return true;
	   			
	   		}
		}
		return false;
	}
	
	public bool checkNeighbours(){
		
		if(transform.rotation == Quaternion.Euler(90,0,0))
			return false;
		
		Vector3 horizontal = transform.forward;
		
		if (Physics.Raycast (transform.position, horizontal,out hit, 2, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
	   		if(hit.transform.tag=="wood"&&Vector3.Distance(closestPoint, hit.point)<0.05){
				
					Wood script =  hit.transform.GetComponent<Wood>();
					if(script.checkBedRock()){
						return true;	
					}
				
	   		}
		}
		
		horizontal = -horizontal;
		
		if (Physics.Raycast (transform.position, horizontal,out hit, 2, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
	   		if(hit.transform.tag=="wood"&&Vector3.Distance(closestPoint, hit.point)<0.05){
				
					Wood script =  hit.transform.GetComponent<Wood>();
					if(script.checkBedRock()){
						return true;	
					}
				
	   		}
		}
		
		return false;
	}
	
}
