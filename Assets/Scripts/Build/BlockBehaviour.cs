﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class BlockBehaviour : MonoBehaviour {
	private int nbCheckGravity  = 0;
	private int moving = 0;
	
	
	private AbstractObject objScript;
	
	AnimationCurve curveFragmentStrength = AnimationCurve.EaseInOut(0, 0.05f, 1,0.0f);
    AnimationCurve curveFragmentArePow = AnimationCurve.EaseInOut(0, 4, 1, 10);   
	
	
//	AnimationCurve position_curve = AnimationCurve.Linear(0, 0.0001f, 40, 1);
	
	 
    float delta  =0;
	Queue<Vector3> lastPositions = new Queue<Vector3>();
	bool rotationCheck = false;
    public Material material;
	Material matFragmentum;
	
	IList<Collision> colliders = new List<Collision>();
	
	void Awake (){
		addToPathFinder();
	}
	
	// Use this for initialization
	void Start () {
		objScript = gameObject.GetComponent<AbstractObject>();
		matFragmentum = GetComponent<Fragmentum>().GetMaterial();
		
	}
	
	void addToPathFinder(){
		Bounds b =	gameObject.collider.bounds;
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs (guo);
		print ("add");
	}
	
	void removeFromPathFinder(){
		Bounds b = gameObject.collider.bounds;
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs (guo,0.0f);
		print ("remove");
	}
	
	
	void Update () {
		if(delta>=0){
			
			if(delta<1){
				delta += Time.deltaTime;
		        matFragmentum.SetFloat("_FragTexStrength", curveFragmentStrength.Evaluate(delta));
		        matFragmentum.SetFloat("_FragPow", curveFragmentArePow.Evaluate(delta));
			}else{
				Destroy(GetComponent<Fragmentum>());
				Destroy(gameObject.renderer.material);
				gameObject.renderer.material = material;
				
				
				matFragmentum=null;
			}
		
		}
		
		
		
		if(transform.rigidbody.isKinematic){
			if(nbCheckGravity >0){
				nbCheckGravity --;
				
					Vector3 r = transform.localRotation.eulerAngles;
					//deltaPos<=0&&(((r.x+5)%90-5>5||(r.y+5)%90-5>5||(r.z+5)%90-5>5)||
					if(!objScript.hasBedRock()||(rotationCheck&&((r.x+5)%90-5>5||(r.y+5)%90-5>5||(r.z+5)%90-5>5))){
						
						nbCheckGravity=0;
						rotationCheck = false;
						//gameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
						//transform.rigidbody.mass = weight;
						transform.rigidbody.isKinematic=false;
						transform.rigidbody.WakeUp();
						addToPathFinder();
						notifyNeighbourg();
					}
				if(nbCheckGravity==0){
					rotationCheck=false;
				}
				
			}
		}else{
			/*if(nbCheckGravity>0){
				notifyNeighbourg();
				nbCheckGravity = 0;
			}*/
			
			
			//print (deplacement.magnitude+" : "+position_curve.Evaluate(deltaPos));
			if(!isMoving()){
				
				moving ++;
				if(moving == 4){
					lastPositions.Clear();
					moving = 0;
					
					nbCheckGravity=0;
					transform.rigidbody.isKinematic=true;
					removeFromPathFinder();
					//Destroy (transform.rigidbody);
				}
				
				
			}else{
				moveNotify();
				moving =0;
			}
		}
	}
	
	void destroy(){
		notifyNeighbourg();
		moveNotify();
		removeFromPathFinder();
		Destroy (gameObject.collider);
		Destroy (gameObject);
		
	}

	void checkGravity(){
		nbCheckGravity =  50;
	}
	
	void checkGravityTotal(){
		nbCheckGravity =  50;
		rotationCheck=true;
	}
	
	
	bool isMoving(){
		
		if(!transform.rigidbody.isKinematic){
			lastPositions.Enqueue(transform.position);
			if(lastPositions.Count>10){
				Vector3 deplacement = lastPositions.Dequeue()-transform.position;
	
				if(deplacement.magnitude<0.03f){
					return false;
				}
			}
			return transform.rigidbody.velocity.magnitude>0.001f;
		}
		
		return false;
	}
	
	void moveNotify(){
		for(int i = 0; i<colliders.Count; i++){
		
				Collision c = colliders[i];
				if(!c.Equals(null)){
					if(!(c.collider==null)){
						print("ok");
						c.collider.SendMessage("checkGravityTotal");
					}
				}
			
			
		}
		colliders.Clear();
	//Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2, Constants.MaskBlock);
		//for (var i = 0; i < hitColliders.Length; i++) {
			//transform
			//Vector3 closestPointOnObject  = hitColliders[i].ClosestPointOnBounds(transform.position);
			//Vector3 closestPoint  = transform.collider.ClosestPointOnBounds(hitColliders[i].transform.position);
			//if(Vector3.Distance(closestPoint, closestPointOnObject)<0.05){
			//	print (hitColliders[i].transform.tag);
			//	hitColliders[i].SendMessage("checkGravityTotal");
			//}
		//}
					
	}
	
	void notifyNeighbourg(){
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3, Constants.MaskBlock);
		for (var i = 0; i < hitColliders.Length; i++) {
			
			hitColliders[i].SendMessage("checkGravity");
				
			
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if(collision.transform.gameObject.layer==gameObject.layer){
			colliders.Add(collision);
		}
		
		if(transform.rigidbody.isKinematic&&collision.relativeVelocity.sqrMagnitude>20){
			transform.rigidbody.isKinematic=false;
			transform.rigidbody.WakeUp();
			removeFromPathFinder();
			transform.rigidbody.velocity=collision.relativeVelocity/2;
			
		}		 
	}
	
	void OnCollisionExit(Collision collision) {
		colliders.Remove(collision);

	}
	
	
	
}
