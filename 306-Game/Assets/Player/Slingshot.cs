using UnityEngine;
using System.Collections;

public class Slingshot : Weapon {

	//Range of the weapon
	public float range;

	//The projectile to fire
	public GameObject projectile;

	// Use this for initialization
	void Start () {
		itemType = ItemType.WEAPON;
	}

	// Update is called once per frame
	void Update () {

	}

	public override void Attack(){
		
	}
}
