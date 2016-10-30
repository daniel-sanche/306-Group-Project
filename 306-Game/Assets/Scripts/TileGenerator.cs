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

		Vector2 buildingSize = new Vector2 (10, 10);
		TileType[,] buildingMap = BuidingGenerator.GenerateBuilding (buildingSize, roomSplitScaler:2);
		for (int x = 0; x < buildingSize.x; x++) {
			for (int y = 0; y < buildingSize.y; y++) {
				tileMap [x, y] = buildingMap [x, y];
			}
		}
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
				float treeVal = Mathf.PerlinNoise (treeXOffset + x / (xSize * heightmapScale), treeXOffset + y / (ySize * heightmapScale));
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
		
		
}
