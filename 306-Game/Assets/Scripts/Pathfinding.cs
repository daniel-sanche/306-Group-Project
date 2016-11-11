using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {
	public Transform seeker, target;
	Grid grid;

	void Awake(){
		grid = GetComponent<Grid> ();
	}


	void FindPath(Vector3 startpos, Vector3 targetpos){
		Node startnode = grid.NodeFromWorldPoint (startpos);
		Node targetnode = grid.NodeFromWorldPoint (targetpos);
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
				RetracePath (startnode, targetnode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
				if (!neighbour.walkable || closedset.Contains (neighbour)) {
					continue;
				}
				int newmovementcosttoneighbour = currentNode.gcost + GetDistance (currentNode, neighbour);
				if (newmovementcosttoneighbour < neighbour.gcost || !openset.Contains(neighbour)){
					neighbour.gcost = newmovementcosttoneighbour;
					neighbour.hcost = GetDistance(neighbour,targetnode);
				
					neighbour.parent = currentNode;

					if (!openset.Contains (neighbour)) {
						openset.Add (neighbour);
					}
					else {
						openset.UpdateItem (neighbour);
					}
						 
				}
	 		}
		}
	}

	void Update(){
		FindPath (seeker.position, target.position);
	}

	void RetracePath( Node start, Node end){
		List<Node> path = new List<Node> ();
		Node currentNode = end;

		while (currentNode != start){
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();

		grid.path = path;
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
