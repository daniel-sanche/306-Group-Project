using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	// Number of slots in inventory.
	public int size;

	// The number of rows (Must be greater than 0).
	public int rows;

	// The X and Y spacing between inventory slots.
	public float xSpacing, ySpacing;

	// Array of item slots.
	public static Slot[] itemSlot;

	public static Slot weaponSlot;

	// Prefab of an empty slot for instantiating.
	public GameObject slotPrefab;


	// Use this for initialization
	void Start () {
	
		itemSlot = new Slot[size];														//Initialize array

		for (int x = 0; x < size; x++) {												//For each x, instantiate a slot
			GameObject curSlot = (GameObject)Instantiate (slotPrefab, new Vector2((x % rows) * xSpacing, (x / rows) *  ySpacing) + (Vector2) transform.position, Quaternion.identity);		
			curSlot.transform.SetParent(this.transform);								//Set the parent of the slot to be the inventory
			curSlot.name = "Slot" + x;													//Name the slot with its current array index
			itemSlot[x] = curSlot.GetComponent<Slot>();									//Assign the new slot to an array index
		}

		GameObject weapSlot = (GameObject)Instantiate (slotPrefab, new Vector2((size % rows) * xSpacing, (size / rows) *  ySpacing) + (Vector2) transform.position, Quaternion.identity);	

		weapSlot.transform.SetParent (this.transform);
		weapSlot.name = "Weapon Slot";
		weaponSlot = weapSlot.GetComponent<Slot> ();
		weaponSlot.GetComponent<Image> ().color = Color.blue;
	}


	// Update is called once per frame
	void Update () {

	}

	/**
	* Performs a basic bubble sort of the inventory based on the name.
	**/
	public static void SortByName(){
		for (int x = 0; x < itemSlot.Length; x++) {										//For each index
			for (int y = 0; y < itemSlot.Length; y++) {									//Check each index
				if (itemSlot [x].item.name.CompareTo (itemSlot [y].item.name) < 0) {	//If the x slot's item is alphabetically ahead of y's item
					Item temp = itemSlot [x].getItem();									//
					itemSlot [x].setItem(itemSlot[y].getItem());						//Swap the two slots
					itemSlot [y].setItem (temp);										//
				}	
			}
		}
	}

	/**
	 * Is the inventory currently full?
	 **/
	public static bool isFull(){
		for (int x = 0; x < itemSlot.Length; x++) {
			if(itemSlot[x].isEmpty())
				return false;
		}

		return true;
	}

	/**
	* Adds the given item to the first available slot.
	*
	* True if successful, false if inventory is full.
	**/
	public static bool AddItem(Item toIns){
		
		for (int x = 0; x < itemSlot.Length; x++) {										//Check each slot
			if (itemSlot [x].isEmpty ()) {												//If the slot is empty
				itemSlot [x].setItem (toIns);											//Give the item to the slot
				return true;															//Return successful
			}
		}

		return false;																	//Return false if not successful
	}

	/// <summary>
	/// Finds the item by name, removes it from its slot and returns it
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="toFind">The item to be found.</param>
	public static Item findItem(string toFind){
		for (int x = 0; x < itemSlot.Length; x++) {										//Check each slot
			if (!itemSlot [x].isEmpty ()) {												//If the slot is not empty
				if(itemSlot[x].name == toFind){											//If this is the item we're looking for
					return itemSlot [x].getItem ();										//Remove it from its slot and return it
				}
			}
		}

		return null;																	//Returns null if the item could not be found
	}


	public static void EquipItem(){
		
	}
}
