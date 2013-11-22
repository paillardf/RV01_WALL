using UnityEngine;
using System.Collections;

public class BuildPlacement : MonoBehaviour {


public Transform currentBox;

public Transform stoneBox;
public Transform mediumStoneBox;
public Transform woodBox;
public Transform stairsBox;
public Transform plankBox;
public Transform torchBox;
public Transform doorBox;
public Transform vaultBox;

void Start () {

	
}


void selectBox(string boxName){
	if(currentBox!=null){
	
		Destroy(currentBox.gameObject);
	}

	if(boxName=="Stone"){
		currentBox=(Transform)Instantiate (stoneBox, Input.mousePosition, Quaternion.Euler(0,0,0));
	}else if(boxName=="MediumStone"){
		currentBox=(Transform)Instantiate (mediumStoneBox, Input.mousePosition, Quaternion.Euler(0,0,0));	
	}else if(boxName=="Wood"){
		currentBox=(Transform)Instantiate (woodBox, Input.mousePosition, Quaternion.Euler(0,0,0));	
	}else if(boxName=="Stairs"){
		currentBox=(Transform)Instantiate (stairsBox, Input.mousePosition, Quaternion.Euler(-90,0,0));	
	}else if(boxName=="Plank"){
		currentBox=(Transform)Instantiate (plankBox, Input.mousePosition, Quaternion.Euler(0,0,0));	
	}else if(boxName=="Torch"){
		currentBox=(Transform)Instantiate (torchBox, Input.mousePosition, Quaternion.Euler(0,0,0));	
	}else if(boxName=="Door"){
		currentBox=(Transform)Instantiate (doorBox, Input.mousePosition, Quaternion.Euler(0,0,0));	
	}else if(boxName=="Vault"){
		currentBox=(Transform)Instantiate (vaultBox, Input.mousePosition, Quaternion.Euler(0,0,0));	
	}else{
		currentBox = null;
	}

}

private RaycastHit hit = new RaycastHit();
	
void Update () {
	if(currentBox==null)
		return;
		
	
	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	

	int mask  = Constants.MaskCollision;
	if(Input.GetKey(KeyCode.RightShift)){
		mask = Constants.MaskBlock;
	}
	
	
	if (Physics.Raycast (ray, out hit ,40.0f, mask)){
			if(Input.GetKey(KeyCode.RightShift)||Input.GetKey(KeyCode.LeftShift)){
			currentBox.transform.position = hit.transform.position;
			currentBox.rotation = hit.transform.rotation;
			if(Input.GetMouseButtonDown(0)){
				hit.transform.SendMessage("destroy");
				
			}
			
			
			}else if(Input.GetKeyUp(KeyCode.RightShift)||Input.GetKeyUp(KeyCode.LeftShift)){
			currentBox.SendMessage("init");
		}else{
			currentBox.transform.position = hit.point+hit.normal.normalized;	
			Vector3 closestPoint  = currentBox.collider.ClosestPointOnBounds(hit.point);
			float distance  = Vector3.Distance(hit.point, closestPoint);
			currentBox.transform.position = hit.point+hit.normal.normalized*(1-distance);	
			currentBox.transform.position = new Vector3(Mathf.Round(currentBox.transform.position.x*2)/2, Mathf.Round(currentBox.transform.position.y*2)/2, Mathf.Round(currentBox.transform.position.z*2)/2);
			//block.transform.position = (hit.point + block.transform.position - closestPoint)	
		}
	}
	if (Input.GetMouseButtonDown(1)){
		currentBox.SendMessage("rotate");	
			
	}
	
	if (Input.GetMouseButtonDown(0)){
		currentBox.SendMessage("buildBlock");
	}
			
}
	
	
	
	
	






}
