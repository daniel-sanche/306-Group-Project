using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileChunk : MonoBehaviour {


	private int [,] terrainMap;
	private List<GameObject> tileList;
	private Vector2 topLeftCorner;
	private TileGenerator generator;

	private bool isRendered = false;

	private List<TileChunk> connectedChunks;
	private List<TileChunk> distantChunks;


	public void InitChunk(int [,] terrain, int x, int y){
		generator = GetComponent<TileGenerator> ();
		terrainMap = terrain;
		topLeftCorner = new Vector2 (x*generator.TilesPerChunkX, y*generator.TilesPerChunkY);
		tileList = new List<GameObject> ();
		connectedChunks = new List<TileChunk>();
		distantChunks = new List<TileChunk>();
	}

	public void Activate(){
		Render ();
		foreach (TileChunk thisNeighbour in connectedChunks) {
			thisNeighbour.Render ();
		}
		foreach (TileChunk distantChunk in distantChunks) {
			distantChunk.Remove ();
		}
	}

	public void AddConnectedChunk(TileChunk newChunk){
		connectedChunks.Add (newChunk);
	}

	public void AddDistantChunk(TileChunk newChunk){
		distantChunks.Add (newChunk);
	}


	private void Render() {
		if (!isRendered) {
			tileList.Clear ();
			for (int x = 0; x < generator.TilesPerChunkX; x++) {
				for (int y = 0; y < generator.TilesPerChunkY; y++) {
					int code = terrainMap [x, y];
					GameObject groundTile = generator.SpriteForCode (code);
					GameObject instance = Instantiate (groundTile, new Vector3 (topLeftCorner.x + x, topLeftCorner.y + y, 0), Quaternion.identity) as GameObject;
					tileList.Add (instance);
				}
			}
			isRendered = true;
		}
	}

	private void Remove () {
		if (isRendered) {
			foreach (GameObject thisTile in tileList) {
				GameObject.Destroy (thisTile);
			}
			tileList.Clear ();
			isRendered = false;
		}
	}
	

}
