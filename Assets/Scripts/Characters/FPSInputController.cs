﻿using UnityEngine;
using System.Collections;

public class FPSInputController : MonoBehaviour {

	private CharacterMotor motor;
	private Animator m_animator;
	private HashIDs hash ;     
	
	// Use this for initialization
	void Awake () {
		motor = GetComponent<CharacterMotor>();
		
	}
	
	void Start() {
		m_animator = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
	}
	
	// Update is called once per frame
	void Update () {
		if ((!Network.isClient && !Network.isServer)||networkView.isMine){
			Vector3 directionVector;

			SixenseInput.Controller controller = SixenseInput.GetController(SixenseHands.LEFT);
			if(controller != null) {
				// Get the input vector from Razer Hydra
				directionVector = new Vector3(controller.JoystickX, 0, controller.JoystickY);
				motor.inputJump = controller.GetButton(SixenseButtons.BUMPER);
			}
			else {
			
				// Get the input vector from keyboard
				directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				motor.inputJump = Input.GetButton("Jump");
			}
			m_animator.SetFloat(hash.directionFloat, directionVector.x);
			m_animator.SetFloat(hash.speedFloat, directionVector.z);
			
			if (directionVector != Vector3.zero) {
				// Get the length of the directon vector and then normalize it
				// Dividing by the length is cheaper than normalizing when we already have the length anyway
				float directionLength = directionVector.magnitude;
				directionVector = directionVector / directionLength;
				
				// Make sure the length is no bigger than 1
				directionLength = Mathf.Min(1, directionLength);
				
				// Make the input vector more sensitive towards the extremes and less sensitive in the middle
				// This makes it easier to control slow speeds when using analog sticks
				directionLength = directionLength * directionLength;
				
				// Multiply the normalized direction vector by the modified length
				directionVector = directionVector * directionLength;
			}
			
			// Apply the direction to the CharacterMotor
			motor.inputMoveDirection = transform.rotation * directionVector;
		}
	}

}

