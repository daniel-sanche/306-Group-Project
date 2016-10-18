using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryDatabase : MonoBehaviour {

	/// <summary>
	/// All items currently in the inventory database.
	/// </summary>
	public static List<GameObject> database;

	// Use this for initialization
	void Start () {

		database = new List<GameObject>();								//Initializing the database

		Object[] items = Resources.LoadAll ("ItemDatabase");			//Loading all existing items from Resources folder
	
		for (int x = 0; x < items.Length; x++) {						//For each item
			GameObject cur = (GameObject)items [x];						//
			database.Add(cur);											//Add it to the database
		}
	}
	
	/// <summary>
	/// Returns the item to be found.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="toFind">The item to be found.</param>
	public static Item getItem(string toFind){
		foreach (GameObject cur in database) {							//Check each GameObject in the database 
			if (cur.name == "toFind") {									//If we have found a GameObject that matches the name
				if (cur.GetComponent<Item> () != null) {				//Cast if needed and return the found item
					return cur.GetComponent<Item> ();
				} else if (cur.GetComponent<Item> () != null) {
					return (Item)cur.GetComponent<Item> ();
				} else if (cur.GetComponent<Regeneration> () != null) {
					return (Item)cur.GetComponent<Regeneration> ();
				} else if (cur.GetComponent<Barricade> () != null) {
					return (Item)cur.GetComponent<Barricade> ();
				}
			}
		}

		return null;												//Returns null if the item could not be found
	}
}
