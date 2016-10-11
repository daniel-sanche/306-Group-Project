using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class WeaponSlot : Slot {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/**
	 * Handles all events involving mouse clicks
	 **/
	public override void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right && !isEmpty()) {			//If the user right clicks the slot with an item in it
			if (!isEmpty()){
				Inventory.AddItem (getItem ());
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
						if (isEmpty ()){													//If this slot is empty
							setItem (dragged.getItem ());									//Give the item to this slot
						}
						else {
							Item cur = getItem ();											//Otherwise
							setItem (dragged.getItem ());									//Swap the two items
							dragged.setItem (cur);
						}
					}
				}	
			}
		}
	}
}
