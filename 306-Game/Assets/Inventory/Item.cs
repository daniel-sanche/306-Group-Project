using UnityEngine;
using System.Collections;

public enum ItemType{
	TESTING, WEAPON, BUILDING, REGENERATION, WALL
}

public class Item : MonoBehaviour {

	// The item's name.
	public new string name;

	// The item's tooltip.
	public string tooltip;

	// The item's sprite.
	public Sprite sprite;

	//The item's type category
	public ItemType itemType;

	// Uses the item for it's given purpose.
	public virtual void Use(){
		
	}
}
