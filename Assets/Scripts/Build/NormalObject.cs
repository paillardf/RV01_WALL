using UnityEngine;
using System.Collections;

public class NormalObject : AbstractObject {
	
	override
	 public void rotate(){
		
	}
	
	RaycastHit hit = new RaycastHit();
	
	override
	public bool hasBedRock(){
		Vector3 up = new Vector3(0,0.3f,0);
		
		if (Physics.Raycast (transform.position+up, Vector3.down,out hit, 2, Constants.MaskCollision)) {
   			Debug.DrawLine (transform.position+up, hit.point, Color.cyan);
   			
			Vector3 closestPoint  = collider.ClosestPointOnBounds(hit.point);
			print (Vector3.Distance(closestPoint, hit.point));
	   		if(Vector3.Distance(closestPoint, hit.point)<0.06){
	   			return true;
	   			
	   		}
		}
		return false;
	}
	
}
