using UnityEngine;
using System.Collections;

public class Regeneration : Item {

	//The amount of health this item gives when consumed
	public int healthRegen;

	//The amount of energy points this item gives when consumed
	public int energyRegen;

	// Use this for initialization
	void Start () {
		itemType = ItemType.REGENERATION;
	}

	//Uses the regeneration item
	public override void Use(){
		if(healthRegen >= 0f)
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("addHealth", healthRegen);
		else
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("removeHealth", healthRegen * -1f);

		if(energyRegen >= 0f)
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("addEnergy", energyRegen);
		else
			GameObject.FindGameObjectWithTag ("Player").SendMessage ("removeEnergy", energyRegen * -1f);
	}
}
