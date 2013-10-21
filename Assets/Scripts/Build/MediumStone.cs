using UnityEngine;
using System.Collections;

public class MediumStone : AbstractObject {
	
	override
	 public void rotate(){
		
		}
	
	RaycastHit hit = new RaycastHit();
	
	override
	public bool hasBedRock(){
		
		
		if (Physics.Raycast (transform.position, Vector3.down,out hit, 2, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
	   		if(Vector3.Distance(closestPoint, hit.point)<0.03){
	   			return true;
	   			
	   		}
		}
		return false;
	}
	
}
