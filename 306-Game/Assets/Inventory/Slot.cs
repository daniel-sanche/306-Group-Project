using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {

	/// <summary>
	/// Currently contained item.
	/// </summary>
	public Item item;

	/// <summary>
	/// The pickup prefab.
	/// </summary>
	public GameObject pickupPrefab;


	private Slot selectedSlot;
	// Use this for initialization
	void Start () {
		item = null;							//Initializes the item to null
		setSprite (null);						//Ensure the sprite is null
	}

	/// <summary>
	/// Is the slot currently empty?
	/// </summary>
	/// <returns> <c>true</c> if slot is empty <c>false</c> otherwise </returns>
	public bool isEmpty(){
		return item == null ? true : false;		//returns true if the slot is empty, false otherwise
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
	/// Drops the item based on the current mouse position.
	/// </summary>
	public void dropItem(){
		if (!isEmpty ()) {
			Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (GameObject.FindGameObjectWithTag("Player").transform.position);		//Gets the position of the mouse in relation to the player;
			Vector2 dropPos = Vector2.MoveTowards((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position, relativeMousePos, 2f);					//Finds the drop position based on the mouse and the player
			Vector2 dropForce = Vector2.MoveTowards((Vector2)GameObject.FindGameObjectWithTag("Player").transform.position, relativeMousePos, 4f);					//Finds the force to apply based on the mouse and player

			GameObject drop = (GameObject)Instantiate (pickupPrefab, dropPos, Quaternion.identity);																	//Drops the item at the given location
			drop.GetComponent<Rigidbody2D> ().AddForce (dropForce - dropPos);																						//Applies force relative to the player
			drop.GetComponent<Pickup> ().item = getItem ();																											//Transfers item from this to the pickup item
		}
	}

	/// <summary>
	/// Gives the sprite renderer a new sprite
	/// </summary>
	/// <param name="sprite">The sprite to render.</param>
	public void setSprite(Sprite sprite){
		GetComponentInChildren<Image> ().sprite = sprite;
	}


	/// <summary>
	/// Handles all events involving mouse clicks
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right) {			//If this is clicked with the left mouse button
			//dropItem ();														//Drop the item
		}
	}

	public void OnEndDrag(PointerEventData eventData){
		Destroy (hoverImage);
		if (eventData.pointerCurrentRaycast.gameObject == null) {
			dropItem ();
		}
	}


	/// <summary>
	/// This function is called when a player drags something and releases it on this slot.
	/// </summary>
	/// <param name="eventData">Event data.</param>
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

	public void OnDrag(PointerEventData eventData){
		if(hoverImage != null)
			hoverImage.transform.position = eventData.position;
	}

	private GameObject hoverImage;

	public void OnBeginDrag(PointerEventData eventData){
		if (!isEmpty ()) {
			hoverImage = new GameObject ("Hover Image");

			hoverImage.transform.SetParent (GetComponentInParent<Canvas> ().transform);
			hoverImage.AddComponent<Image> ().sprite = item.sprite;
			hoverImage.GetComponent<Image> ().raycastTarget = false;

		}
	}
}
