using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	//Currently contained item.
	public Item item;

	//The pickup prefab.
	public GameObject pickupPrefab;

	//The force given to items dropped.
	public float dropForce;

	// Use this for initialization
	void Start () {
		item = null;							//Initializes the item to null
		setSprite (null);						//Ensure the sprite is null
	}

	/*
	 * Is the slot currently empty?
	 **/
	public bool isEmpty(){
		return item == null ? true : false;		//returns true if the slot is empty, false otherwise
	}

		
	/*
	 * Removes and returns the item from the slot.
	 **/
	public Item getItem(){

		if (!isEmpty()) {						//If the slot is occupied
			Item temp = item;
			item = null;
			setSprite (null);					//Sets the current sprite to null
			return temp;						//Return the item
		}
		
		return null;							//Return null otherwise
	}


	/*
	 * Sets the slot's current item.
	 **/
	public void setItem(Item toIns){
		if (isEmpty ()){						//If the slot is empty
			item = toIns;						//Add the item
			setSprite(toIns.sprite);			//Update sprite renderer
		}
	}

	/*
	 * Drops the item based on the current mouse position.
	 **/
	public void dropItem(){
		if (!isEmpty ()) {
			Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (GameObject.FindGameObjectWithTag("Player").transform.position);		//Gets the position of the mouse in relation to the player;
			Vector2 dropPos = Vector2.MoveTowards((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position, relativeMousePos, 2f);					//Finds the drop position based on the mouse and the player
			Vector2 itemForce = Vector2.MoveTowards((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position, relativeMousePos, dropForce + 2f);		//Finds the force to apply based on the mouse and player

			GameObject drop = (GameObject)Instantiate (pickupPrefab, dropPos, Quaternion.identity);																	//Drops the item at the given location
			drop.GetComponent<Rigidbody2D> ().AddForce (itemForce - dropPos);																						//Applies force relative to the player
			drop.GetComponent<Pickup> ().item = getItem ();																											//Transfers item from this to the pickup item
		}
	}


	/**
	 * Handles all events involving mouse clicks
	 **/
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right && !isEmpty()) {
			getItem().Use ();
		}
	}

	/*
	 * This function is called once the player has released something they are dragging.
	 **/
	public void OnEndDrag(PointerEventData eventData){
		Destroy (hoverImage);													//Destroy the item hover imaage
		if (eventData.pointerCurrentRaycast.gameObject == null) {				//If we dragged an item over nothing
			dropItem ();														//Drop the item
		}
	}


	/*
	 * This function is called when the player drags something onto this slot
	 **/
	public void OnDrop(PointerEventData eventData){
		
		if (eventData.pointerDrag != null) {									//If the player dragged something
			if (eventData.pointerDrag.tag == "Slot") {							//If the player dragged from a spot
				Slot dragged = eventData.pointerDrag.GetComponent<Slot>();

				if (!dragged.isEmpty ()) {										//If the slot has an item
					if (isEmpty ())												//If this slot is empty
						setItem (dragged.getItem ());							//Give the item to this slot
					else {
						Item cur = getItem ();									//Otherwise
						setItem (dragged.getItem ());							//Swap the two items
						dragged.setItem (cur);
					}
				}	
			}
		}
	}

	/*
	 * This function is called constantly while this slot is dragged by the cursor
	 **/
	public void OnDrag(PointerEventData eventData){						
		if(hoverImage != null)													//Set the hover object's location to the mouse position
			hoverImage.transform.position = eventData.position;
	}

	//GameObject that represents the hover image for when items are moved by the player
	private GameObject hoverImage;

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

	//GameObject that represents the tooltip displayed when hovering over an item
	private GameObject tooltip;

	/**
	 * This function is called whenever the pointer enters the slot
	 **/
	public void OnPointerEnter(PointerEventData eventData){
		if (tooltip == null && !isEmpty()) {
			tooltip = new GameObject ("Tooltip");
			tooltip.transform.SetParent (GetComponentInParent<Canvas> ().transform);			//Set the parent of the tooltip to be the canvas
			tooltip.AddComponent<Text> ().text = item.tooltip;									//Set the text of the tooltip to display the current item's tooltip
			tooltip.transform.position = transform.position;									//Set the tooltip's location
			tooltip.GetComponent<Text> ().raycastTarget = false;								//Prevents the hover image from being raycasted into
			tooltip.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 12);	//Set the font of the tooltip
			tooltip.GetComponent<Text> ().color = Color.black;									//Set the font color
		}
	}

	/**
	 * This function is called whenever the pointer exits the slot
	 **/
	public void OnPointerExit(PointerEventData eventData){
		if (tooltip != null) {																	//Upon moving the mouse out of the sloow
			Destroy (tooltip);																	//Delete the tooltip
		}
	}

//****************************************************************************************************************************************************
// Private/Helper functions
//****************************************************************************************************************************************************


	/*
	 * Gives the sprite renderer a new sprite.
	 **/
	private void setSprite(Sprite sprite){
		Color invis = GetComponent<Image> ().color;

		if (sprite == null) {
			invis.a = 255f;
		} else {
			invis.a = 100f;
		}

		GetComponent<Image> ().sprite = sprite;
		GetComponent<Image> ().color = invis;
	}
}
