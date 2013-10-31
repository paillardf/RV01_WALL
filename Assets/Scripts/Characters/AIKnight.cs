using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;


[RequireComponent(typeof(Seeker))]

public class AIKnight : AIPathFinder
{
    
	 private Animator anim;                  // Reference to the Animator.
    private HashIDs hash;                   // Reference to the HashIDs script.
    private AnimatorSetup animSetup;        // An instance of the AnimatorSetup helper class.
	
	private bool isClimbing =false;
    
    protected override void  Awake ()
    {
		base.Awake();
        anim = GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
        animSetup = new AnimatorSetup(anim, hash);
        
		
		
   }
    
	 void OnAnimatorMove ()
    {
		if (navController != null) {
			navController.SimpleMove (GetFeetPosition(),anim.deltaPosition/ Time.deltaTime);
		} else if (controller != null) {
			controller.SimpleMove (anim.deltaPosition/ Time.deltaTime);
		} else if (rigid != null) {
			rigid.AddForce (anim.deltaPosition/ Time.deltaTime);
		} else {
			transform.Translate (anim.deltaPosition, Space.World);
		}
        
        // The gameobject's rotation is driven by the animation's rotation.
       transform.rotation = anim.rootRotation;
    }
	
    public override void Update ()
    {
		
		float angle = 0;
		float speed = 0;
		bool climb=false;
		if(isClimbing){
			
		}else if (canMove) {  
			Vector3 desiredVelocity = CalculateVelocity (GetFeetPosition());
			if(desiredVelocity!=Vector3.zero){
				
				desiredVelocity.y = 0;
				Debug.DrawRay(tr.position+transform.up, desiredVelocity, Color.red);
				angle = FindAngle(transform.forward, targetDirection, transform.up);
				print (angle);
				if(angle>Mathf.PI/2||angle<-Mathf.PI/2){
					speed = 0;
				}else{
					speed = desiredVelocity.magnitude;
				}
				if(Mathf.Abs(angle) < deadZone)
		        {
		                // ... set the direction to be along the desired direction and set the angle to be zero.
		            transform.LookAt(transform.position + targetDirection);
		            angle = 0f;
		        }
			}
		}
		
		
			
		
        // Call the Setup function of the helper class with the given parameters.
        animSetup.Setup(speed, angle,climb);
		
		
		
		
        
    }
    
   
    
 
   
}