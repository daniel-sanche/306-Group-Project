using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType {Grass, Gravel, Floor, FloorTop, FloorBottom, FloorLeft, FloorRight, FloorTL, FloorTR, FloorBL, FloorBR};

public class TileGenerator : MonoBehaviour {
	/**
	 * This class is responsible for procedurally generating the island
	 * All functions should be static
	 **/


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
				if (Random.value >= 0.15) {
					tileMap [x, y] = TileType.Grass;
				} else {
					tileMap [x, y] = TileType.Gravel;
				}
			}
		}
		Vector2 buildingSize = new Vector2 (5, 5);
		TileType[,] buildingMap = BuidingGenerator.GenerateBuilding (buildingSize);
		for (int x = 0; x < buildingSize.x; x++) {
			for (int y = 0; y < buildingSize.y; y++) {
				tileMap [x, y] = buildingMap [x, y];
			}
		}
		return tileMap;
	}


		
}
