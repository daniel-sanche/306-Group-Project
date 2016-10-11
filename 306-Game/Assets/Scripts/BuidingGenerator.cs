using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BuidingGenerator : MonoBehaviour {


	/**
	 * Function to generate a building of a requested size
	 * size = a vector of the size of the building (width/height)
	 * minDoors = the minimum number of exterior doors the building should have
	 * roomSplitProb = the probability that any room will split in two
	 **/
	public static TileType[,] GenerateBuilding(Vector2 size, int minDoors=1, double roomSplitProb = 0.8){
		RoomNode root = new RoomNode (size);
		root.GenerateSubtree (roomSplitProb);
		TileType[,] tileMap = _InitializeTiles(root);
		_GenerateInnerWalls (root, tileMap);
		_GenerateInteriorDoors (root, tileMap);
		_GenerateExteriorDoors (tileMap, minDoors);
		return tileMap;
	}


	/**
	 * --------------------------------------------------------------------------------------------------------------------------------------------
	 * Helper Functions
	 * --------------------------------------------------------------------------------------------------------------------------------------------
	 **/

	/**
	 * Initializes the tilemap and adds part of the outer wall to the building
	 * Only the top and right walls are added, because the room generation will add the rest
	 * root = the root of the tree of rooms
	 */
	private static TileType[,] _InitializeTiles(RoomNode root){
		TileType[,] tileMap = new TileType[(int)root._size.x, (int)root._size.y];
		//visit each tile, filling in either blank floor space, or walls
		for (int x = 0; x < root._size.x; x++) {
			for (int y = 0; y < root._size.y; y++) {
				bool isTop = (y == (int)root._size.y - 1);
				bool isRight = (x == (int)root._size.x - 1);
				if (isTop) {
					tileMap [x, y] = TileType.FloorTop;
				} else if (isRight) {
					tileMap [x, y] = TileType.FloorRight;
				} else {
					tileMap [x, y] = TileType.Floor;
				}
			}
		}
		//set the corners to the proper tiles
		tileMap [0, (int)root._size.y-1] = TileType.FloorTL;
		tileMap [(int)root._size.x-1, 0] = TileType.FloorBR;
		tileMap [(int)root._size.x-1, (int)root._size.y-1] = TileType.FloorTR;
		return tileMap;
	}

	/**
	 * Adds walls between rooms to the building
	 * root = the current node of the building's tree we are working on
	 * tileMap = the pre-initialized array of tiles for the building
	 * xOffset = internal parameter to track how far from 0 the root room starts at in the x axis
	 * yOffset = internal parameter to track how far from 0 the root room starts at in the y axis
	 */
	private static void _GenerateInnerWalls(RoomNode root, TileType[,] tileMap, int xOffset=0, int yOffset=0){
		if (!root._isLeaf) {
			//not a leaf room node. Recurse on children instead
			if (root._verticalSplit) {
				//find children
				RoomNode leftSide = root._firstChild;
				RoomNode rightSide = root._secondChild;
				int leftWidth = (int)leftSide._size.x;
				//recurse
				_GenerateInnerWalls (leftSide, tileMap, xOffset, yOffset);
				_GenerateInnerWalls (rightSide, tileMap, xOffset + leftWidth, yOffset);
			} else {
				//find children
				RoomNode bottomSide = root._firstChild;
				RoomNode topSide = root._secondChild;
				int bottomHeight = (int)bottomSide._size.y;
				//recurse
				_GenerateInnerWalls (bottomSide, tileMap, xOffset, yOffset);
				_GenerateInnerWalls (topSide, tileMap, xOffset, yOffset + bottomHeight);
			}
		} else {
			//The node represents an individual room
			//add walls to the bottom and left edges

			//add bottom edge
			for(int x=xOffset; x< xOffset+root._size.x; x++){
				if (tileMap [x, yOffset] == TileType.FloorRight || tileMap [x, yOffset] == TileType.FloorBR) {
					//if the tile is already a right wall, make it a bottom right corner tile
					tileMap [x, yOffset] = TileType.FloorBR;
				} else {
					tileMap [x, yOffset] = TileType.FloorBottom;
				}
			}
			//add left edge
			for(int y=yOffset; y< yOffset+root._size.y; y++){
				if (tileMap [xOffset, y] == TileType.FloorTop || tileMap [xOffset, y] == TileType.FloorTL) {
					//if the tile is already a top wall, make it a top left corner tile
					tileMap [xOffset, y] = TileType.FloorTL;
				} else {
					tileMap [xOffset, y] = TileType.FloorLeft;
				}
			}
			tileMap [xOffset, yOffset] = TileType.FloorBL;
		}
	}

	/**
	 * Add doors and empty spaces connecting the interior rooms
	 * The algorithm works by looking at every level in the tree that is not a leaf, 
	 * and adding a door somewhere in the dividing line between regions. In this way, 
	 * every room will be connected and accessible
	 * 
	 * root = the current node of the building tree we are working on
	 * tileMap = the pre-initialized array of tiles for the building (with rooms generated)
	 * xOffset = internal parameter to track how far from 0 the root room starts at in the x axis
	 * yOffset = internal parameter to track how far from 0 the root room starts at in the y axis
	 * probEmptySpace = the probability that the connection between rooms is an empty floor tile, rather than a door
	 */
	private static void _GenerateInteriorDoors(RoomNode root, TileType[,] tileMap, int xOffset=0, int yOffset=0, double probEmptySpace=0.5){
		//if the node is a leaf, we can ignore it, because it didn't produce any walls inside itself
		if (!root._isLeaf) {
			//logic is similar, but different for horizontal splits
			if (root._verticalSplit) {
				RoomNode leftSide = root._firstChild;
				RoomNode rightSide = root._secondChild;
				int leftWidth = (int)leftSide._size.x;
				//find a random point om the vertical line separating the two child regions for a door
				int doorSpace = (int)(Random.value * root._size.y + yOffset);
				//if the tile at this spot is a corner, remove the vertical component of it
				if (tileMap [leftWidth + xOffset, doorSpace] == TileType.FloorTL ||
					tileMap [leftWidth + xOffset, doorSpace] == TileType.FloorTR) {
					tileMap [leftWidth + xOffset, doorSpace] = TileType.FloorTop;
				} else if (tileMap [leftWidth + xOffset, doorSpace] == TileType.FloorBL ||
					tileMap [leftWidth + xOffset, doorSpace] == TileType.FloorBR) {
					tileMap [leftWidth + xOffset, doorSpace] = TileType.FloorBottom;
				} else {
					//add either a door or an empty floor tile here to connect the regions
					if (Random.value < probEmptySpace) {
						tileMap [leftWidth + xOffset, doorSpace] = TileType.Floor;
					} else {
						tileMap [leftWidth + xOffset, doorSpace] = TileType.FloorDoorL;
					}
				}
				//recurse on children
				_GenerateInteriorDoors (leftSide, tileMap, xOffset, yOffset, probEmptySpace);
				_GenerateInteriorDoors (rightSide, tileMap, xOffset + leftWidth, yOffset, probEmptySpace);
			} else {
				RoomNode bottomSide = root._firstChild;
				RoomNode topSide = root._secondChild;
				int bottomHeight = (int)bottomSide._size.y;
				//find a random point om the vertical line separating the two child regions for a door
				int doorSpace = (int)(Random.value * root._size.x + xOffset);
				//if the tile at this spot is a corner, remove the horizontal component of it
				if (tileMap[doorSpace, bottomHeight + yOffset] == TileType.FloorTL ||
					tileMap[doorSpace, bottomHeight + yOffset] == TileType.FloorBL) {
					tileMap[doorSpace, bottomHeight + yOffset] = TileType.FloorLeft;
				} else if (tileMap[doorSpace, bottomHeight + yOffset] == TileType.FloorBR ||
					tileMap[doorSpace, bottomHeight + yOffset] == TileType.FloorTR) {
					tileMap[doorSpace, bottomHeight + yOffset] = TileType.FloorRight;
				} else {
					//add either a door or an empty floor tile here to connect the regions
					if (Random.value < probEmptySpace) {
						tileMap [doorSpace, bottomHeight + yOffset] = TileType.Floor;
					} else {
						tileMap [doorSpace, bottomHeight + yOffset] = TileType.FloorDoorB;
					}
				}
				//recurse on children
				_GenerateInteriorDoors (bottomSide, tileMap, xOffset, yOffset, probEmptySpace);
				_GenerateInteriorDoors (topSide, tileMap, xOffset, yOffset + bottomHeight, probEmptySpace);
			}
		}
	}

	/**
	 * Generates the doors leading in/out of the building
	 * The algorithm works by traversing over each door, with the probability of adding a door increasing as it goes.
	 * When it adds a door or encounters an existing one, the probability of adding a door drops to it's initial value, 
	 * to make it rare to add doors side by side. The algorithm keeps traversing over the edges until it creates the 
	 * min number of doors, or times out
	 * 
	 * tileMap = the pre-initialized array of tiles for the building (with rooms/interior doors generated)
	 * minDoors = the minimum number of doors to add to the building
	 * initialProb = the initial probability of a door being added. 
	 * 				 Should be small, because this will be the value of adding a door right after the previous one
	 * growthFactor = the value that will scale the initialProb value after each step, assuming a door has not been added yet
	 * maxTries = how many times the algorithm should visit each edge. If it can't find a set of doors before this many tries, it will give up
	 */
	private static void _GenerateExteriorDoors(TileType[,] tileMap, int minDoors=1, double initialProb = 0.01, double growthFactor = 1.5, int maxTries=1000){
		int doorsAdded = 0;
		int tries = 0;
		double topProb = initialProb;
		double bottomProb = initialProb;
		double leftProb = initialProb;
		double rightProb = initialProb;
		do {
			//visit the tiles on the top and bottom edges
			int width = tileMap.GetLength(0);
			for (int x=0; x<width; x++){
				//add doors to the top row
				if(tileMap[x, width-1] == TileType.FloorTop && Random.value < topProb){
					//if the random value is passed and it's on a regular wall, add a door
					tileMap[x, width-1] = TileType.FloorDoorT;
					//reset probability of next tile being a door to initaial small value
					topProb = initialProb;
					doorsAdded++;
				} else if(tileMap[x, width-1] == TileType.FloorDoorT){
					//encountered an old door; reset probability of next tile being a door to initaial small value
					//this is to avoid adding multiple doors close together
					topProb = initialProb;
				} else {
					//if no door was added, grow the probability of the next one by the growthFactor
					topProb = topProb * growthFactor;
				}
				//add doors to bottom row
				if(tileMap[x, 0] == TileType.FloorBottom && Random.value < bottomProb){
					tileMap[x, 0] = TileType.FloorDoorB;
					bottomProb = initialProb;
					doorsAdded++;
				} else if(tileMap[x, 0] == TileType.FloorDoorB){
					bottomProb = initialProb;
				} else {
					bottomProb = bottomProb * growthFactor;
				}
			}
			//iterate left and right columns
			int height = tileMap.GetLength(1);
			for (int y=0; y<height; y++){
				//add doors to left column
				if(tileMap[0,y] == TileType.FloorLeft && Random.value < leftProb){
					tileMap[0, y] = TileType.FloorDoorL;
					leftProb = initialProb;
					doorsAdded++;
				} else if(tileMap[0, y] == TileType.FloorDoorL){
					leftProb = initialProb;
				} else {
					leftProb = leftProb * growthFactor;
				}
				//add doors to right column
				if(tileMap[height-1,y] == TileType.FloorRight && Random.value < rightProb){
					tileMap[height-1, y] = TileType.FloorDoorR;
					rightProb = initialProb;
					doorsAdded++;
				} else if(tileMap[height-1, y] == TileType.FloorDoorR){
					rightProb = initialProb;
				} else {
					rightProb = rightProb * growthFactor;
				}
			}
			tries++;
		//loop until we either have enough doors, or we've tried too many times and are ready to give up
		} while(doorsAdded < minDoors && tries < maxTries);
	}
		
	/**
	 * --------------------------------------------------------------------------------------------------------------------------------------------
	 * Helper Class RoomNode
	 * This class is used for creating a tree of rooms, styled like a k-d tree
	 * The root node represents the entire building, the 2nd layer represents the building split in half, etc
	 * In this way, we can generate intricate rooms in a simple manner
	 * --------------------------------------------------------------------------------------------------------------------------------------------
	 **/

	private class RoomNode {
		public Vector2 _size;
		public RoomNode _firstChild;
		public RoomNode _secondChild;
		public bool _verticalSplit = false;
		public bool _isLeaf = true;

		/**
		 * Create a new RoomNode
		 * roomSize = the size of the room
		 */
		public RoomNode(Vector2 roomSize){
			_size = roomSize;
		}

		/**
		 * Creates the children rooted at this node
		 * potentially splits the room in half, and then recurses on it's children, potentially splitting them in half
		 * splitProb = the probability that the current node will split
		 * vertProb = the probability that the split will be a vertical line through the room (otherwise horizontal)
		 */
		public void GenerateSubtree(double splitProb=0.5, double vertProb=0.5){
			//4 is the smallest value for any dimension, because of the way our tiles are set up. We need to leave a buffer
			int dimMin = 4;
			int height = (int)_size.y;
			int width = (int)_size.x;

			//randomly decide whether we're going to split this room into 2. never split small rooms
			if (Random.value <= splitProb && (height >= dimMin || width >= dimMin)) {
				_isLeaf = false;
				//make it so rooms small in one dimension cannot be split in that direction
				if (height < dimMin) {
					vertProb = 1;
				} else if (width < dimMin) {
					vertProb = -1;
				}
				//calculate whether we're doing a vertical split of a horizontal one
				Vector2 firstSize = _size;
				Vector2 secondSize  = _size;
				if (Random.value <= vertProb) {
					//do a vertical split
					//find a split point on the x axis that's not an edge
					int splitPt = (int)(Random.value * (width - dimMin)) + 2;
					//adjust the child sizes to reflect split
					firstSize.x = splitPt;
					secondSize.x = _size.x - splitPt;
					_verticalSplit = true;
				} else {
					//do a horizontal split
					//find a split point on the y axis that's not an edge
					int splitPt = (int)(Random.value * (height - dimMin)) + 2;
					//adjust the child sizes to reflect split
					firstSize.y = splitPt;
					secondSize.y = _size.y - splitPt;
					_verticalSplit = false;
				}
				//recurse on children
				_firstChild = new RoomNode (firstSize);
				_secondChild = new RoomNode (secondSize);
				_firstChild.GenerateSubtree (splitProb, vertProb);
				_secondChild.GenerateSubtree(splitProb, vertProb);
			}
		}
	}


}
