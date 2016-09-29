using UnityEngine;
using System.Collections;
using System;

public class CameraMover : MonoBehaviour {
	/**
	 * very simple script that lets us move the camera around with the arrow keys
	 * Useful for early testing, but can likely be deleted later
	 */

	private int minX = 0;
	private int maxX = 0;
	private int minY = 0;
	private int maxY = 0;


	private TileRenderer tileRenderer;

	void Awake () {
		tileRenderer = gameObject.GetComponent<TileRenderer> () as TileRenderer;
		tileRenderer.SetActiveChunk (0, 0);
		maxX = (int)tileRenderer.TilesPerChunk.x;
		maxY = (int)tileRenderer.TilesPerChunk.y;
	}


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
			int chunkNumX = (int)Mathf.Floor (newPos.x / tileRenderer.TilesPerChunk.x);
			minX = chunkNumX * (int)tileRenderer.TilesPerChunk.x;
			maxX = (chunkNumX+1) * (int)tileRenderer.TilesPerChunk.x;
			int chunkNumY = (int)Mathf.Floor (newPos.y / tileRenderer.TilesPerChunk.y);
			minY = chunkNumY * (int)tileRenderer.TilesPerChunk.y;
			maxY = (chunkNumY+1) * (int)tileRenderer.TilesPerChunk.y;
			tileRenderer.SetActiveChunk (chunkNumX, chunkNumY);
		}
	}
}
