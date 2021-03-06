﻿using UnityEngine;
using System.Collections;

public class AnimatorSetup{
	
	public float speedDampTime = 0.1f;
	public float angularSpeedDampTime = 0.2f;
	public float angleResponseTime = 0.2f;
	
	private Animator anim;
	private HashIDs hash;
	
	public AnimatorSetup(Animator animator, HashIDs hashIDs){
		anim = animator;
		hash = hashIDs;
	}
	
	public void Setup(float speed, float angle, bool climb, bool attack, bool death){
	
		float angularSeed = angle/angleResponseTime;
		
		anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
		anim.SetFloat(hash.angularSpeedFloat, angularSeed, angularSpeedDampTime, Time.deltaTime);
		anim.SetBool(hash.climbBool, climb);
		anim.SetBool(hash.attackBool, attack);
		anim.SetBool(hash.deathBool, death);
	}
}
