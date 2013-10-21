using UnityEngine;
using System.Collections;

public abstract class AbstractObject : MonoBehaviour  {
	
	protected int rotationState = 0;
	public Quaternion currentRotation;
	
	void Start(){
		currentRotation = transform.rotation;
	}
	
	public abstract void rotate();
	public abstract bool hasBedRock();
	
	
}
