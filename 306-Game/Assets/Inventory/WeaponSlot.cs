using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlot : Slot {

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

						GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().weapon = (Weapon) draggedItem;
					}
				}	

			}
		}
	}
		

	/**
	 * This function is called whenever this slot is initially dragged by the cursor
	 **/
	public void OnBeginDrag(PointerEventData eventData){
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
	public void OnDrag(PointerEventData eventData){						
		if(hoverImage != null)																//Set the hover object's location to the mouse position
			hoverImage.transform.position = eventData.position;
	}

	/*
	 * This function is called once the player has released something they are dragging.
	 **/
	public void OnEndDrag(PointerEventData eventData){
		Destroy (hoverImage);																//Destroy the item hover imaage
		if (eventData.pointerCurrentRaycast.gameObject == null) {							//If we dragged an item over nothing
			dropItem ();																	//Drop the item
			GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().weapon = null;
		}
	}
}
