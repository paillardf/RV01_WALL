using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int life = 100;
	public float sensitivityX = 15F;
	private HashIDs hash;
	private Animator anim;
	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!Network.isClient && !Network.isServer||networkView.isMine){
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}else if(syncStartPosition!=Vector3.zero){
			syncTime += Time.deltaTime;
			transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
			transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
		}
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
		int syncLife = life;
		float syncSpeed = 0;
		float syncDirection = 0;
		
		if (stream.isWriting)
		{
			
			stream.Serialize(ref syncLife);
			
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);
			
			
			syncRotation = transform.rotation;
			stream.Serialize(ref syncRotation);
			

			syncSpeed = anim.GetFloat(hash.speedFloat);
			stream.Serialize(ref syncSpeed);
			
			syncDirection = anim.GetFloat(hash.directionFloat);
			stream.Serialize(ref syncDirection);
			
			
			
		}
		else
		{

			stream.Serialize(ref syncLife);
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation); 
			
			stream.Serialize(ref syncSpeed);
			stream.Serialize(ref syncDirection);


			life = syncLife;
			//transform.rotation = syncRotation;
			//transform.position = syncPosition;
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;
			
			syncStartRotation = transform.rotation;
			syncEndRotation = syncRotation;


			anim.SetFloat(hash.directionFloat, syncDirection);
			anim.SetFloat(hash.speedFloat, syncSpeed);
			//animSetup.Setup(syncSpeed, syncAngularSpeed,syncClimp, syncAttack, life <=0);
			
			/*syncTime = 0f;
	        syncDelay = Time.time - lastSynchronizationTime;
	        lastSynchronizationTime = Time.time;
	 
	        syncEndPosition = syncPosition + syncVelocity * syncDelay;
	        syncStartPosition = rigidbody.position;*/
		}
	}

	[RPC]
	public void hitReceived(int value){
		life -= value;
		if(life<0){
			anim.SetBool(hash.deathBool, true);
			if (!Network.isClient || Network.isServer){
				GameObject.FindGameObjectWithTag("GameController").SendMessage("GameOver");
			}else{
				GameObject.FindGameObjectWithTag("GameController").networkView.RPC("GameOver", RPCMode.Server);
			}
		}
	}



}
