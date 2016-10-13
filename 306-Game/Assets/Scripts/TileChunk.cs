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


	private TileType [,] terrainMap;
	private List<GameObject> tileList;
	private Vector2 tilesPerChunk;
	private bool isCached = false;


	private List<TileChunk> connectedChunks;
	private List<TileChunk> distantChunks;

	public GameObject grass;
	public GameObject gravel;
	public GameObject floor;
	public GameObject floorTop;
	public GameObject floorBottom;
	public GameObject floorLeft;
	public GameObject floorRight;
	public GameObject floorTL;
	public GameObject floorTR;
	public GameObject floorBL;
	public GameObject floorBR;
	public GameObject floorDoorL;
	public GameObject floorDoorR;
	public GameObject floorDoorT;
	public GameObject floorDoorB;

	public float cacheClearTime = (5f*60f);

	/**
	 * Creates a new chunk of tiles
	 * terrain = array of tileIDs
	 * x = the x coordinate of the tile in the overall matrix of tiles
	 * y = the y coordinate of the tile in the overall matrix of tiles
	 */
	public void InitChunk(TileType [,] terrain, int x, int y, Vector2 tilesInChunk){
		terrainMap = terrain;
		tilesPerChunk = tilesInChunk;
	}

	void Awake(){
		isCached = false;
		gameObject.SetActive(false);
		tileList = new List<GameObject> ();
		connectedChunks = new List<TileChunk>();
		distantChunks = new List<TileChunk>();
	}

	/**
	 * Sets the chunk as active
	 * Should be called when the chunk is the one in the center of the screen
	 * Renders the chunk and it's neighbours, and tells further chunk to remove themselves from memory
	 **/
	public void Activate(){
		DisplayTiles ();
		foreach (TileChunk thisNeighbour in connectedChunks) {
			thisNeighbour.DisplayTiles ();
		}
		foreach (TileChunk distantChunk in distantChunks) {
			distantChunk.HideTiles ();
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
	 * render's the tiles in the chunk on screen. 
	 * If the tiles were already saved in a cache, just set the chunk as active so they are rendered
	 **/
	private void DisplayTiles() {
		if (!isCached) {
			tileList.Clear ();
			for (int x = 0; x < tilesPerChunk.x; x++) {
				for (int y = 0; y < tilesPerChunk.y; y++) {
					TileType code = terrainMap [x, y];
					GameObject groundTile = SpriteForCode (code);
					GameObject instance = Instantiate (groundTile, Vector3.zero, Quaternion.identity) as GameObject;
					tileList.Add (instance);
					instance.transform.SetParent (transform);
					instance.transform.localPosition = new Vector3 (x, y, 0); 
				}
			}
			isCached = true;
			gameObject.SetActive(true);
		} else {
			gameObject.SetActive(true);
		}
	}

	/**
	 * hides the tiles rendered on screen
	 * If the tiles aren't in use after a set amount of time, clear the cache of the tiles in memory
	 **/
	private void HideTiles () {
		gameObject.SetActive(false);
		Invoke ("ClearCache", cacheClearTime);
	}

	/**
	 * Function to clear the cache of tile objects from memory
	 * The next time they need to be rendered, they will have to be created from scratch
	 */
	private void ClearCache(){
		if (!gameObject.activeInHierarchy) {
			foreach (GameObject thisTile in tileList) {
				GameObject.Destroy (thisTile);
			}
			tileList.Clear ();
			isCached = false;
		} else {
			Invoke ("ClearCache", cacheClearTime);
		}
	}


	/**
	 * Converts between tile id's and the actual game objects they represent
	 * TileGenerator generates a 2D matrix of tile id's, but it's TileRenderer's job to turn them into actual tiles
	 * code = the id of the tile from the generator
	 **/
	public GameObject SpriteForCode(TileType code){
		switch (code) 
		{
		case TileType.Grass:
			return grass;
		case TileType.Gravel:
			return gravel;
		case TileType.Floor:
			return floor;
		case TileType.FloorTop:
			return floorTop;
		case TileType.FloorBottom:
			return floorBottom;
		case TileType.FloorLeft:
			return floorLeft;
		case TileType.FloorRight:
			return floorRight;
		case TileType.FloorBL:
			return floorBL;
		case TileType.FloorBR:
			return floorBR;
		case TileType.FloorTL:
			return floorTL;
		case TileType.FloorTR:
			return floorTR;
		case TileType.FloorDoorB:
			return floorDoorB;
		case TileType.FloorDoorT:
			return floorDoorT;
		case TileType.FloorDoorL:
			return floorDoorL;
		case TileType.FloorDoorR:
			return floorDoorR;
		default:
			return grass;
		}
	}

}
