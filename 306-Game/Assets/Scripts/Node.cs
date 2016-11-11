using UnityEngine;
using System.Collections;

public class Node {

	public bool walkable;
	public Vector3 worldposition;
	public Node( bool _walkable, Vector3 _worldpos){
		walkable = _walkable;
		worldposition = _worldpos;
	}

}
