using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	public Vector2 gridWorldSize;
	public LayerMask unwalkablemask;
	public float nodeRadius;
	public Transform player;
	Node[,] grid;
	public bool displayGridGizmos;
	float nodeDiameter;
	int gridsizex, gridsizey;

	void Awake(){
		nodeDiameter = nodeRadius * 2;
		gridsizex = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridsizey = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);
		CreateGrid ();
	}

	public int maxSize {
		get {
			return gridsizex * gridsizey;
		}
	}
	void CreateGrid(){
		grid = new Node[gridsizex, gridsizey];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.up * gridWorldSize.y/2;
		for (int x = 0; x < gridsizex; x++) {
			for (int y = 0; y < gridsizey; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x *nodeDiameter + nodeRadius) + Vector3.up * (y* nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkablemask));
				grid[x,y] = new Node(walkable, worldPoint, x , y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node){
		List<Node> neighbours = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
				/* If we want diagonal lines
				if (x == 0 && y == 0 || Mathf.Abs(x) + Mathf.Abs(y) == 2)﻿{*/
					continue;
				}
				int checkx = node.gridx + x;
				int checky = node.gridy + y;
				if (checkx >= 0 && checkx < gridsizex && checky >= 0 && checky < gridsizey) {
					neighbours.Add (grid [checkx, checky]);
				}

			}
		}
		return neighbours;
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition){
		float percentx = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percenty = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
		percentx = Mathf.Clamp01 (percentx);
		percenty = Mathf.Clamp01 (percenty);

		int x = Mathf.RoundToInt((gridsizex - 1) * percentx);
		int y = Mathf.RoundToInt((gridsizey - 1) * percenty);

		return grid [x, y];

	}


	void OnDrawGizmos(){
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, gridWorldSize.y,1 ));
		if (grid != null && displayGridGizmos ) {
			foreach (Node n in grid) {
				
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube (n.worldposition, Vector3.one * (nodeDiameter-.01f));
			}
	
		}

	}
}
