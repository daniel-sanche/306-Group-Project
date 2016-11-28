using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileRenderer : MonoBehaviour {
	/**
	 * This class is responsible for rendering the tiles, and managing the matric of TileChunks
	 * Because it renders based on what the camera can see, this script should be a component of the camera
	 * Works closely with TileGenerator. TileGenerator creates the map, and TileRenderer turns it into sprites, 
	 * breaks it into manageable chunks, and activates the chunks when needed
	 **/

	public Vector2 NumChunks = new Vector2 ( 30, 30);
	public Vector2 TilesPerChunk = new Vector2 (15, 10);
	public int numBuildings = 30;

	public GameObject tileChunkObj;

	public static Transform heading; 
	private TileChunk [,] chunkMatrix;

	//these variables are for changing the active  chunk
	private int minX = 0;
	private int maxX = 0;
	private int minY = 0;
	private int maxY = 0;

	/**
	 * Called when the script is first initialized
	 **/
	void Awake(){
		maxX = (int)TilesPerChunk.x;
		maxY = (int)TilesPerChunk.y;
		InitMap ();
		SetActiveChunk (0, 0);

	}

	/**
	 * Called on a continuous loop
	 * Will check the current position of the camera, and render new chunks if necessary
	 **/
	void Update(){
		Vector3 newPos = this.transform.position;
		if (!(newPos.x < maxX && newPos.x > minX && newPos.y < maxY && newPos.y > minY)) {
			//changing active chunk of tiles
			int chunkNumX = (int)Mathf.Floor (newPos.x / TilesPerChunk.x);
			minX = chunkNumX * (int)TilesPerChunk.x;
			maxX = (chunkNumX+1) * (int)TilesPerChunk.x;
			int chunkNumY = (int)Mathf.Floor (newPos.y / TilesPerChunk.y);
			minY = chunkNumY * (int)TilesPerChunk.y;
			maxY = (chunkNumY+1) * (int)TilesPerChunk.y;
			SetActiveChunk (chunkNumX, chunkNumY);
		}
	}
		

	/**
	 * --------------------------------------------------------------------------------------------------------------------------------------------
	 * Helper Functions
	 * --------------------------------------------------------------------------------------------------------------------------------------------
	 **/

	/**
	* Sets the chunk at the current location as active
	* The active chuk and its neighbours will be rendered on screen
	* activeX = the X position of the new active chunk
	* activeY = the Y position of the new active chunk
	**/
	private void SetActiveChunk(int activeX, int activeY){
		if (activeX >= 0 && activeX < NumChunks.x && activeY >= 0 && activeY < NumChunks.y) {
			TileChunk newChunk = chunkMatrix [activeX, activeY];
			newChunk.Activate ();
		}
	}
		

	/**
	 * Initial map set up
	 * Calls on TileGenerator to create a map, then generates chunks from the map
	 **/
	private void InitMap(){

		var gameboard = new GameObject ("GameBoard");
		gameboard.AddComponent("A_");
		heading = gameboard.transform;

		TileType[,] tileMap = TileGenerator.GenerateTileMap ((int)TilesPerChunk.x*(int)NumChunks.x, (int)TilesPerChunk.y*(int)NumChunks.y, numBuildings);
		chunkMatrix = ChunksFromTileMap (tileMap, NumChunks, TilesPerChunk);
	}

	/**
	 * Converts the grid of tiles into a grid of TileChunks
	 * Tiles are grouped together to create the chunks
	 * combinedMap = map of tiles, created by TileGenerator
	 * numChunks = the number of chunks to create in the x and y direction
	 * numTiles = the number of tiles per chunk in the x and y direction
	 * returns a grid of TileChunks
	 **/
		private TileChunk [,] ChunksFromTileMap(TileType[,] combinedMap, Vector2 numChunks, Vector2 numTiles){
		//initialize empty array to hold out chunks
		TileChunk [,] chunkMat = new TileChunk[(int)numChunks.x, (int)numChunks.y];
		//iterate over each space in the array, generating a chunk for the position
		for (int chunkCol = 0; chunkCol < numChunks.x; chunkCol++) {
			for (int chunkRow = 0; chunkRow < numChunks.y; chunkRow++) {
				//create an array for the tiles this chunk is responsible for
				TileType[,] theseTiles = new TileType[(int)numTiles.x, (int)numTiles.y];
				for (int tileCol = 0; tileCol < numTiles.x; tileCol++) {
					for (int tileRow = 0; tileRow < numTiles.y; tileRow++) {
						theseTiles [tileCol, tileRow] = combinedMap [tileCol+chunkCol*(int)numTiles.x, tileRow+chunkRow*(int)numTiles.y];
					}
				}
				//create the game object for the chunl
				GameObject newChunk = Instantiate (tileChunkObj, new Vector3 (chunkCol*TilesPerChunk.x, chunkRow*TilesPerChunk.y, 0), Quaternion.identity) as GameObject;
				TileChunk chunkScript = newChunk.GetComponent<TileChunk> ();
				chunkScript.transform.SetParent (TileRenderer.heading);
				chunkScript.InitChunk(theseTiles, chunkCol, chunkRow, TilesPerChunk);
				chunkMat [chunkCol, chunkRow] = chunkScript;
			}
		}
		ConnectChunks(chunkMat, numChunks);
		return chunkMat;
	}

	/**
	 * Helper function to attach neighbour chunks and distant chunks
	 * Chunks must be connected to notify each other when to render or remove themselves to provide a seamless movement experience
	 * chunkArr = 2D grid of TileChunks
	 * numChunks = x/y values representing the numbers to expect in chunkArr
	 **/
	private static void ConnectChunks(TileChunk [,] chunkArr, Vector2 numChunks) {
		//iterate over each chunk
		for(int y=0; y<numChunks.x; y++){
			for(int x=0; x<numChunks.y; x++){
				TileChunk newChunk = chunkArr [x, y];
				//look at every chunk in a 5x5 grid around this chunk
				for (int i = -2; i <= 2; i++) {
					for (int j = -2; j <= 0; j++) {
						int newX = x + i;
						int newY = y + j;
						//we want to look behind us and above, to the chunks that have already been visited
						if (newX >= 0 && newX < numChunks.x && newY >= 0 && newY < numChunks.y && (i != 0 || j != 0) && (newX < x || newY < y)) {
							//at this point, we know this neighbour is close, a valid chunk, and has already been visited by this function
							//we will want to hook it up to the current chunk
							TileChunk closeChunk = chunkArr [newX, newY];
							//use abs to find the absolute distance between the chunks
							float absI = Mathf.Abs (i);
							float absJ = Mathf.Abs (j);
							if (absI + absJ == 1 || (absI == absJ && absI == 1)) {
								//the chunks are direct neighbours. Make them add each other to connected chunks
								closeChunk.AddConnectedChunk (newChunk);
								newChunk.AddConnectedChunk (closeChunk);
							} else {
								//the chunks are distant neighbours. Make them add each other to distant chunks
								closeChunk.AddDistantChunk (newChunk);
								newChunk.AddDistantChunk (closeChunk);
							}
						}
					}
				}
			}
		}
	}
}
