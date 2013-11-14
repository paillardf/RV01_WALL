using UnityEngine;
using System.Collections;

public class BlockInitializer : MonoBehaviour {

	public Transform Stone;
	public Transform MediumStone;
	public Transform Plank;
	public Transform Door;
	public Transform Stairs;
	public Transform Torch;
	public Transform Vault;
	public Transform Wood;
	
	
	public void buildBlock (object[] args){
		rpcBuildBlock((string)args[0], (Vector3)args[1], (Quaternion)args[2]);		
	}
	
	[RPC]
    public void rpcBuildBlock (string blockStr,Vector3 position, Quaternion rotation)
    {
		
		Transform block = getBlock(blockStr);
		if (!Network.isClient && !Network.isServer){
			Instantiate(block, position, rotation);
		}else{
			Network.Instantiate(block, position, rotation,0);
		}
        
    }
	
	private Transform getBlock(string str){
	
		switch(str){
			case "Stone":
			return Stone;
			case "MediumStone":
			return MediumStone;	
			case "Plank":
			return Plank;	
			case "Door":
			return Door;
			case "Stairs":
			return Stairs;
			case "Torch":
			return Torch;
			case "Vault":
			return Vault;
			case "Wood":
			return Wood;
			
		}
		return null;
		
	}
}
