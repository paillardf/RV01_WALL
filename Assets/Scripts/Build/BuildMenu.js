#pragma strict

public var mySkin : GUISkin;
var textureStair : Texture;
var texturePlank : Texture;
var textureWood : Texture;
var textureStone : Texture;
var textureMStone : Texture;
var textureVault : Texture;

private var longTouch : double = 0;
private var selectedMenu : int = 1;
private var mousePos_2D : Vector2;
private var hover : String;
function OnGUI () {
	
	
	if(Input.GetMouseButtonUp(1)){
			    if(hover!=""){
			   	 	
			   	 	Camera.main.SendMessage("selectBox", hover);
			   	 	hover = "";
			    }
			 		
	}	
	
	if(Input.GetMouseButtonDown(1)){
			mousePos_2D = Vector2(Input.mousePosition.x, (Screen.height - Input.mousePosition.y));
			hover="";
			longTouch= 0;
		}
		
	if(Input.GetMouseButton(1)){
	
		longTouch += Time.deltaTime;
		
		if(longTouch<1)
			return;
		
		GUI.skin = mySkin;
		var angle : int = 60;
		var width : int = 80;
		var height : int = 72;
		var centerR : int = 25;
		//pivotPoint = Vector2(Screen.width/2,Screen.height/2);
		GUIUtility.RotateAroundPivot (-30, mousePos_2D); 
			
		if(selectedMenu==1){
		
			GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent (textureStone, "Stone"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
			
			GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent (textureMStone, "MediumStone"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
			GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent (textureWood, "Wood"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
			GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent (texturePlank, "Plank"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
			GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent (textureStair, "Stairs"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
			
			
			
			GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent ("...", "2"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
		
		}else if(selectedMenu==2){
		GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent (textureVault, "Vault"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
			
		GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent ("Door", "Door"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
		GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent ("...", "1"));
			GUIUtility.RotateAroundPivot (angle, mousePos_2D);;
			
		GUI.Button (Rect (mousePos_2D.x-width/2,mousePos_2D.y-height-centerR, width, height ),  GUIContent ("Torch", "Torch"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D);
			
			
		/*GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("MediumStone", "MediumStone"));
		
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
		GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("Wood", "Wood"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
		GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("Stairs", "Stairs"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
		GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("Plank", "Plank"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
		GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("Torch", "Torch"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
		GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("Door", "Door"));
		GUIUtility.RotateAroundPivot (angle, mousePos_2D); 
		GUI.Button (Rect (mousePos_2D.x-50,mousePos_2D.y-100,100,60),  GUIContent ("Vault", "Vault"));
		*/
		}
		
		hover = GUI.tooltip;
		if(hover=="2"){
			selectedMenu=2;
			mousePos_2D = Vector2(Input.mousePosition.x, (Screen.height - Input.mousePosition.y));
		}else if(hover=="1"){
			selectedMenu=1;
			mousePos_2D = Vector2(Input.mousePosition.x, (Screen.height - Input.mousePosition.y));
		}
		
	}
	
	
		
}


function Update(){
 
 }