using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType {Grass, Gravel, Water, Sand, Rock, Tree, Floor, FloorTop, FloorBottom, FloorLeft, FloorRight, FloorTL, FloorTR, FloorBL, FloorBR, FloorDoorL, FloorDoorR, FloorDoorT, FloorDoorB, NULL};


public class TileGenerator : MonoBehaviour {
	/**
	 * This class is responsible for procedurally generating the island
	 * All functions should be static
	 **/

	//size of unmoveable region on all edges of board
	public static int borderSize = 20;

	//max size of buildings (nxn)
	public static int maxBuildingDim = 12;
	public static int minBuildingDim = 6;
	public static float minBuildingRatio = 0.6f;

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
	 *	buildingCount = number of buildings to add to map
	 *	seed = a seed value to use to recreate the map
	 *	returns an array of numbers where each number represents the terrain at that space
	**/
	public static TileType[,] GenerateTileMap(int xSize, int ySize, int buildingCount, int seed = -1){
		if (seed != -1) {
			Random.InitState (seed);
		}

		TileType[,] tileMap = GenerateTerrain (xSize, ySize);

		int buildingsAdded = 0;
		int failures = 0;
		//keep generating buildings until we hit our goal, or try too many times
		while (buildingsAdded < buildingCount && failures < 100) {
			int width = Random.Range (minBuildingDim, maxBuildingDim);
			int height = (int)Mathf.Round(width * Random.Range (minBuildingRatio, 1.0f));
			Vector2 buildingSize = new Vector2 (width, height);
			List<Vector2> acceptable = FreeBuildingLocations (buildingSize, tileMap);
			int numOptions = acceptable.Count;
			if (numOptions > 0) {
				Vector2 newPos = acceptable[Random.Range (0, numOptions)];
				AddBuilding (newPos, buildingSize, tileMap);
				buildingsAdded++;
			} else {
				failures++;
			}
		}
		return tileMap;
	}

	/**
	 * Generates the terrain features of the map
	 * Uses a PerlinNoise heightmap to add water, sand, gravel, and rocks
	 * Uses a PerlinNoise treemap along with the heightmap to generate forests
	 * xSize = the width of the map
	 * ySize = the height of the map
	 * returns a TileType[xSize,ySize] 2D array representing the terrain
	 **/
	private static TileType[,] GenerateTerrain(int xSize, int ySize){
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
					//add an impassible obstacle (water, trees, rocks) around the border of the map
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
		
	/**
	 * Adds a building to the map at the requested position
	 * bottomLeft = the bottom left coordinates of the new building
	 * buildingSize = the height and width of the new building
	 * tileMap = the existing map of tiles
	 **/
	private static void AddBuilding(Vector2 bottomLeft, Vector2 buildingSize, TileType[,] tileMap){
		TileType[,] buildingMap = BuidingGenerator.GenerateBuilding (buildingSize);
		for (int x = 0; x < buildingSize.x; x++) {
			for (int y = 0; y < buildingSize.y; y++) {
				tileMap [(int)bottomLeft.x+x, (int)bottomLeft.y+y] = buildingMap [x, y];
			}
		}
	}

	/**
	 * Finds the map positions where a building could be constructed
	 * Runs in O(n) time where N is the size of the tilemap
	 * buildingSize = the size of the building to search for
	 * tileMap = a 2D array of tile positions
	 * returns a list of coordinates where buildings can be constructed (bottom left corners)
	 **/
	private static List<Vector2> FreeBuildingLocations(Vector2 buildingSize, TileType[,] tileMap){
		int sizeX = tileMap.GetLength (0);
		int sizeY = tileMap.GetLength (1);
		int[] spaceAbove = new int[sizeX];
		int stripLen = 0;
		//2 is added to building size to ensure no doors are blocked on any side
		Vector2 buildingSizeBuffered = new Vector2 (buildingSize.x + 2, buildingSize.y + 2);
		List<Vector2> result = new List<Vector2>();
		for (int y = sizeY-1; y >= 0; y--) {
			stripLen = 0;
			for (int x = sizeX-1; x >= 0; x--) {
				TileType tile = tileMap [x, y];
				bool isBlockingTile = !(tile == TileType.Grass || tile == TileType.Gravel || tile == TileType.Sand);
				if (isBlockingTile) {
					spaceAbove [x] = 0;
					stripLen = 0;
				} else {
					spaceAbove [x] = spaceAbove [x] + 1;
					if (spaceAbove [x] >= buildingSizeBuffered.y) {
						stripLen++;
					} else {
						stripLen = 0;
					}

					if (stripLen >= buildingSizeBuffered.x) {
						//add 1 to remove the buffer
						result.Add (new Vector2 (x + 1, y + 1));
					}
				}
			}
		}
		return result;
	}
		
}
