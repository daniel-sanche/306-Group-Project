using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileGenerator : MonoBehaviour {

	public int cols = 10;
	public int rows = 10;

	public GameObject grass;
	public GameObject gravel;

	// Use this for initialization
	public void GenerateLevel () {
		for (int y = 0; y < rows; y++) {
			for(int x=0; x<cols; x++){
				if (Random.value >= 0.5) {
					Instantiate (grass, new Vector3 (x, y, 0), Quaternion.identity);
				} else {
					Instantiate (gravel, new Vector3 (x, y, 0), Quaternion.identity);
				}
			}
		}
	}
		
}
