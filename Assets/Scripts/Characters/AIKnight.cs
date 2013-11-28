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


	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity ;


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

			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);
			
			
			syncRotation = transform.rotation;
			stream.Serialize(ref syncRotation);
			
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

			stream.Serialize(ref syncLife);
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation); 


			stream.Serialize(ref syncSpeed);
			stream.Serialize(ref syncAngularSpeed);
			stream.Serialize(ref syncClimp);
			stream.Serialize(ref syncAttack);

			life = syncLife;
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;
			
			syncStartRotation = transform.rotation;
			syncEndRotation = syncRotation;


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

		if(Network.isClient){
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
			transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
			return;
		}

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



					Collider[] hitColliders = Physics.OverlapSphere(transform.position+transform.up, 1.3f, Constants.MaskTarget);
					if(hitColliders.Length>0) {	
						hitTarget = hitColliders[0].gameObject;
					}
					hitColliders = Physics.OverlapSphere(transform.position+transform.up, 1.3f, Constants.MaskBlock);
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
		
		if(hitTarget!=null&&life>0){
			if (!Network.isClient){
				hitTarget.SendMessage("hitReceived" , 5);
			}else{
				object[] args = new object[1];
				args[0]=5;
				hitTarget.networkView.RPC("hitReceived", RPCMode.All, args);
			}
		}
			
		
	}

	[RPC]
	public void hitReceived(int value){
		life -= value;
	}
    
 
   
}