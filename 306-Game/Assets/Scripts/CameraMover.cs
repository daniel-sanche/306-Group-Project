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


	private TileGenerator generator;

	void Awake () {
		generator = gameObject.GetComponent<TileGenerator> () as TileGenerator;
		generator.SetActiveChunk (0, 0);
		maxX = generator.TilesPerChunkX;
		maxY = generator.TilesPerChunkY;
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
			int chunkNumX = (int)Mathf.Floor (newPos.x / generator.TilesPerChunkX);
			minX = chunkNumX * generator.TilesPerChunkX;
			maxX = (chunkNumX+1) * generator.TilesPerChunkX;
			int chunkNumY = (int)Mathf.Floor (newPos.y / generator.TilesPerChunkY);
			minY = chunkNumY * generator.TilesPerChunkY;
			maxY = (chunkNumY+1) * generator.TilesPerChunkY;
			generator.SetActiveChunk (chunkNumX, chunkNumY);
		}
	}
}
