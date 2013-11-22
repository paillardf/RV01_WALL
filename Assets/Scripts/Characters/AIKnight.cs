using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;


[RequireComponent(typeof(Seeker))]

public class AIKnight : AIPathFinder
{
	public int life = 20;
	public float climbingValue = 0.5f;
	private GameObject hitTarget;
	private bool waitHitTarget = false;
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

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		Vector3 syncVelocity = Vector3.zero;
		Vector3 syncAngularVelocity = Vector3.zero;
		bool syncAttack = false;
		bool syncClimp = false;
		int syncLife = life;
		float syncSpeed = 0;
		float syncAngularSpeed = 0;

		if (stream.isWriting)
		{

			stream.Serialize(ref syncLife);

			syncPosition = rigidbody.position;
			stream.Serialize(ref syncPosition);
			
			
			syncRotation = rigidbody.rotation;
			stream.Serialize(ref syncRotation);
			
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncVelocity);
			
			syncAngularVelocity = rigidbody.angularVelocity;
			stream.Serialize(ref syncAngularVelocity);
			
			syncSpeed = anim.GetFloat(hash.speedFloat);
			stream.Serialize(ref syncSpeed);

			syncAngularSpeed = anim.GetFloat(hash.angularSpeedFloat);
			stream.Serialize(ref syncAngularSpeed);

			syncClimp =  anim.GetBool(hash.climbBool);
			stream.Serialize(ref syncClimp);

			syncAttack =  anim.GetBool(hash.attackBool);
			stream.Serialize(ref syncAttack);



		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation); 
			
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncAngularVelocity);

			stream.Serialize(ref syncSpeed);
			stream.Serialize(ref syncAngularSpeed);
			stream.Serialize(ref syncClimp);
			stream.Serialize(ref syncAttack);
			stream.Serialize(ref syncLife);

			rigidbody.rotation = syncRotation;
			rigidbody.position = syncPosition;
			rigidbody.velocity = syncVelocity;
			rigidbody.angularVelocity = syncAngularVelocity;

			animSetup.Setup(syncSpeed, syncAngularSpeed,syncClimp, syncAttack, life <=0);

			/*syncTime = 0f;
	        syncDelay = Time.time - lastSynchronizationTime;
	        lastSynchronizationTime = Time.time;
	 
	        syncEndPosition = syncPosition + syncVelocity * syncDelay;
	        syncStartPosition = rigidbody.position;*/
		}
	}
    
	void destroy(){
		
		if (!Network.isClient && !Network.isServer){
			Destroy(gameObject);
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().removeIA();
		}else{
			if(networkView.isMine){
				GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().removeIA();
				Network.Destroy(gameObject);
			}
		}
		
		
	}



	 void OnAnimatorMove ()
    {
		if (!networkView.isMine&&(Network.isClient || Network.isServer))
		{
			return;
		}

		if (navController != null) {
			navController.SimpleMove (GetFeetPosition(),anim.deltaPosition/ Time.deltaTime);
		} else if (controller != null) {
			AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
			if(currentBaseState.nameHash==	hash.deathState	){
				if(currentBaseState.normalizedTime>0.95f){
					destroy ();
				
				}
			}else if(currentBaseState.nameHash==	hash.climbState	){
				controller.Move (anim.deltaPosition/Time.fixedDeltaTime/28);
			}else{
				controller.SimpleMove (anim.deltaPosition/ Time.deltaTime);
			}

			currentBaseState = anim.GetCurrentAnimatorStateInfo(1);
			if(currentBaseState.nameHash ==	hash.attackState){
				waitHitTarget = true;
				
			}else if(waitHitTarget){
				waitHitTarget = false;
				attack();
			}


		} else if (rigid != null) {
			rigid.AddForce (anim.deltaPosition/ Time.deltaTime);
		} else {
			transform.Translate (anim.deltaPosition, Space.World);
		}
       /* controller.
		AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
		if(currentBaseState.nameHash==	hash.climbState	){
			if(currentBaseState.normalizedTime>0.95){
				anim.
				currentBaseState.normalizedTime=1;
				tr.Translate(new Vector3(0,1,0));
				isClimbing = false;
			}else{
				isClimbing = true;
			}
			//tr.Translate(new Vector3(0,climbingCurve.Evaluate(currentBaseState.normalizedTime),0));
		}else if(isClimbing){
			isClimbing = false;
			//tr.Translate(new Vector3(0,1,0));
		}*/
		
        // The gameobject's rotation is driven by the animation's rotation.
       transform.rotation = anim.rootRotation;
		
    }
	
	void FixedUpdate(){
		
	}
	
    public override void Update ()
    {
		
		float angle = 0;
		float speed = 0;
		bool climb=false;
		if(isClimbing){
			
		}else if (canMove) {  
			hitTarget =null;
			Vector3 desiredVelocity = CalculateVelocity (GetFeetPosition());
			if(desiredVelocity!=Vector3.zero){
				
				if(path == null || path.vectorPath == null || path.vectorPath.Count == 0){

					Collider[] hitColliders = Physics.OverlapSphere(transform.position+transform.up, 1f, Constants.MaskBlock);
					if(hitColliders.Length>0) {	
						hitTarget = hitColliders[0].gameObject;
					}

			
					
				}else{



					Collider[] hitColliders = Physics.OverlapSphere(transform.position+transform.up, 1f, Constants.MaskTarget);
					if(hitColliders.Length>0) {	
						hitTarget = hitColliders[0].gameObject;
					}
					hitColliders = Physics.OverlapSphere(transform.position+transform.up, 1f, Constants.MaskBlock);
					for(int i = 0; i < hitColliders.Length; i++){
						if(hitColliders[i].tag == "Target"){
							hitTarget = hitColliders[i].gameObject;
							break;
						}
						
					}

					if(desiredVelocity.y>climbingValue){

						RaycastHit hit = new RaycastHit();
						int mask  = Constants.MaskCollision;
						Ray ray = new Ray(tr.position+tr.up*controller.height*3/4, tr.forward);
						Debug.DrawRay(tr.position+tr.up*controller.height*3/4, tr.forward, Color.blue);
						if (!Physics.Raycast (ray, out hit ,0.4F, mask)){
							ray = new Ray(tr.position+tr.up*controller.height/4, tr.forward);
							Debug.DrawRay(tr.position+tr.up*controller.height/4, tr.forward, Color.blue);
							if (Physics.Raycast (ray,out hit ,0.38F, mask)){
								climb=true;
							}
						}
							
						
					}
				}
				desiredVelocity.y = 0;
				Debug.DrawRay(tr.position+transform.up, desiredVelocity, Color.red);
				angle = FindAngle(transform.forward, targetDirection, transform.up);
					
				if(angle>Mathf.PI/2||angle<-Mathf.PI/2||hitTarget!=null){
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
        animSetup.Setup(speed, angle,climb, hitTarget!=null, life<=0);

		
		
		
        
    }


    
   private void attack(){
		
		if(hitTarget!=null){
			if (!Network.isClient && !Network.isServer){
				hitTarget.SendMessage("hitReveived" , 5);
			}else{
				object[] args = new object[1];
				args[0]=5;
				hitTarget.networkView.RPC("hitReveived", RPCMode.Server, args);
			}
		}
			
		
	}

	[RPC]
	public void hitReveived(int value){
		life -= value;
	}
    
 
   
}