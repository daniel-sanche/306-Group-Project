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
	private List<GameObject> moveableObjects;
	private Vector2 tilesPerChunk;
	private bool isCached = false;


	private List<TileChunk> connectedChunks;
	private List<TileChunk> distantChunks;

	public GameObject grass;
	public GameObject gravel;
	public GameObject water;
	public GameObject sand;
	public GameObject rock;
	public GameObject tree;

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

	public GameObject quickshot;
	public GameObject poison;
	public GameObject health;
	public GameObject energy;
	public GameObject club;
	public GameObject mallet;
	public GameObject slingshot;
	public GameObject sword;

	public GameObject[] itemPrefabs;
	public float[] itemSpawnChances;
	public GameObject enemy;

	public float cacheClearTime = (5f*60f);

	//the probability of an item being added to an individual tile
	public float itemProbGround = 0.01f;
	public float itemProbBuilding = 0.2f;
	//the probability of an enemy being added to a chunk
	public float enemyProb = 0.1f;

	public float quickshotProb = 1;
	public float poisonProb = 1;
	public float healthProb = 5;
	public float energyProb = 5;
	public float clubProb = 1;
	public float malletProb = 1;
	public float slingshotProb = 1;
	public float swordProb = 1;

	private bool generateNewItems = true;
	private bool generateNewEnemy = true;
	public Vector2 offset;

	/**
	 * Creates a new chunk of tiles
	 * terrain = array of tileIDs
	 * x = the x coordinate of the tile in the overall matrix of tiles
	 * y = the y coordinate of the tile in the overall matrix of tiles
	 */
	public void InitChunk(TileType [,] terrain, int x, int y, Vector2 tilesInChunk){
		terrainMap = terrain;
		tilesPerChunk = tilesInChunk;
		offset = new Vector2 (x * tilesPerChunk.x, y * tilesPerChunk.y);
		moveableObjects = new List<GameObject> ();
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
		//add items if necessary
		//items will be randomly added when the chunk is first discovered, and possibly after a night cycle
		if (generateNewItems) {
			generateNewItems = false;
			List<Vector2> openGround = GetSpaces (new TileType[]{TileType.Grass, TileType.Sand, TileType.Gravel});
			AddNewItems (itemProbGround, openGround);
			List<Vector2> openBuildings = GetSpaces (new TileType[]{TileType.Floor, TileType.FloorBottom, TileType.FloorTop,
					TileType.FloorLeft, TileType.FloorRight, TileType.FloorBL, TileType.FloorBR, TileType.FloorTL, TileType.FloorTR});
			AddNewItems (itemProbBuilding, openBuildings);
		}
		if (generateNewEnemy) {
			generateNewEnemy = false;
			if (Random.value < enemyProb) {
				List<Vector2> openGround = GetSpaces (new TileType[]{TileType.Grass, TileType.Sand, TileType.Gravel});
				AddNewEnemies (Mathf.Max((int)Mathf.Round(enemyProb), 1), openGround);
			}
		}
		if (!isCached) {
			tileList.Clear ();
			for (int x = 0; x < tilesPerChunk.x; x++) {
				for (int y = 0; y < tilesPerChunk.y; y++) {
					TileType code = terrainMap [x, y];
					TileType l = TileType.NULL;
					TileType r = TileType.NULL;
					TileType t = TileType.NULL;
					TileType b = TileType.NULL;
					if (x > 0) {
						l = terrainMap [x - 1, y];
					} else if (x < tilesPerChunk.x - 1) {
						r = terrainMap [x + 1, y];
					}
					if (y > 0) {
						b = terrainMap [x, y - 1];
					} else if (x < tilesPerChunk.x - 1) {
						t = terrainMap [x, y + 1];
					}
					GameObject groundTile = SpriteForCode (code, left: l, right: r, top: t, bottom: b);
					GameObject instance = Instantiate (groundTile, Vector3.zero, Quaternion.identity) as GameObject;
					tileList.Add (instance);
					instance.transform.SetParent (transform);
					instance.transform.localPosition = new Vector3 (x, y, 0); 
				}
			}
			isCached = true;
		}
		foreach (GameObject obj in moveableObjects) {
			obj.SetActive (true);
		}
		moveableObjects.Clear ();
		gameObject.SetActive(true);

	}

	/**
	 * hides the tiles rendered on screen
	 * If the tiles aren't in use after a set amount of time, clear the cache of the tiles in memory
	 **/
	private void HideTiles () {
		gameObject.SetActive(false);
		Invoke ("ClearCache", cacheClearTime);
		//grab a reference to all moveable objects currently on the tile. 
		//We will unload them from memory, and reload them if the tile is reactivated
		Vector2 otherCorner = new Vector2(offset.x+tilesPerChunk.x, offset.y+tilesPerChunk.y);
		Collider2D[] colliderList = Physics2D.OverlapAreaAll (offset, otherCorner);
		foreach (Collider2D col in colliderList) {
			GameObject obj = col.gameObject;
			if (obj.tag == "Item" || obj.tag == "Enemy") {
				obj.SetActive (false);
				moveableObjects.Add (obj);
			}
		}
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
	 * left,right,top,bottom = the tiles surrounding this one, to use for context information
	 **/
	public GameObject SpriteForCode(TileType code,	TileType left=TileType.NULL, TileType right = TileType.NULL, 
									TileType top = TileType.NULL, TileType bottom = TileType.NULL){
		switch (code) 
		{
		case TileType.Grass:
			return grass;
		case TileType.Gravel:
			return gravel;
		case TileType.Water:
			return water;
		case TileType.Sand:
			return sand;
		case TileType.Rock:
			return rock;
		case TileType.Tree:
			return tree;
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

	/**
	 * Finds a list of tiles of a certain type
	 */
	public List<Vector2> GetSpaces(TileType[] requestedTileArr){
		List<Vector2> openList = new List<Vector2> ();
		for (int x = 0; x < tilesPerChunk.x; x++) {
			for (int y = 0; y < tilesPerChunk.y; y++) {
				TileType code = terrainMap [x, y];
				foreach(TileType thisAllowedTile in requestedTileArr){
					if (code == thisAllowedTile) {
						openList.Add(new Vector2(x, y));
					}
				}
			}
		}
		return openList;
	}

	/**
	 * Returns a random item based on the probabilities assigned to each one
	 */
	private GameObject _randomItem(){
		float[] probArr = itemSpawnChances;
		GameObject[] objArr = itemPrefabs;
		float sum = 0;
		foreach (float thisProb in probArr) {
			sum = sum + thisProb;
		}
		float randVal = Random.Range (0, sum);
		for (int i = 0; i < probArr.Length; i++) {
			float thisProb = probArr [i];
			if (randVal < thisProb) {
				return objArr [i];
			}
			randVal = randVal - thisProb;
		}
		return club;
	}

	/**
	 * adds new items to the list of spaces passed in, based on the probability passed in
	 */
	private void AddNewItems(float itemProb, List<Vector2>availableSpaces){
		for (int i = 0; i < availableSpaces.Count; i = i + 1) {
			if (Random.value < itemProb) {
				int randomIndex = Random.Range(0, availableSpaces.Count);
				Vector2 point = availableSpaces [randomIndex];
				GameObject pickUpPrefab = Resources.Load ("Pickup") as GameObject;
				pickUpPrefab.GetComponent<Pickup> ().item = _randomItem ().GetComponent<Item>();
				GameObject itemInstance = Instantiate (pickUpPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				itemInstance.transform.localPosition = new Vector3 (point.x+offset.x, point.y+offset.y, 0); 
			}
		}
	}

	/**
	 * Add enemies to the chunk
	 * Will add numToAdd enemies
	 */
	private void AddNewEnemies(int numToAdd, List<Vector2>availableSpaces){
		int randomIndex = Random.Range(0, availableSpaces.Count);
		Vector2 point = availableSpaces [randomIndex];
		GameObject itemInstance = Instantiate (enemy, Vector3.zero, Quaternion.identity) as GameObject;
		itemInstance.transform.localPosition = new Vector3 (point.x+offset.x, point.y+offset.y, 0); 
	}

}
