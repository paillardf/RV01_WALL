using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	
	public Transform warrior;
	public Transform lord;
	public Vector3 spoon;
	public Camera menuCamera;
	
	
	private bool hasplayer=true;
	
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
			}
	            
		}
		
	}
	
	public void selectPlayer(){
		hasplayer = false;
	}
	
	

	
	
}
