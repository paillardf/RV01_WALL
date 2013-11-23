using UnityEngine;
using System.Collections;

public class AISpawner : MonoBehaviour {

	private GameController gc;

	Vector3 positionSpawner = new Vector3(40, 40, 6);
	public GameObject iaPrefab;
	public double difficultyTime = 0;

	void SpawnIA(){
		if (!Network.isClient && !Network.isServer){
			Instantiate(iaPrefab, positionSpawner, Quaternion.identity);
		}else{
			Network.Instantiate(iaPrefab, positionSpawner, Quaternion.identity,0);
		}
			
	}



	// Use this for initialization
	void Start () {
		gc = GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gc.nbKnight>0){
			difficultyTime +=Time.deltaTime;
			if(gc.nbIA<1+difficultyTime%60000){
				gc.addIA();
			}
		}


	
	}
}
