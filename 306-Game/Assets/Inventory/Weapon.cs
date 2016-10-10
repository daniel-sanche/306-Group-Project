using UnityEngine;
using System.Collections;

public class Weapon : Item {

	//The weapon's attack speed
	public float attackSpeed;

	// Use this for initialization
	void Start () {
		itemType = ItemType.WEAPON;
	}
}
