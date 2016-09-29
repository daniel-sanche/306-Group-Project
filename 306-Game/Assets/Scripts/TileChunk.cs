using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileChunk : MonoBehaviour {

	public static int chunkSizeX = 20;
	public static int chunkSizeY = 10;

	private int [,] terrainMap;
	private List<GameObject> tileList;
	private Vector2 topLeftCorner;
	private TileGenerator generator;

	private bool isRendered = false;

	private List<TileChunk> connectedChunks;
	private List<TileChunk> distantChunks;


	public void InitChunk(int [,] terrain, int x, int y){
		terrainMap = terrain;
		topLeftCorner = new Vector2 (x*chunkSizeX, y*chunkSizeY);
		tileList = new List<GameObject> ();
		generator = GetComponent<TileGenerator> ();
		connectedChunks = new List<TileChunk>();
		distantChunks = new List<TileChunk>();
	}

	public void Activate(){
		Render ();
		foreach (TileChunk thisNeighbour in connectedChunks) {
			thisNeighbour.Render ();
			Debug.Log ("Added " + thisNeighbour.topLeftCorner.x / chunkSizeX + ", " + thisNeighbour.topLeftCorner.y / chunkSizeY);
		}
		foreach (TileChunk distantChunk in distantChunks) {
			distantChunk.Remove ();
			Debug.Log ("Removed " + distantChunk.topLeftCorner.x / chunkSizeX + ", " + distantChunk.topLeftCorner.y / chunkSizeY);
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
			for (int x = 0; x < chunkSizeX; x++) {
				for (int y = 0; y < chunkSizeY; y++) {
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
