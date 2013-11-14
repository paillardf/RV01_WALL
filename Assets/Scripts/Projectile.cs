using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
	    Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
	    Vector3 syncVelocity = Vector3.zero;
		Vector3 syncAngularVelocity = Vector3.zero;
		bool syncKinematic = false;
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
			
			syncKinematic = rigidbody.isKinematic;
	        stream.Serialize(ref syncKinematic);
	    }
	    else
	    {
	        stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncAngularVelocity);
			stream.Serialize(ref syncKinematic);
	 
			rigidbody.rotation = syncRotation;
			rigidbody.position = syncPosition;
			rigidbody.isKinematic = syncKinematic;
			if(!syncKinematic){
				rigidbody.velocity = syncVelocity;
				rigidbody.angularVelocity = syncAngularVelocity;
			}
	        /*syncTime = 0f;
	        syncDelay = Time.time - lastSynchronizationTime;
	        lastSynchronizationTime = Time.time;
	 
	        syncEndPosition = syncPosition + syncVelocity * syncDelay;
	        syncStartPosition = rigidbody.position;*/
	    }
	}
}
