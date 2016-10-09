using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BuidingGenerator : MonoBehaviour {


	public static TileType[,] GenerateBuilding(Vector2 size){
		TileType[,] tileMap = new TileType[(int)size.x, (int)size.y];
		RoomNode root = new RoomNode (size);
		root.GenerateSubtree (0.5);
		return tileMap;
	}


	private class RoomNode {
		private Vector2 _size;
		private RoomNode _firstChild;
		private RoomNode _secondChild;
		private bool _verticalSplit;
		public int smallestRoomArea = 4;

		public RoomNode(Vector2 roomSize){
			_size = roomSize;
			_verticalSplit = false;
		}

		public void GenerateSubtree(double splitProb, double vertProb=0.5){
			int height = (int)_size.y;
			int width = (int)_size.x;
			int area = height * width;
			//randomly decide whether we're going to split this room into 2. never split small rooms
			if (Random.value <= splitProb && area > smallestRoomArea) {
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
