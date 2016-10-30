using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType {Grass, Gravel, Water, Sand, Mountain, Floor, FloorTop, FloorBottom, FloorLeft, FloorRight, FloorTL, FloorTR, FloorBL, FloorBR, FloorDoorL, FloorDoorR, FloorDoorT, FloorDoorB};

public class TileGenerator : MonoBehaviour {
	/**
	 * This class is responsible for procedurally generating the island
	 * All functions should be static
	 **/

	//scale impacts how rough the heightmap is. Lower values will create more peaks and calleys
	public static float heightmapScale = 0.1f;
	public static float waterThreshold = 0.25f;
	/**
	 *	Generates a map of pixels representing the island
	 *  xSize = horizontal size of map
	 *	ySize = vertical size of map
	 *	returns an array of numbers where each number represents the terrain at that space
	**/
	public static TileType[,] GenerateTileMap(int xSize, int ySize){
		TileType[,] tileMap = new TileType[xSize, ySize];
		for(int x=0; x<xSize; x++){
			for(int y=0; y<ySize; y++){
				float heightVal = Mathf.PerlinNoise (x/(xSize*heightmapScale), y/(ySize*heightmapScale));
				if (heightVal < waterThreshold) {
					tileMap [x, y] = TileType.Water;
				} else if (heightVal < waterThreshold + 0.05){
					tileMap [x, y] = TileType.Sand;
				} else if (Random.value >= 0.15) {
					tileMap [x, y] = TileType.Grass;
				} else {
					tileMap [x, y] = TileType.Gravel;
				}
			}
		}
		Vector2 buildingSize = new Vector2 (10, 10);
		TileType[,] buildingMap = BuidingGenerator.GenerateBuilding (buildingSize, roomSplitScaler:2);
		for (int x = 0; x < buildingSize.x; x++) {
			for (int y = 0; y < buildingSize.y; y++) {
				tileMap [x, y] = buildingMap [x, y];
			}
		}
		return tileMap;
	}
		
		
}
