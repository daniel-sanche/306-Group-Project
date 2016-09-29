using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public string name;

	public string tooltip;

	public Sprite sprite;

	/// <summary>
	/// Uses the item for it's given purpose.
	/// </summary>
	public virtual void Use(){
		
	}
}
