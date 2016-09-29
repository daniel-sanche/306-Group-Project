using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	/// <summary>
	/// The item to be picked up.
	/// </summary>
	public Item item;
	
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Player") {					//If a player collides with this
			if (Inventory.AddItem (item))						//Attempt to add the item to the inventory
				Destroy (this.gameObject);						//Delete this object if successful
		}
	}
}
