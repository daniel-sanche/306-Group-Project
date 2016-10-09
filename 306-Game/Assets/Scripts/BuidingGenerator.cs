using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BuidingGenerator : MonoBehaviour {


	public static TileType[,] GenerateBuilding(Vector2 size){
		RoomNode root = new RoomNode (size);
		root.GenerateSubtree (0.5, 0.5);
		return GenerateTiles (root);
	}

	private static TileType[,] GenerateTiles(RoomNode root){
		TileType[,] tileMap = GenerateOuterWalls(root);
		GenerateInnerWalls (root, tileMap);
		return tileMap;
	}

	private static void GenerateInnerWalls(RoomNode root, TileType[,] tileMap, int xOffset=0, int yOffset=0){
		if (!root._isLeaf) {
			if (root._verticalSplit) {
				//add walls along the vertical wall
				RoomNode leftSide = root._firstChild;
				RoomNode rightSide = root._secondChild;
				int leftWidth = (int)leftSide._size.x;
				GenerateInnerWalls (leftSide, tileMap, xOffset, yOffset);
				GenerateInnerWalls (rightSide, tileMap, xOffset + leftWidth, yOffset);
			} else {
				//add walls along the horizontal wall
				RoomNode bottomSide = root._firstChild;
				RoomNode topSide = root._secondChild;
				int bottomHeight = (int)bottomSide._size.y;
				GenerateInnerWalls (bottomSide, tileMap, xOffset, yOffset);
				GenerateInnerWalls (topSide, tileMap, xOffset, yOffset + bottomHeight);
			}
		} else {
			//add bottom row
			for(int x=xOffset; x< xOffset+root._size.x; x++){
				if (tileMap [x, yOffset] == TileType.FloorRight || tileMap [x, yOffset] == TileType.FloorBR) {
					tileMap [x, yOffset] = TileType.FloorBR;
				} else {
					tileMap [x, yOffset] = TileType.FloorBottom;
				}
			}
			//add left row
			for(int y=yOffset; y< yOffset+root._size.y; y++){
				if (tileMap [xOffset, y] == TileType.FloorTop || tileMap [xOffset, y] == TileType.FloorTL) {
					tileMap [xOffset, y] = TileType.FloorTL;
				} else {
					tileMap [xOffset, y] = TileType.FloorLeft;
				}
			}
			tileMap [xOffset, yOffset] = TileType.FloorBL;
		}
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
				} else if (isRight) {
					tileMap [x, y] = TileType.FloorRight;
				} else {
					tileMap [x, y] = TileType.Floor;
				}
			}
		}
		tileMap [0, (int)root._size.y-1] = TileType.FloorTL;
		tileMap [(int)root._size.x-1, 0] = TileType.FloorBR;
		tileMap [(int)root._size.x-1, (int)root._size.y-1] = TileType.FloorTR;
		return tileMap;
	}

	private class RoomNode {
		public Vector2 _size;
		public RoomNode _firstChild;
		public RoomNode _secondChild;
		public bool _verticalSplit = false;
		public bool _isLeaf = true;

		public RoomNode(Vector2 roomSize){
			_size = roomSize;
		}

		public void GenerateSubtree(double splitProb, double vertProb=0.5){
			int height = (int)_size.y;
			int width = (int)_size.x;
			//randomly decide whether we're going to split this room into 2. never split small rooms
			if (Random.value <= splitProb && (height >= 4 || width >= 4)) {
				//make it so rooms small in one dimension cannot be split in that direction
				if (height < 4) {
					vertProb = 1;
				} else if (width < 4) {
					vertProb = -1;
				}
				_isLeaf = false;
				//calculate whether we're doing a vertical split of a horizontal one
				Vector2 firstSize = _size;
				Vector2 secondSize  = _size;
				if (Random.value <= vertProb) {
					//do a vertical split
					//find a split point on the x axis that's not an edge
					int splitPt = (int)(0.5 * (width - 4)) + 2;
					//adjust the child sizes to reflect split
					firstSize.x = splitPt;
					secondSize.x = _size.x - splitPt;
					_verticalSplit = true;
				} else {
					//do a horizontal split
					//find a split point on the y axis that's not an edge
					int splitPt = (int)(0.5 * (height - 4)) + 2;
					//adjust the child sizes to reflect split
					firstSize.y = splitPt;
					secondSize.y = _size.y - splitPt;
					_verticalSplit = false;
				}
				_firstChild = new RoomNode (firstSize);
				_secondChild = new RoomNode (secondSize);
				_firstChild.GenerateSubtree (splitProb, vertProb);
				_secondChild.GenerateSubtree(splitProb, vertProb);
			}
		}
	}


}
