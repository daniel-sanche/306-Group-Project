using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileRenderer : MonoBehaviour {


	public Vector2 NumChunks = new Vector2 ( 30, 30);
	public Vector2 TilesPerChunk = new Vector2 (15, 10);

	public GameObject grass;
	public GameObject gravel;

	private Transform boardHolder; 
	private TileChunk [,] chunkMatrix;

	void Awake () {
		InitMap ();
	}

	private void InitMap(){
		int[,] tileMap = TileGenerator.GenerateTileMap ((int)TilesPerChunk.x*(int)NumChunks.x, (int)TilesPerChunk.y*(int)NumChunks.y);
		chunkMatrix = ChunksFromTileMap (tileMap, NumChunks, TilesPerChunk);
		ConnectChunks(chunkMatrix, NumChunks);
	}

	private TileChunk [,] ChunksFromTileMap(int[,] combinedMap, Vector2 numChunks, Vector2 numTiles){
		TileChunk [,] chunkMat = new TileChunk[(int)numChunks.x, (int)numChunks.y];
		for (int chunkCol = 0; chunkCol < numChunks.x; chunkCol++) {
			for (int chunkRow = 0; chunkRow < numChunks.y; chunkRow++) {
				int[,] theseTiles = new int[(int)numTiles.x, (int)numTiles.y];
				for (int tileRow = 0; tileRow < numTiles.x; tileRow++) {
					for (int tileCol = 0; tileCol < numTiles.y; tileCol++) {
						theseTiles [tileRow, tileCol] = combinedMap [tileRow+chunkRow*(int)numTiles.x, tileCol+chunkCol*(int)numTiles.y];
					}
				}
				TileChunk newChunk = gameObject.AddComponent<TileChunk> () as TileChunk;
				newChunk.InitChunk(theseTiles, chunkCol, chunkRow);
				chunkMat [chunkCol, chunkRow] = newChunk;
			}
		}
		return chunkMat;
	}

	private static void ConnectChunks(TileChunk [,] chunkArr, Vector2 numChunks) {
		for(int y=0; y<numChunks.x; y++){
			for(int x=0; x<numChunks.y; x++){
				TileChunk newChunk = chunkArr [x, y];
				for (int i = -2; i <= 2; i++) {
					for (int j = -2; j <= 0; j++) {
						int newX = x + i;
						int newY = y + j;
						if (newX >= 0 && newX < numChunks.y && newY >= 0 && newY < numChunks.x && (i != 0 || j != 0) && (newX < x || newY < y)) {
							TileChunk closeChunk = chunkArr [newX, newY];
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
		if (activeX >= 0 && activeX < NumChunks.x && activeY >= 0 && activeY < NumChunks.y) {
			TileChunk newChunk = chunkMatrix [activeX, activeY];
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
