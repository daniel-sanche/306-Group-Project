using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
	public static int[,] GenerateTileMap(int xSize, int ySize){
		int[,] tileMap = new int[xSize, ySize];
		for(int x=0; x<xSize; x++){
			for(int y=0; y<ySize; y++){
				if (Random.value >= 0.15) {
					tileMap [x, y] = 0;
				} else {
					tileMap [x, y] = 1;
				}
			}
		}
		return tileMap;
	}


		
}
