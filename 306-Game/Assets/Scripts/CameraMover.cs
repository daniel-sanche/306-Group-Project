using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {
	/**
	 * very simple script that lets us move the camera around with the arrow keys
	 * Useful for early testing, but can likely be deleted later
	 */

	void Update()
	{
		Vector3 oldPos = this.transform.position;
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.position = new Vector3(oldPos.x + 1,oldPos.y,oldPos.z);
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.position = new Vector3(oldPos.x -1,oldPos.y,oldPos.z);
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.position = new Vector3(oldPos.x, oldPos.y-1,oldPos.z);
		}
		if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.position = new Vector3(oldPos.x,oldPos.y+1,oldPos.z);
		}
	}
}
