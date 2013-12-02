using UnityEngine;
using System.Collections;

public class DoorBehaviour : MonoBehaviour {
	public  Transform Pivot;
	public  float     OpenSpeed = 3;
	private bool      isOpened = false;
	private bool      isBroken = false;
	private bool      isMoving = false;
	private float     m_minAngle;
	private float     m_maxAngle;

	private Quaternion m_closed;
	private Quaternion m_opened;

	public void Start() {
		m_minAngle = transform.rotation.eulerAngles.y;
		m_maxAngle = m_minAngle + 90;
		m_closed = transform.rotation;
     	m_opened = Quaternion.Euler(0, m_maxAngle, 0);
	}

	public void Update() {
		if(isBroken) {
			return;
		}
		if(!rigidbody.velocity.Equals(Vector3.zero)) {
			isBroken = true;
		}
		if(isOpened && transform.eulerAngles.y < m_maxAngle) {
			transform.RotateAround(Pivot.position, Pivot.up, OpenSpeed);
			if(transform.eulerAngles.y > m_maxAngle) {
				transform.rotation = m_opened;
			}
			isMoving = true;
		}
		else if(!isOpened && transform.eulerAngles.y > m_minAngle) {
			transform.RotateAround(Pivot.position, -Pivot.up, OpenSpeed);
			if(transform.eulerAngles.y > m_maxAngle) {
				transform.rotation = m_closed;
			}
			isMoving = true;
		}
		else {
			isMoving = false;
		}
	}
	
	public void interact() {
		if(!isMoving) {
			isOpened = !isOpened;
		}
	}
}
