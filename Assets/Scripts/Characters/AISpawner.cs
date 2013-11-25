using UnityEngine;
using System.Collections;

public class AISpawner : MonoBehaviour {

	private GameController gc;

	public Vector3[] positionSpawner = {new Vector3(40, 40, 6), new Vector3(20, 40, 6)};

	public GameObject iaPrefab;
	public double difficultyTime = 0;

	void SpawnIA(){
		if (!Network.isClient && !Network.isServer){
			Instantiate(iaPrefab, positionSpawner[Random.Range(0,positionSpawner.Length)], Quaternion.identity);
		}else{
			Network.Instantiate(iaPrefab, positionSpawner[Random.Range(0,positionSpawner.Length)], Quaternion.identity,0);
		}
		gc.addIA();
			
	}



	// Use this for initialization
	void Start () {
		gc = GetComponent<GameController>();
	}
	
	// Update is called once per frame
	private float sponTime= 1;

	void Update () {
		sponTime-=Time.deltaTime;
		if(gc.nbKnight>0){
			difficultyTime +=Time.deltaTime;
			if(sponTime<0){
				sponTime = 1;

				if(gc.nbIA<difficultyTime/60*gc.nbKnight){
					SpawnIA();
				}
			}
		}


	
	}
}
