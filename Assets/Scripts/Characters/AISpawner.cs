using UnityEngine;
using System.Collections;

public class AISpawner : MonoBehaviour {

	private GameController gc;

	public Transform[] positionSpawner;

	public GameObject[] iaPrefab;

	public double difficultyTime = 0;

	void SpawnIA(){

		int i = Random.Range(-5, 100);
		int selectedPrefab = 0;
		if(i<0){
			selectedPrefab = 1;
		}

		if (!Network.isClient && !Network.isServer){
			Instantiate(iaPrefab[selectedPrefab], positionSpawner[Random.Range(0,positionSpawner.Length)].position, Quaternion.identity);
		}else{
			Network.Instantiate(iaPrefab[selectedPrefab], positionSpawner[Random.Range(0,positionSpawner.Length)].position, Quaternion.identity,0);
		}
		gc.addIA();
			
	}



	// Use this for initialization
	void Start () {
		gc = GetComponent<GameController>();
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{

		float syncScore = 0;
		
		if (stream.isWriting)
		{
			
			syncScore = (float) difficultyTime;
			stream.Serialize(ref syncScore);
			
			
			
		}
		else
		{
			
			stream.Serialize(ref syncScore);
			difficultyTime = syncScore;
		}
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
