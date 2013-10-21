using UnityEngine;
using System.Collections;

public class Plank : AbstractObject {
	override
	 public void rotate(){
	
		switch(rotationState){
			case 0:
				currentRotation = Quaternion.Euler(0,90,0);	
			break;
			case 1:
				currentRotation = Quaternion.Euler(0,180,0);	
			break;
			case 2:
				currentRotation = Quaternion.Euler(0,-90,0);	
			break;
			case 3:
				currentRotation = Quaternion.Euler(0,0,0);	
			break;
		}
		rotationState++;
		rotationState=rotationState%4;
		transform.rotation = currentRotation;	
	}
	
	RaycastHit hit = new RaycastHit();
	
	override
	public bool hasBedRock(){
		
		return checkBedRock();
	}
	
	
	public bool checkBedRock(){
		Vector3 up = new Vector3(0 , 0.03f , 0);
		Vector3 pos = up + transform.position;
		bool leftBedRock = false;
		bool rightBedRock = false;
		
		Vector3 middle = pos-gameObject.transform.right*2.5f;
		Vector3 left = pos-gameObject.transform.right*1.5f;
		Vector3 maxleft = pos-gameObject.transform.right*0.5f;
		Vector3 right = pos-gameObject.transform.right*3.5f;
		Vector3 maxright = pos-gameObject.transform.right*4.5f;
		
		if (Physics.Raycast (middle, Vector3.down,out hit, 2, Constants.MaskCollision)) {
			Debug.DrawLine (middle, hit.point, Color.cyan);
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
			if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			return true;
	   		}
		}
		
		if (Physics.Raycast (left, Vector3.down,out hit, 2, Constants.MaskCollision)) {
			Debug.DrawLine (left, hit.point, Color.cyan);
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
			if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			leftBedRock = true;
	   		}
		}
		
		if(leftBedRock == false&&Physics.Raycast (maxleft, Vector3.down,out hit, 2, Constants.MaskCollision)) {
			Debug.DrawLine (maxleft, hit.point, Color.cyan);
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
			if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			leftBedRock = true;
	   		}
		}
		
		if (Physics.Raycast (right, Vector3.down,out hit, 2, Constants.MaskCollision)) {
			Debug.DrawLine (right, hit.point, Color.cyan);
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
			if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			rightBedRock = true;
	   		}
		}
		
		if(rightBedRock == false&&Physics.Raycast (maxright, Vector3.down,out hit, 2, Constants.MaskCollision)) {
			Debug.DrawLine (maxright, hit.point, Color.cyan);
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
			if(Vector3.Distance(closestPoint, hit.point)<0.05){
	   			rightBedRock = true;
	   		}
		}
		return leftBedRock&&rightBedRock;
	}
	
}
