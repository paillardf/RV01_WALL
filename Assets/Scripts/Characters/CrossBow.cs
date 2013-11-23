using UnityEngine;
using System.Collections;

public class CrossBow : MonoBehaviour {
	public GameObject arrowPrefab;
	public int power = 200;
	public void fire(){
		GameObject arrow;
		if (!Network.isClient && !Network.isServer){
			arrow= (GameObject)Instantiate(arrowPrefab, transform.position+transform.forward, transform.rotation);
		}else{
			arrow = (GameObject)Network.Instantiate(arrowPrefab, transform.position+transform.forward, transform.rotation,0);
		}
		arrow.rigidbody.AddForce(arrow.transform.forward*power*10);

	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.A)){
			fire ();
		}
	}
}
