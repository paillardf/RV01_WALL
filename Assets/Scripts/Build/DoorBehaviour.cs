using UnityEngine;
using System.Collections;

public class DoorBehaviour : MonoBehaviour {
	public  Transform Pivot;
	public  float     OpenSpeed = 3;
	private bool      isOpened = false;
	private bool      isBroken = false;
	// TODO : cas où la porte a changé de place
	private bool      isMoving = false;

	private static Quaternion s_closed = Quaternion.Euler(0, 0, 0);
	private static Quaternion s_opened = Quaternion.Euler(0, 90, 0);

	public void Update() {
		if(isOpened && transform.eulerAngles.y < 90) {
			transform.RotateAround(Pivot.position, Pivot.up, OpenSpeed);
			if(transform.eulerAngles.y > 90) {
				transform.rotation = s_opened;
			}
			isMoving = true;
		}
		else if(!isOpened && transform.eulerAngles.y > 0) {
			transform.RotateAround(Pivot.position, -Pivot.up, OpenSpeed);
			if(transform.eulerAngles.y > 90) {
				transform.rotation = s_closed;
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
