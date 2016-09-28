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
	private Transform boardHolder; 

	// Use this for initialization
	public void GenerateLevel () {
		boardHolder = new GameObject ("BoardTiles").transform;
		for (int y = 0; y < rows; y++) {
			for(int x=0; x<cols; x++){
				GameObject toInstantiate;
				if (Random.value >= 0.5) {
					toInstantiate = grass;
				} else {
					toInstantiate = gravel;
				}
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}
		
}
