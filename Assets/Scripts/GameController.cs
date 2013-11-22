using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	
	public Transform warrior;
	public Transform lord;
	public Vector3 spoon;
	public Camera menuCamera;

	public int nbKnight = 0;
	public int nbIA = 0;
	
	private bool hasplayer=true;

	[RPC]
	public void addKnight ()
	{
		nbKnight++;
	}


	public void addIA ()
	{
		nbIA++;
	}

	public void removeIA ()
	{
		nbIA--;
	}

	
	void OnGUI(){
		
		if(!hasplayer){
			if (GUI.Button(new Rect(100, 100, 250, 100), "Light Lord")){
	            Instantiate(lord, spoon, Quaternion.identity);
				hasplayer = true;
				menuCamera.enabled=false;
			}
	 
	        if (GUI.Button(new Rect(100, 250, 250, 100), "Warrior")){
				Network.Instantiate(warrior, spoon, Quaternion.identity, 0);
				hasplayer=true;
				menuCamera.enabled=false;	
				if (!Network.isClient && !Network.isServer){
					addKnight();
				}else{
					networkView.RPC("addKnight", RPCMode.Server);
				}

			}
	            
		}
		
	}
	
	public void selectPlayer(){
		hasplayer = false;
	}
	
	
	
	
	

	
	
}
