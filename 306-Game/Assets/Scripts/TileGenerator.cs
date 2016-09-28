using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class TileGenerator : MonoBehaviour {

	private Transform boardHolder; 

	public GameObject grass;
	public GameObject gravel;

	public int rows = 10;
	public int cols = 10;

	void Awake () {
		GetChunk ();
	}


	public TileChunk GetChunk () {
		boardHolder = new GameObject ("Tiles").transform;
		List<GameObject> tileList = new List<GameObject> ();
		for (int y = 0; y < rows; y++) {
			for(int x=0; x<cols; x++){
				GameObject toInstantiate;
				if (Random.value >= 0.5) {
					toInstantiate = grass;
				} else {
					toInstantiate = gravel;
				}
				GameObject instance = Instantiate (toInstantiate, new Vector3(x,y,0), Quaternion.identity) as GameObject;
				tileList.Add (instance);
				instance.transform.SetParent (boardHolder);
			}
		}
		return new TileChunk (tileList);
	}
		
}
