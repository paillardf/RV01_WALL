using UnityEngine;
using System.Collections;

public class BoxBehaviour : MonoBehaviour {
	private bool collide = false;
	private bool support = false;
	
	public Transform block;
	private AbstractObject objScript;
	
	void Start () {
		objScript = gameObject.GetComponent<AbstractObject>();
	}

	void Update () {
		
		
		support = objScript.hasBedRock();
	
		 foreach(Material matt in renderer.materials)
	    {
	       if(isValide()){
				matt.color = new Color(0, 0, 0.9f);
			}else{
				matt.color = new Color(0.9f, 0, 0);
			}
	    }
		
	}


	void OnTriggerEnter (Collider other) {
		OnTriggerStay(other);
			
	}

	void OnTriggerStay (Collider other) {
		if(other.tag!="no-placement-collide"||other.tag==gameObject.tag){
			collide = true;
		}
			
	}
	void OnTriggerExit (Collider other) {
			collide = false;
	}

	void buildBlock(){
		if(isValide()){
			
			GameObject.Instantiate (block, transform.position, transform.rotation);
			
		}
	}

	bool isValide(){
		return !collide&&support;
	}
	void init(){
		collide = false;
		support = false;
		transform.rotation=objScript.currentRotation;
	}


}
 