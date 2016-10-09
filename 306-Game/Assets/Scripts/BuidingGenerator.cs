using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BuidingGenerator : MonoBehaviour {


	public static TileType[,] GenerateBuilding(Vector2 size){
		RoomNode root = new RoomNode (size);
		root.GenerateSubtree (0.5);
		return GenerateTiles (root);
	}

	private static TileType[,] GenerateTiles(RoomNode root){
		TileType[,] tileMap = GenerateOuterWalls(root);
		return tileMap;
	}

	private static TileType[,] GenerateOuterWalls(RoomNode root){
		TileType[,] tileMap = new TileType[(int)root._size.x, (int)root._size.y];
		for (int x = 0; x < root._size.x; x++) {
			for (int y = 0; y < root._size.y; y++) {
				bool isBottom = (y == 0);
				bool isTop = (y == (int)root._size.y - 1);
				bool isLeft = (x == 0);
				bool isRight = (x == (int)root._size.x - 1);
				if (isTop) {
					tileMap [x, y] = TileType.FloorTop;
				} else if (isBottom) {
					tileMap [x, y] = TileType.FloorBottom;
				} else if (isRight) {
					tileMap [x, y] = TileType.FloorRight;
				} else if (isLeft) {
					tileMap [x, y] = TileType.FloorLeft;
				} else {
					tileMap [x, y] = TileType.Floor;
				}
			}
		}
		tileMap [0, 0] = TileType.FloorBL;
		tileMap [0, (int)root._size.y-1] = TileType.FloorTL;
		tileMap [(int)root._size.x-1, 0] = TileType.FloorBR;
		tileMap [(int)root._size.x-1, (int)root._size.y-1] = TileType.FloorTR;
		return tileMap;
	}

	private class RoomNode {
		public Vector2 _size;
		private RoomNode _firstChild;
		private RoomNode _secondChild;
		private bool _verticalSplit = false;
		private bool _isLeaf = true;
		public int smallestRoomArea = 4;

		public RoomNode(Vector2 roomSize){
			_size = roomSize;
		}

		public void GenerateSubtree(double splitProb, double vertProb=0.5){
			int height = (int)_size.y;
			int width = (int)_size.x;
			int area = height * width;
			//randomly decide whether we're going to split this room into 2. never split small rooms
			if (Random.value <= splitProb && area > smallestRoomArea) {
				_isLeaf = false;
				//make it so rooms small in one dimension cannot be split in that direction
				if (height <= 2) {
					vertProb = 1;
				} else if (width <= 2) {
					vertProb = -1;
				}
				//calculate whether we're doing a vertical split of a horizontal one
				Vector2 firstSize = _size;
				Vector2 secondSize  = _size;
				if (Random.value <= vertProb) {
					//do a vertical split
					//find a split point on the x axis that's not an edge
					int splitPt = (int)(Random.value * (width - 2)) + 1;
					//adjust the child sizes to reflect split
					firstSize.x = splitPt;
					secondSize.x = _size.x - splitPt;
					_verticalSplit = true;
				} else {
					//do a horizontal split
					//find a split point on the y axis that's not an edge
					int splitPt = (int)(Random.value * (height - 2)) + 1;
					//adjust the child sizes to reflect split
					firstSize.y = splitPt;
					secondSize.y = _size.x - splitPt;
					_verticalSplit = false;
				}
				_firstChild = new RoomNode (firstSize);
				_secondChild = new RoomNode (secondSize);
				_firstChild.GenerateSubtree (0.5);
				_secondChild.GenerateSubtree(0.5);
			}
		}
	}


}
