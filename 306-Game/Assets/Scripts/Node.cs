using UnityEngine;
using System.Collections;

public class Node {

	public int gcost;
	public int hcost;
	public int gridx, gridy;
	public Node parent;
	public bool walkable;
	public Vector3 worldposition;

	public Node( bool _walkable, Vector3 _worldpos, int _gridx, int _gridy){
		walkable = _walkable;
		worldposition = _worldpos;
		gridx = _gridx;
		gridy = _gridy;

	}

	public int fcost {
		get {
			return gcost+hcost;
		}
	}
}
