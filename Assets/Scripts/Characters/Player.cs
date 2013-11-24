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
		transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		Vector3 syncVelocity = Vector3.zero;
		Vector3 syncAngularVelocity = Vector3.zero;
		int syncLife = life;
		float syncSpeed = 0;
		float syncDirection = 0;
		
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
			
			syncDirection = anim.GetFloat(hash.directionFloat);
			stream.Serialize(ref syncDirection);
			
			
			
		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation); 
			
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncAngularVelocity);
			
			stream.Serialize(ref syncSpeed);
			stream.Serialize(ref syncDirection);
			stream.Serialize(ref syncLife);
			
			rigidbody.rotation = syncRotation;
			rigidbody.position = syncPosition;
			rigidbody.velocity = syncVelocity;
			rigidbody.angularVelocity = syncAngularVelocity;

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
}
