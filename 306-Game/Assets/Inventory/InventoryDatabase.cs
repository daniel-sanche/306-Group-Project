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

		database = new List<GameObject>();							//Initializing the database

		Object[] items = Resources.LoadAll ("ItemDatabase");		//Loading all existing items from Resources folder
	
		for (int x = 0; x < items.Length; x++) {					//For each item
			GameObject cur = (GameObject)items [x];					//
			database.Add(cur);										//Add it to the database
		}
	}
	
	/// <summary>
	/// Returns the item to be found.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="toFind">The item to be found.</param>
	public static Item getItem(string toFind){
		foreach (GameObject cur in database) {
			if (cur.name == "toFind" && cur.GetComponent<Item>() != null)
				return cur.GetComponent<Item>();
		}

		return null;												//Returns null if the item could not be found
	}
}
