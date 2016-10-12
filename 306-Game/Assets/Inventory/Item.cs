using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	/// <summary>
	/// The item's name.
	/// </summary>
	public string name;

	/// <summary>
	/// The item's tooltip.
	/// </summary>
	public string tooltip;

	/// <summary>
	/// The item's sprite.
	/// </summary>
	public Sprite sprite;

	/// <summary>
	/// Uses the item for it's given purpose.
	/// </summary>
	public virtual void Use(){
		
	}
}
