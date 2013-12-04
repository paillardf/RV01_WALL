using UnityEngine;
using System.Collections;

public class FootstepSounds : MonoBehaviour {
	public  AudioClip   grassFootsteps;
	public  AudioClip   blockFootsteps;
	private AudioSource m_audioSource;
	
	void Start () {
		m_audioSource = gameObject.AddComponent<AudioSource>();
	}

	public void OnControllerColliderHit(ControllerColliderHit hit) {
		bool isMoving = hit.controller.velocity != Vector3.zero; 
		bool isStepping = (hit.controller.collisionFlags & CollisionFlags.CollidedBelow) != 0;
		if(isMoving && isStepping) {
			// Play foot step sound
			if(!m_audioSource.isPlaying) {
				if(hit.collider.name.Equals("Island")) {
					m_audioSource.clip = grassFootsteps;
				}
				else {
					m_audioSource.clip = blockFootsteps;
				}
				m_audioSource.Play();
			}
		}
	}
}
