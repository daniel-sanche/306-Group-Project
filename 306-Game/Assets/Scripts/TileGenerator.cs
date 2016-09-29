using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileGenerator : MonoBehaviour {

	private static Transform boardHolder; 

	public TileChunk [,] chunkList;
	public GameObject grass;
	public GameObject gravel;

	public static int rows = 10;
	public static int cols = 10;

	void Awake () {
		int[,] tileMap = new int[TileChunk.chunkSizeX, TileChunk.chunkSizeY];
		for(int x=0; x<TileChunk.chunkSizeX; x++){
			for(int y=0; y<TileChunk.chunkSizeY; y++){
				tileMap [x, y] = 0;
			}
		}

		chunkList = new TileChunk[rows, cols];
		for(int x=0; x<cols; x++){
			for(int y=0; y<rows; y++){
				TileChunk newChunk = gameObject.AddComponent<TileChunk> () as TileChunk;
				newChunk.InitChunk(tileMap, x, y);
				chunkList [x, y] = newChunk;
			}
		}
	}


	public void SetActiveChunk(int activeX, int activeY){
		if (activeX > 0 && activeX < cols && activeY > 0 && activeY < rows) {
			TileChunk newChunk = chunkList [activeX, activeY];
			newChunk.Render ();
		}
	}

	public GameObject SpriteForCode(int code){
		GameObject groundTile;
		if (code == 0) {
			groundTile = grass;
		} else {
			groundTile = gravel;
		}
		return groundTile;
	}
		
}
