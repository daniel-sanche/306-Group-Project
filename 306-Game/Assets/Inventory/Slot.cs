using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour {

	/// <summary>
	/// Currently contained item.
	/// </summary>
	public Item item;

	// Use this for initialization
	void Start () {
		item = null;
		setSprite (null);
	}

	/// <summary>
	/// Is the slot currently empty?
	/// </summary>
	/// <returns> <c>true</c> if slot is empty <c>false</c> otherwise </returns>
	public bool isEmpty(){
		return item == null ? true : false;		//returns true if the 
	}

		
	/// <summary>
	/// Removes and returns the item from the slot.
	/// </summary>
	/// <returns>The item.</returns>
	public Item getItem(){

		if (!isEmpty()) {						//If the slot is occupied
			Item temp = item;
			item = null;
			setSprite (null);					//Sets the current sprite to null
			return temp;						//Return the item
		}
		
		return null;							//Return null otherwise
	}


	/// <summary>
	/// Sets the slot's current item.
	/// </summary>
	/// <param name="toIns"> The item to insert. </param>
	public void setItem(Item toIns){
		if (isEmpty ()){						//If the slot is empty
			item = toIns;						//Add the item
			setSprite(toIns.sprite);			//Update sprite renderer
		}
	}


	/// <summary>
	/// Gives the sprite renderer a new sprite
	/// </summary>
	/// <param name="sprite">The sprite to render.</param>
	public void setSprite(Sprite sprite){
		GetComponent<SpriteRenderer> ().sprite = sprite;		
	}
}
