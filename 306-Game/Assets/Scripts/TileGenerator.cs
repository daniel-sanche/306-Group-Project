using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileGenerator : MonoBehaviour {


	public static int[,] GenerateTileMap(int xSize, int ySize){
		int[,] tileMap = new int[xSize, ySize];
		for(int x=0; x<xSize; x++){
			for(int y=0; y<ySize; y++){
				if (Random.value >= 0.5) {
					tileMap [x, y] = 0;
				} else {
					tileMap [x, y] = 1;
				}
			}
		}
		return tileMap;
	}


		
}
