using UnityEngine;
using System.Collections;
using System;

public class CameraMover : MonoBehaviour {
	/**
	 * very simple script that lets us move the camera around with the arrow keys
	 * Useful for early testing, but can likely be deleted later
	 */


	void Update()
	{
		Vector3 oldPos = this.transform.position;
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		if (h>0) {
			transform.position = new Vector3(oldPos.x + 1,oldPos.y,oldPos.z);
		} else if(h<0) {
			transform.position = new Vector3(oldPos.x -1,oldPos.y,oldPos.z);
		}
		if (v<0) {
			transform.position = new Vector3(oldPos.x, oldPos.y-1,oldPos.z);
		} else if(v>0) {
			transform.position = new Vector3(oldPos.x,oldPos.y+1,oldPos.z);
		}
	}
}
