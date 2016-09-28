using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileChunk : MonoBehaviour {


	private List<GameObject> tileList;

	public TileChunk(List<GameObject> tiles){
		tileList = tiles;
	}

	// Use this for initialization
	public void Remove () {
		foreach (GameObject thisTile in tileList) {
			GameObject.Destroy (thisTile);
		}
	}
	

}
