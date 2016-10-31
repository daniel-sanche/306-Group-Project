using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType {Grass, Gravel, Water, Sand, Rock, Tree, Floor, FloorTop, FloorBottom, FloorLeft, FloorRight, FloorTL, FloorTR, FloorBL, FloorBR, FloorDoorL, FloorDoorR, FloorDoorT, FloorDoorB};

public class TileGenerator : MonoBehaviour {
	/**
	 * This class is responsible for procedurally generating the island
	 * All functions should be static
	 **/

	//size of unmoveable region on all edges of board
	public static int borderSize = 20;

	//scale impacts how rough the heightmap is. Lower values will create more peaks and calleys
	public static float heightmapScale = 0.1f;
	public static float treemapScale = 0.01f;

	//any point lower than this height will become water
	public static float waterThreshold = 0.25f;
	//any point heigher than this point will always be rocks
	public static float mountainThreshold = 0.8f;
	//any pount heigher than this height may generate rocks
	public static float rockThreshold = 0.7f;
	//liklihood of rocks forming in a rock-friendly region
	public static float rockGenProb = 0.3f;
	//any point higher than this may generate trees
	public static float treeHeightThreshold = 0.4f;
	//using a sperate tree noise function, any point above this may become trees
	public static float treeThreshold = 0.6f;
	//liklihood of genrating a tree in a tree-friendly region
	public static float treeGenProb = 0.2f;
	/**
	 *	Generates a map of pixels representing the island
	 *  xSize = horizontal size of map
	 *	ySize = vertical size of map
	 *	returns an array of numbers where each number represents the terrain at that space
	**/
	public static TileType[,] GenerateTileMap(int xSize, int ySize){

		TileType[,] tileMap = GenerateTerrain (xSize, ySize, 0);
		List<Vector2> acceptable = FreeBuildingLocations (new Vector2 (5, 5), tileMap);
		Debug.Log (acceptable.Count);
		AddBuilding (new Vector2 (borderSize, borderSize), new Vector2 (5, 5), tileMap);
		AddBuilding (new Vector2 (borderSize+7, borderSize), new Vector2 (10, 10), tileMap);

		return tileMap;
	}

	private static TileType[,] GenerateTerrain(int xSize, int ySize, int seed=-1){
		if (seed != -1) {
			Random.InitState (seed);
		}
		int heightXOffset = Random.Range (0, 1000);
		int heightYOffset = Random.Range (0, 1000);
		int treeXOffset = Random.Range (0, 1000);
		int treeYOffset = Random.Range (0, 1000);

		TileType[,] tileMap = new TileType[xSize, ySize];
		for(int x=0; x<xSize; x++){
			for (int y = 0; y < ySize; y++) {
				float heightVal = Mathf.PerlinNoise (heightXOffset + x / (xSize * heightmapScale), heightYOffset + y / (ySize * heightmapScale));
				float treeVal = Mathf.PerlinNoise (treeXOffset + x / (xSize * heightmapScale), treeYOffset + y / (ySize * heightmapScale));
				if (x < borderSize || x > xSize - borderSize ||
				   y < borderSize || y > ySize - borderSize) {
					if (heightVal < waterThreshold) {
						tileMap [x, y] = TileType.Water;
					} else if (treeVal > treeThreshold && heightVal > treeHeightThreshold) {
						tileMap [x, y] = tileMap [x, y] = TileType.Tree;
					} else {
						tileMap [x, y] = TileType.Rock;
					}
				} else {
					//add mountains, water, and sand based on height information
					if (heightVal < waterThreshold) {
						tileMap [x, y] = TileType.Water;
					} else if (heightVal < waterThreshold + 0.05) {
						tileMap [x, y] = TileType.Sand;
					} else if (heightVal > mountainThreshold) {
						tileMap [x, y] = TileType.Rock;
					} else if (heightVal > rockThreshold && Random.value < rockGenProb) {
						tileMap [x, y] = TileType.Rock;
					} else if (heightVal > rockThreshold + 0.01) {
						tileMap [x, y] = TileType.Gravel;
					} else {
						//add trees to middle ground
						if (treeVal > treeThreshold && heightVal > treeHeightThreshold && Random.value < treeGenProb) {
							tileMap [x, y] = TileType.Tree;
						} else {
							tileMap [x, y] = TileType.Grass;
						}
					}
				}
			}
		}
		return tileMap;
	}
		
	private static void AddBuilding(Vector2 bottomLeft, Vector2 buildingSize, TileType[,] tileMap){
		TileType[,] buildingMap = BuidingGenerator.GenerateBuilding (buildingSize);
		for (int x = 0; x < buildingSize.x; x++) {
			for (int y = 0; y < buildingSize.y; y++) {
				tileMap [(int)bottomLeft.x+x, (int)bottomLeft.y+y] = buildingMap [x, y];
			}
		}
	}

	private static List<Vector2> FreeBuildingLocations(Vector2 buildingSize, TileType[,] tileMap){
		int sizeX = tileMap.GetLength (0);
		int sizeY = tileMap.GetLength (1);
		int lastBlockX = -1;
		int lastBlockY = -1;
		//2 is added to building size to ensure no doors are blocked on any side
		Vector2 buildingSizeBuffered = new Vector2 (buildingSize.x + 2, buildingSize.y + 2);
		List<Vector2> result = new List<Vector2>();
		for (int x = sizeX-1; x >= 0; x--) {
			lastBlockX = -1;
			for (int y = sizeY-1; y >= 0; y--) {
				lastBlockY = -1;
				TileType tile = tileMap [x, y];
				bool isBlockingTile = (tile == TileType.Grass || tile == TileType.Gravel || tile == TileType.Sand);
				if (isBlockingTile) {
					lastBlockX = x;
					lastBlockY = y;
				}
				if(!(lastBlockX > (x-buildingSizeBuffered.x) || lastBlockY > (y-buildingSizeBuffered.y))){
					//add 1 to remove the buffer
					result.Add(new Vector2(x+1,y+1));
				}
			}
		}
		return result;
	}
		
}
