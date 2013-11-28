using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;


[RequireComponent(typeof(Seeker))]

public class AICatapult : AIPathFinder
{
	public int life = 60;
	private GameObject hitTarget;
	
	private double fireTime = 2;
    protected override void  Awake ()
    {
		base.Awake();
    }

	public GameObject ball;
	public int fireDistance = 60;
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
		if (stream.isWriting)
		{
			syncPosition = rigidbody.position;
			stream.Serialize(ref syncPosition);
			
			
			syncRotation = rigidbody.rotation;
			stream.Serialize(ref syncRotation);
			
			syncVelocity = rigidbody.velocity;
			stream.Serialize(ref syncVelocity);
			
			syncAngularVelocity = rigidbody.angularVelocity;
			stream.Serialize(ref syncAngularVelocity);

		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncAngularVelocity);
			
			rigidbody.rotation = syncRotation;
			rigidbody.position = syncPosition;

			rigidbody.velocity = syncVelocity;
			rigidbody.angularVelocity = syncAngularVelocity;
			
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



	
    public override void Update ()
    {

		
		if(Network.isClient){
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
			transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
			return;
		}

		if(life<=0){
			destroy();
		}
		if (canMove) {  
			hitTarget =null;
			Vector3 desiredVelocity = CalculateVelocity (GetFeetPosition());
			if(desiredVelocity!=Vector3.zero){
				GameObject p = GameObject.FindGameObjectWithTag("Player");
				if(p!=null&&Vector3.Distance(transform.position, p.transform.position)<fireDistance){
					hitTarget = p;
				}
				desiredVelocity.y = 0;
				Debug.DrawRay(tr.position+transform.up, desiredVelocity, Color.red);



				if(hitTarget==null){
					transform.LookAt(transform.position + targetDirection);
					//rigidbody.velocity = - desiredVelocity;
					if(rigidbody.isKinematic){
						rigidbody.isKinematic = false;
						transform.rigidbody.WakeUp();
					}
					desiredVelocity.y = rigidbody.velocity.y;
					rigidbody.velocity =  desiredVelocity;
				}else{
					transform.LookAt(hitTarget.transform.position);
					rigidbody.isKinematic = true;
					fireTime -= Time.deltaTime;
					if(fireTime<0){
						fireTime = 15;
						attack();
					}
				}

			}
		}
		


		
		
		
        
    }


    
   private void attack(){
		//xp = v0² sin 2a / g

		// rac(xp /sin2a *g) = v0
		if(hitTarget!=null){
			float vIni = Mathf.Sqrt(Vector3.Distance(transform.position, hitTarget.transform.position) *Physics.gravity.magnitude);

			GameObject newBlock;
			Quaternion rot = Quaternion.identity;
			if (!Network.isClient && !Network.isServer){
				newBlock = (GameObject)Instantiate (ball, transform.position+transform.up-transform.forward, transform.rotation);
			}else{
				newBlock = (GameObject)Network.Instantiate (ball, transform.position+transform.up-transform.forward, transform.rotation,0);
			}
			Vector3 vAngle = transform.up+transform.forward;
			vAngle=vAngle.normalized;
			newBlock.rigidbody.velocity = vAngle*vIni;


		}
			
		
	}

	[RPC]
	public void hitReceived(int value){
		life -= value;

	}
    
 
   
}