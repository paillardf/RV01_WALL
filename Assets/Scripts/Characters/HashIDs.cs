using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {

	// Here we store the hash tags for various strings used in our animators.
    public int climbState;
    public int locomotionState;
    //public int shoutState;

    public int speedFloat;
	public int climbBool;
	public int attackBool;
    public int angularSpeedFloat;

    
    
    void Awake ()
    {
        climbState = Animator.StringToHash("Base Layer.Climb");
        locomotionState = Animator.StringToHash("Base Layer.Locomotion");
        //shoutState = Animator.StringToHash("Shouting.Shout");
		
		
        speedFloat = Animator.StringToHash("Speed");
		climbBool = Animator.StringToHash("Climb");
        angularSpeedFloat = Animator.StringToHash("AngularSpeed");
		attackBool  = Animator.StringToHash("Attack"); 
      
    }
}
