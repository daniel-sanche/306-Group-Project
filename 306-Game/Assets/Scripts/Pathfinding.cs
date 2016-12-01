using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
/* Credit is due to Sebastian Lague's tutorial on https://www.youtube.com/watch?v=-L-WgKMFuhE&t=2s*/

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;


	//awake
	void Awake(){
		grid = GetComponent<Grid> ();
		requestManager = GetComponent <PathRequestManager >();
	}

	public void StartFindPath(Vector3 startpos, Vector3 targetPos){
		StartCoroutine(FindPath(startpos ,targetPos ));	

	}


	IEnumerator FindPath(Vector3 startpos, Vector3 targetpos){
		Node startnode = grid.NodeFromWorldPoint (startpos);
		Node targetnode = grid.NodeFromWorldPoint (targetpos);
		Vector3[] waypoints = new Vector3 [0];
		bool pathSuccess =false;

		if (startnode.walkable && targetnode.walkable) {
			
			HashSet<Node> closedset = new HashSet<Node> ();
			//List<Node> openset = new List<Node> ();
			Heap<Node> openset = new Heap<Node> (grid.maxSize);

			openset.Add (startnode);
			while (openset.Count > 0) {
				/*Node currentNode = openset [0];
			for (int i = 1; i < openset.Count; i++) {
				if (openset [i].fcost < currentNode.fcost || openset[i].fcost == currentNode.fcost && openset[i].hcost<currentNode.hcost) {
					currentNode = openset [i];
				}
			}

			openset.Remove (currentNode);
			*/
				Node currentNode = openset.RemoveFirst ();
				closedset.Add (currentNode);

				if (currentNode == targetnode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedset.Contains (neighbour)) {
						continue;
					}
					int newmovementcosttoneighbour = currentNode.gcost + GetDistance (currentNode, neighbour);
					if (newmovementcosttoneighbour < neighbour.gcost || !openset.Contains (neighbour)) {
						neighbour.gcost = newmovementcosttoneighbour;
						neighbour.hcost = GetDistance (neighbour, targetnode);
				
						neighbour.parent = currentNode;

						if (!openset.Contains (neighbour)) {
							openset.Add (neighbour);
						}

					}
				}
			}
		}

		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath (startnode, targetnode);
		}
		requestManager.finishedProcessingPath (waypoints, pathSuccess);

	}



	 Vector3[] RetracePath( Node start, Node end){
		List<Node> path = new List<Node> ();
		Node currentNode = end;

		while (currentNode != start){
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path ){
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridx - path [i].gridx, path [i - 1].gridy - path [i].gridy);
			if (directionNew != directionOld) {
				waypoints.Add (path [i].worldposition);
			}
			directionOld = directionNew;

		}
		return waypoints.ToArray ();
	}

	int GetDistance( Node a, Node b){
		int dstx = Mathf.Abs (a.gridx - b.gridx);
		int dsty = Mathf.Abs (a.gridy - b.gridy);

		if (dstx > dsty) {
			return 14 * dsty + 10* (dstx - dsty);
		}
		return 14 * dstx + 10 * (dsty - dstx);

	}

}
