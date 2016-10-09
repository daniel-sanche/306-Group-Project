using UnityEngine;
using System.Collections;

public class BuidingGenerator : MonoBehaviour {

	GameObject topFloor = (GameObject)Resources.Load("/Prefabs/top");


	public static TileType[,] GenerateBuilding(Vector2 size){
		TileType[,] tileMap = new TileType[(int)size.x, (int)size.y];
		CreateBorder (tileMap, size);
		return tileMap;
	}

	private static void CreateBorder(TileType[,] tileMap, Vector2 size){
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				bool isBottom = (y == 0);
				bool isTop = (y == (int)size.y - 1);
				bool isLeft = (x == 0);
				bool isRight = (x == (int)size.x - 1);
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
		tileMap [0, (int)size.y-1] = TileType.FloorTL;
		tileMap [(int)size.x-1, 0] = TileType.FloorBR;
		tileMap [(int)size.x-1, (int)size.y-1] = TileType.FloorTR;
	}
}
