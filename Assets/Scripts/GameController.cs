﻿using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	
	public Transform warrior;
	public Transform lord;
	public Vector3 spoon;
	public Camera menuCamera;

	public int nbKnight = 0;
	public int nbIA = 0;
	
	private bool hasplayer=true;
	private bool menu = false;
	private bool gameOver = false;
	private bool isKnight;
	public double score = 0;

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

	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			menu = !menu;
			Screen.showCursor = menu || !isKnight;
			Screen.lockCursor = !menu && isKnight;
		}
	}
	
	void OnGUI(){


		
		if(menu){
			if (GUI.Button(new Rect(300,30,200,60), "Restart game"))
				
			{
				
				Network.Disconnect();
				hasplayer = false;
				menu = false;
				Application.LoadLevel (Application.loadedLevelName);
				
				
				
			}

			if (GUI.Button(new Rect(530,30,200,60), "Exit game")) {
				
				Network.Disconnect();
				hasplayer = false;
				menu = false;
				Application.Quit();
			}

			if(gameOver){
				GUI.Label(new Rect(300, 200, 200, 60), "Game Over");

				GUI.Label(new Rect(300, 300, 200, 60), "Score :" + (int)score );

				Screen.showCursor = true;
				Screen.lockCursor = false;
			}
		}


		if(!hasplayer){
			if (GUI.Button(new Rect(100, 100, 250, 100), "Light Lord")){
	            Instantiate(lord, spoon, Quaternion.identity);
				hasplayer = true;
				menuCamera.enabled=false;
				isKnight = false;
			}
	 
	        if (GUI.Button(new Rect(100, 250, 250, 100), "Warrior")){
				Network.Instantiate(warrior, spoon, Quaternion.identity, 0);
				hasplayer = true;
				menuCamera.enabled = false;
				isKnight = true;
				Screen.showCursor = false;
				Screen.lockCursor = true;
				if (!Network.isClient || Network.isServer){
					addKnight();
				}else{
					networkView.RPC("addKnight", RPCMode.Server);
				}

			}
	            
		}

		
	}

	[RPC]
	public void GameOver(){
		if(gameOver)
			return;
		gameOver = true;
		menu = true;
		score = GetComponent<AISpawner>().difficultyTime;
	}
	
	public void selectPlayer(){
		hasplayer = false;
	}
	
	
	
	
	

	
	
}
