var target : Transform;
var distance = 10.0;

var xRotSpeed = 270.0;
var yRotSpeed = 140.0;

var zoomSpeed = 0.1;
var moveSpeed = 20;
var verticalSpeed = 6;
var yMinLimit = -20;
var yMaxLimit = 80;

private var x = 0.0;
private var y = 0.0;

@script AddComponentMenu("Camera-Control/Mouse Orbit")
var ball : Transform;
function Start () {
    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
	
}

function LateUpdate () {


	if(Input.GetKeyDown(KeyCode.Space)){
	  var newBlock : Transform = Instantiate (ball, transform.position, transform.rotation);
		newBlock.transform.rigidbody.AddForce(transform.forward*1000000 );
	}
	
	
    if (target) {
      
    	if(Input.GetAxis("Mouse ScrollWheel") != 0 &&  Input.GetKey(KeyCode.AltGr)) {
	    	
	 		target.transform.Translate(Vector3.down * Input.GetAxis("Mouse ScrollWheel") * verticalSpeed);
	    }else if(Input.GetAxis("Mouse ScrollWheel") != 0) {
	    	distance+=zoomSpeed*Input.GetAxis("Mouse ScrollWheel")* 0.02;
	    }
	    if (Input.GetMouseButton(2)) {
	  
	        x += Input.GetAxis("Mouse X") * xRotSpeed * 0.02;
	        y -= Input.GetAxis("Mouse Y") * yRotSpeed * 0.02;
	 		
	 		y = ClampAngle(y, yMinLimit, yMaxLimit);
	 		 
	        
	   }
	   if (Input.GetKey (KeyCode.UpArrow)){
	    dir = Camera.main.transform.forward;
	    dir.y = 0;
	    dir.Normalize();
	 	target.transform.Translate(dir * Time.deltaTime * moveSpeed, target);
	   }
	   if (Input.GetKey (KeyCode.DownArrow)){
	    dir = -Camera.main.transform.forward;
	    dir.y = 0;
	    dir.Normalize();
	 	target.transform.Translate(dir * Time.deltaTime * moveSpeed, target);
	   }
	    if (Input.GetKey (KeyCode.LeftArrow)){
	    dir = -Camera.main.transform.right;
	    dir.y = 0;
	    dir.Normalize();
	 	target.transform.Translate(dir * Time.deltaTime * moveSpeed, target);
	   }
	    if (Input.GetKey (KeyCode.RightArrow)){
	    dir = Camera.main.transform.right;
	    dir.y = 0;
	    dir.Normalize();
	 	target.transform.Translate(dir * Time.deltaTime * moveSpeed, target);
	   }
   
   		var rotation = Quaternion.Euler(y, x, 0);      
   		var position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
        transform.position = position;
        transform.rotation = rotation;
        
    }
}

static function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}