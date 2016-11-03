using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	// Number of slots in inventory.
	public int size;

	// The number of columns (Must be greater than 0).
	public int columns;

	// The X and Y spacing between inventory slots.
	public float xSpacing, ySpacing;

	// The GameObject containing the slots
	public GameObject overlay;

	// Array of item slots.
	public static Slot[] itemSlot;

	// Slot for holding the equipped weapon
	public static WeaponSlot weaponSlot;

	// Prefab of an empty slot for instantiating.
	public GameObject slotPrefab;

	// Prefab for the weapon slot.
	public GameObject weaponSlotPrefab;

	//Key used for toggling the inventory
	public KeyCode toggleKey;

	//Is the inventory visible?
	public static bool visible;

	// Use this for initialization
	void Start () {
		
		itemSlot = new Slot[size];														//Initialize array

		for (int x = 0; x < size; x++) {												//For each x, instantiate a slot
			GameObject curSlot = (GameObject)Instantiate (slotPrefab, new Vector2((x % columns) * xSpacing, (x / columns) *  ySpacing) + (Vector2) transform.position, Quaternion.identity);		
			curSlot.transform.SetParent(this.transform);								//Set the parent of the slot to be the inventory
			curSlot.name = "Slot" + x;													//Name the slot with its current array index
			itemSlot[x] = curSlot.GetComponent<Slot>();									//Assign the new slot to an array index
		}

																						//Instantiate the weapon slot
		GameObject weapSlot = (GameObject)Instantiate (weaponSlotPrefab, new Vector2((size % columns) * xSpacing, (size / columns) *  ySpacing) + (Vector2) transform.position, Quaternion.identity);	

		weapSlot.transform.SetParent (this.transform);									//Set weapon slot as child of inventory
		weapSlot.name = "Weapon Slot";													//Name the weapon slot
		weaponSlot = weapSlot.GetComponent<WeaponSlot> ();								//Get the slot component
		weaponSlot.GetComponent<Image> ().color = Color.red;							//Change the color to blue

	}


	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (toggleKey)) {
			if (visible)
				hideInventory ();
			else
				showInventory ();
		}
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
		for (int x = 0; x < itemSlot.Length; x++) {										//Check each slot
			if(itemSlot[x].isEmpty())													//If the slot is empty
				return false;															//The inventory is not full
		}

		return true;																	//If all slots are occupied, the inventory is full
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
	/**	
	* Finds the item by name, removes it from its slot and returns it
	**/
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
	/**
	* Hides the inventory GUI
	**/
	public static void hideInventory(){
		for (int x = 0; x < itemSlot.Length; x++) {										//For each item slot
			itemSlot [x].gameObject.SetActive (false);									//Set it as not active
		}

		weaponSlot.gameObject.SetActive (false);										//Set the weapon slot as not active
		visible = false;
	}

	//Reveals the inventory GUI
	public static void showInventory(){
		for (int x = 0; x < itemSlot.Length; x++) {										//For each item slot
			itemSlot [x].gameObject.SetActive (true);									//Set it as active 
		}

		weaponSlot.gameObject.SetActive (true);											//Set the weapon slot as active
		visible = true;
	}
}
