using UnityEngine;
using System.Collections;

public class Barricade : Item {

	//The points given the barricade
	public float barricadeRestore;

	// Use this for initialization
	void Start () {
		itemType = ItemType.BUILDING;
	}

	// Uses the item for it's given purpose.
	public virtual void Use(){

	}
}
