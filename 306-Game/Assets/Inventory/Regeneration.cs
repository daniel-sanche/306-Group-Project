using UnityEngine;
using System.Collections;

public class Regeneration : Item {

	//The amount of health this item gives when consumed
	public int healthRegen;

	//The amount of hunger points this item gives when consumed
	public int hungerRegen;

	// Use this for initialization
	void Start () {
		itemType = ItemType.REGENERATION;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public override void Use(){
		print (name + "\nThis is a regeneration item.");
	}
}
