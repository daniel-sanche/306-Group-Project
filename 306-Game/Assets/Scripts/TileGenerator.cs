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
				if (Random.value >= 0.5) {
					tileMap [x, y] = 0;
				} else {
					tileMap [x, y] = 1;
				}
			}
		}

		chunkList = new TileChunk[rows, cols];
		for(int y=0; y<rows; y++){
			for(int x=0; x<cols; x++){
				TileChunk newChunk = gameObject.AddComponent<TileChunk> () as TileChunk;
				newChunk.InitChunk(tileMap, x, y);
				chunkList [x, y] = newChunk;
				for (int i = -2; i <= 2; i++) {
					for (int j = -2; j <= 0; j++) {
						int newX = x + i;
						int newY = y + j;
						if (newX >= 0 && newX < cols && newY >= 0 && newY < rows && (i != 0 || j != 0) && (newX < x || newY < y)) {
							TileChunk closeChunk = chunkList [newX, newY];
							float absI = Mathf.Abs (i);
							float absJ = Mathf.Abs (j);
							if (absI + absJ == 1 || (absI == absJ && absI == 1)) {
								closeChunk.AddConnectedChunk (newChunk);
								newChunk.AddConnectedChunk (closeChunk);
							} else {
								closeChunk.AddDistantChunk (newChunk);
								newChunk.AddDistantChunk (closeChunk);
							}
						}
					}
				}
			}
		}
	}


	public void SetActiveChunk(int activeX, int activeY){
		if (activeX >= 0 && activeX < cols && activeY >= 0 && activeY < rows) {
			TileChunk newChunk = chunkList [activeX, activeY];
			newChunk.Activate ();
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
