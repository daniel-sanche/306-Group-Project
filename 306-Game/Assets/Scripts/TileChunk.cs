using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileChunk : MonoBehaviour {
	/**
	 * This class represents a group of tiles chunked together so they can be loaded/released from memory as a unit
	 * Each chunk represents approximately screen's visible region
	 * As the player moves around, the chunk in the screen's center will be the active chunk
	 * The active chunk will render it's neighbours so transition is seamless, and tell chunks further away to remove themselves from the view
	 **/


	private int [,] terrainMap;
	private List<GameObject> tileList;
	private Vector2 tilesPerChunk;
	private bool isRendered = false;


	private List<TileChunk> connectedChunks;
	private List<TileChunk> distantChunks;

	public GameObject grass;
	public GameObject gravel;

	/**
	 * Creates a new chunk of tiles
	 * terrain = array of tileIDs
	 * x = the x coordinate of the tile in the overall matrix of tiles
	 * y = the y coordinate of the tile in the overall matrix of tiles
	 */
	public void InitChunk(int [,] terrain, int x, int y, Vector2 tilesInChunk){
		terrainMap = terrain;
		tileList = new List<GameObject> ();
		connectedChunks = new List<TileChunk>();
		distantChunks = new List<TileChunk>();
		tilesPerChunk = tilesInChunk;
		isRendered = false;
	}

	/**
	 * Sets the chunk as active
	 * Should be called when the chunk is the one in the center of the screen
	 * Renders the chunk and it's neighbours, and tells further chunk to remove themselves from memory
	 **/
	public void Activate(){
		Render ();
		foreach (TileChunk thisNeighbour in connectedChunks) {
			thisNeighbour.Render ();
		}
		foreach (TileChunk distantChunk in distantChunks) {
			distantChunk.Remove ();
		}
	}


	/**
	 * Adds the chunk to the list of neighbours that are rendered when this chunk becomes active
	 * newChunk = the chunk to add to the list of neighbours
	 **/
	public void AddConnectedChunk(TileChunk newChunk){
		connectedChunks.Add (newChunk);
	}

	/**
	 * Adds the chunk to the list of distant chunks that are told to remove themselves when this chunk becomes active
	 * newChunk = the chunk to add to the list of distant chunks
	 **/
	public void AddDistantChunk(TileChunk newChunk){
		distantChunks.Add (newChunk);
	}

	/**
	 * render's the tiles in the chunk on screen
	 **/
	private void Render() {
		if (!isRendered) {
			tileList.Clear ();
			for (int x = 0; x < tilesPerChunk.x; x++) {
				for (int y = 0; y < tilesPerChunk.y; y++) {
					int code = terrainMap [x, y];
					GameObject groundTile = SpriteForCode (code);
					GameObject instance = Instantiate (groundTile, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
					tileList.Add (instance);
					instance.transform.SetParent (transform);
					instance.transform.localPosition = new Vector3 (x, y, 0); 
				}
			}
			isRendered = true;
			gameObject.SetActive(true);
		} else {
			gameObject.SetActive(true);
		}
	}

	/**
	 * removes the tiles rendered by this chunk from the screen
	 **/
	private void Remove () {
		gameObject.SetActive(false);
	}


	/**
	 * Converts between tile id's and the actual game objects they represent
	 * TileGenerator generates a 2D matrix of tile id's, but it's TileRenderer's job to turn them into actual tiles
	 * code = the id of the tile from the generator
	 **/
	public GameObject SpriteForCode(int code){
		GameObject groundTile;
		if (code == 0) {
			groundTile = grass;
		} else {
			groundTile = gravel;
		}
		return groundTile;
	}

}
