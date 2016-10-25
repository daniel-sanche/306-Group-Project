using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
 * This class represents an inventory slot that will store and move around items for the player to use.
 **/
public class Slot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	//Currently contained item.
	public Item item;

	//The pickup prefab.
	public GameObject pickupPrefab;

	//The force given to items dropped.
	public float dropForce;

	//The image for used for displaying the item
	public Image itemImage;

	//The font size for tooltips
	public int fontSize;

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
	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right && !isEmpty()) {			//If the user right clicks the slot with an item in it
			if (item.itemType == ItemType.WEAPON) {
				if (Inventory.weaponSlot.isEmpty ()) {
					Inventory.weaponSlot.setItem (getItem ());
				} else {
					Item temp = (Item) Inventory.weaponSlot.getItem ();
					Inventory.weaponSlot.setItem (getItem ());
					setItem (temp);
				}
			} else {
				getItem ().Use ();
			}
		}
	}

	/*
	 * This function is called once the player has released something they are dragging.
	 **/
	public void OnEndDrag(PointerEventData eventData){
		Destroy (hoverImage);																//Destroy the item hover imaage
		if (eventData.pointerCurrentRaycast.gameObject == null) {							//If we dragged an item over nothing
			dropItem ();																	//Drop the item
		}
	}


	/*
	 * This function is called when the player drags something onto this slot
	 **/
	public virtual void OnDrop(PointerEventData eventData){
		
		if (eventData.pointerDrag != null) {												//If the player dragged something
			if (eventData.pointerDrag.tag == "Slot") {										//If the player dragged from a spot
				Slot dragged = eventData.pointerDrag.GetComponent<Slot>();

				if (!dragged.isEmpty ()) {													//If the slot has an item
					if (isEmpty ())															//If this slot is empty
						setItem (dragged.getItem ());										//Give the item to this slot
					else {
						Item cur = getItem ();												//Otherwise
						setItem (dragged.getItem ());										//Swap the two items
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
		if(hoverImage != null)																//Set the hover object's location to the mouse position
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


	/**
	 * This function is called whenever the pointer enters the slot
	 **/
	public void OnPointerEnter(PointerEventData eventData){
		if (tooltip == null && !isEmpty()) {
			displayTooltip ();
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

	//GameObject that represents the tooltip displayed when hovering over an item
	private GameObject tooltip;

	/*
	 * Gives the sprite renderer a new sprite.
	 **/
	private void setSprite(Sprite sprite){
		Color invis = itemImage.color;															//Creates copy of current image color

		if (sprite == null) {																	//Turns image invisible when there is no item in the slot
			invis.a = 0f;
		} else {																				//Otherwise, the item is visible
			invis.a = 100f;
		}

		itemImage.sprite = sprite;																//Sets the image sprite to the given sprite
		itemImage.color = invis;																//Sets the image color to be visible or not
	}	


	private void displayTooltip(){
		tooltip = new GameObject ("Tooltip");
		tooltip.AddComponent<Image> ();
		tooltip.transform.SetParent (GetComponentInParent<Canvas> ().transform);			//Set the parent of the tooltip to be the canvas
		tooltip.transform.position = (Vector2) transform.position + Vector2.right * 100;	//Set the tooltip's location

		GameObject tooltipText = new GameObject ("TooltipText");							//Create tooltip text
		Text desc = tooltipText.AddComponent<Text> ();										//Add the text component to the gameobject
		tooltipText.transform.SetParent (tooltip.transform);								//Set the text as a child of the tooltip
		tooltipText.transform.position = tooltip.transform.position;						//Set the transform
			
		Color backgroundColor;																//Color for the tooltip background

		desc.text = item.name + "\n\n" + item.tooltip + "\n";								//Displays the name and tooltip

		if (item.itemType == ItemType.REGENERATION) {										//If it is a regeneration item
			backgroundColor = Color.green;													//Set background color as green
			Regeneration regen = (Regeneration)item;										//Get regen class from item
			desc.text += "\nHealth: " + regen.healthRegen + "\nFills: " + regen.hungerRegen;	//Display health and hunger points
		} 
		else if (item.itemType == ItemType.BUILDING) {										//If it is a building item
			backgroundColor = Color.yellow;													//Set background color as yellow
			Barricade barricade = (Barricade)item;											//Get barricade class from item
			desc.text += "\nBarricade restore: " + barricade.barricadeRestore;				//Display barricade points
		} 
		else if (item.itemType == ItemType.WEAPON) {										//If it is a building item
			backgroundColor = Color.red;                                                	//Set background color as red
			Weapon weapon = (Weapon) item;													//Get regen class from item
			desc.text += "\nAttack speed: " + weapon.attackSpeed;							//Display health and hunger points
		} 
		else {
			backgroundColor = Color.white;													//Otherwise, default background to white
		}

		backgroundColor.a = 10f;

		tooltip.GetComponent<Image> ().color = backgroundColor;

		desc.color = Color.black;																//Set the font color

		tooltipText.GetComponent<Text> ().raycastTarget = false;								//Prevents the hover image from being raycasted into
		tooltipText.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 12);		//Set the font of the tooltip
		tooltipText.GetComponent<Text>().fontSize = fontSize;
	}
}
