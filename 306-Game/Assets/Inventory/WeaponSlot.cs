using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlot : Slot {

	//Contains a reference to the player for equipping/unequipping
	private Player player;

	void Start(){
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();			//Gets the player
	}

	/*
	 * Removes and returns the item from the slot.
	 **/
	public override Item getItem(){

		if (!isEmpty()) {																		//If the slot is occupied
			Item temp = item;																
			player.weapon = null;																
			item = null;																		//The player's weapon is null
			setSprite (null);																	//Sets the current sprite to null
			return temp;																		//Return the item
		}

		return null;																			//Return null otherwise
	}


	/*
	 * Sets the slot's current item.
	 **/
	public override void setItem(Item toIns){
		if (isEmpty ()){																		//If the slot is empty
			item = toIns;																		//Add the item
			setSprite(toIns.sprite);															//Update sprite renderer
			GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().weapon = (Weapon) toIns;
		}
	}
		
	/*
	 * Drops the item based on the current mouse position.
	 **/
	public override void dropItem(){
		if (!isEmpty ()) {
			Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (GameObject.FindGameObjectWithTag("Player").transform.position);		//Gets the position of the mouse in relation to the player;
			Vector2 dropPos = Vector2.MoveTowards((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position, relativeMousePos, 2f);					//Finds the drop position based on the mouse and the player
			Vector2 itemForce = Vector2.MoveTowards((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position, relativeMousePos, dropForce + 2f);		//Finds the force to apply based on the mouse and player

			GameObject drop = (GameObject)Instantiate (pickupPrefab, dropPos, Quaternion.identity);																	//Drops the item at the given location
			drop.GetComponent<Rigidbody2D> ().AddForce (itemForce - dropPos);																						//Applies force relative to the player
			drop.GetComponent<Pickup> ().item = getItem ();																											//Transfers item from this to the pickup item
			GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().weapon = null;
		}
	}





	/**
	 * Handles all events involving mouse clicks
	 **/
	public override void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right && !isEmpty()) {			//If the user right clicks the slot with an item in it
			if (!isEmpty()){
				if(Inventory.AddItem (getItem ()))
					GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().weapon = null;
			}
		}
	}


	/*
	 * This function is called when the player drags something onto this slot
	 **/
	public override void OnDrop(PointerEventData eventData){

		if (eventData.pointerDrag != null) {												//If the player dragged something
			if (eventData.pointerDrag.tag == "Slot") {										//If the player dragged from a spot
				Slot dragged = eventData.pointerDrag.GetComponent<Slot>();

				if (!dragged.isEmpty ()) {													//If the slot has an item
					if (dragged.item.itemType == ItemType.WEAPON) {
						Item draggedItem = dragged.getItem ();								//The dragged item

						if (isEmpty ()){													//If this slot is empty
							setItem (draggedItem);											//Give the item to this slot
						}
						else {																//Otherwise
							setItem (draggedItem);											//Swap the two items
							dragged.setItem (getItem());
						}
					}
				}	

			}
		}
	}
		

	/**
	 * This function is called whenever this slot is initially dragged by the cursor
	 **/
	public new void OnBeginDrag(PointerEventData eventData){
		if (!isEmpty ()) {																	//If there is an item in this slot
			hoverImage = new GameObject ("Hover Image");									//Create a new hover object

			hoverImage.transform.SetParent (GetComponentInParent<Canvas> ().transform);		//Set the parent of the hover object to be the canvas
			hoverImage.AddComponent<Image> ().sprite = item.sprite;							//Sets the sprite of the hover object as the item
			hoverImage.GetComponent<Image> ().raycastTarget = false;						//Prevents the hover image from being raycasted into

		}
	}

	/*
	 * This function is called constantly while this slot is dragged by the cursor
	 **/
	public new void OnDrag(PointerEventData eventData){						
		if(hoverImage != null)																//Set the hover object's location to the mouse position
			hoverImage.transform.position = eventData.position;
	}

	/*
	 * This function is called once the player has released something they are dragging.
	 **/
	public new void OnEndDrag(PointerEventData eventData){
		Destroy (hoverImage);																//Destroy the item hover imaage
		if (eventData.pointerCurrentRaycast.gameObject == null) {							//If we dragged an item over nothing
			dropItem ();																	//Drop the item
		}
	}
}
