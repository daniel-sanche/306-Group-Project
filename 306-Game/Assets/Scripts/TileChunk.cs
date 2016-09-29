using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileChunk : MonoBehaviour {

	public static int chunkSizeX = 10;
	public static int chunkSizeY = 10;

	private int [,] terrainMap;
	private List<GameObject> tileList;
	private Vector2 topLeftCorner;
	private TileGenerator generator;

	private bool isActive = false;


	public void InitChunk(int [,] terrain, int x, int y){
		terrainMap = terrain;
		topLeftCorner = new Vector2 (x*chunkSizeX, y*chunkSizeY);
		Debug.Log (topLeftCorner);
		tileList = new List<GameObject> ();
		generator = GetComponent<TileGenerator> ();
	}

	public void Render() {
		if (!isActive) {
			tileList.Clear ();
			for (int x = 0; x < chunkSizeX; x++) {
				for (int y = 0; y < chunkSizeY; y++) {
					int code = terrainMap [x, y];
					Debug.Log (topLeftCorner.x + x + ", " + topLeftCorner.y + y);
					GameObject groundTile = generator.SpriteForCode (code);
					GameObject instance = Instantiate (groundTile, new Vector3 (topLeftCorner.x + x, topLeftCorner.y + y, 0), Quaternion.identity) as GameObject;
					tileList.Add (instance);
				}
			}
			isActive = true;
		}
	}

	// Use this for initialization
	public void Remove () {
		if (isActive) {
			foreach (GameObject thisTile in tileList) {
				GameObject.Destroy (thisTile);
			}
			tileList.Clear ();
			isActive = false;
		}
	}
	

}
