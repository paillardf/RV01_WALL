using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {



	void OnCollisionEnter(Collision collision) {
		if (!Network.isClient && !Network.isServer||networkView.isMine){
			if(collision.transform.gameObject.tag.Equals("Enemy")){
				if (!Network.isClient){
					collision.transform.gameObject.SendMessage("hitReceived" , 20);
				}else{
					object[] args = new object[1];
					args[0]=20;

					collision.transform.gameObject.networkView.RPC("hitReceived", RPCMode.Server, args);
				}
			}
			if (!Network.isClient && !Network.isServer){
				Destroy(gameObject);
			}else{
				Network.Destroy(gameObject);
			}
		}		 
	}

}
