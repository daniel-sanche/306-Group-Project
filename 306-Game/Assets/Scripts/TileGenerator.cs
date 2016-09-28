using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileGenerator : MonoBehaviour {

	public int cols = 10;
	public int rows = 10;

	public GameObject grass;

	// Use this for initialization
	public void GenerateLevel () {
		for (int y = 0; y < rows; y++) {
			for(int x=0; x<cols; x++){
				GameObject thisTile = Instantiate (grass, new Vector3(x,y, 0), Quaternion.identity) as GameObject;
			}
		}
	}
		
}
