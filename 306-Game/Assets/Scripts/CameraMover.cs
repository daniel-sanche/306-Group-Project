using UnityEngine;
using System.Collections;
using System;

public class CameraMover : MonoBehaviour {
	/**
	 * very simple script that lets us move the camera around with the arrow keys
	 * Useful for early testing, but can likely be deleted later
	 */

	private int minX = 0;
	private int maxX = 10;
	private int minY = 0;
	private int maxY = 10;

	int chunkNumX = 0;
	int chunkNumY = 0;

	int chunkSizeX = 20;
	int chunkSizeY = 10;

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

		Vector3 newPos = this.transform.position;
		if (!(newPos.x < maxX && newPos.x > minX && newPos.y < maxY && newPos.y > minY)) {
			chunkNumX = (int)Mathf.Floor (newPos.x / chunkSizeX);
			minX = chunkNumX * chunkSizeX;
			maxX = (chunkNumX+1) * chunkSizeX;
			chunkNumY = (int)Mathf.Floor (newPos.y / chunkSizeY);
			minY = chunkNumY * chunkSizeY;
			maxY = (chunkNumY+1) * chunkSizeY;
		}
		Debug.Log (chunkNumX + ", " + chunkNumY);
	}
}
