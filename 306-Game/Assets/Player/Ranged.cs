using UnityEngine;
using System.Collections;

public class Ranged : Weapon {

	//Range of the weapon
	public float range;

	//Force of the weapon
	public float force;

	//The projectile to fire
	public Projectile projectile;

	// Use this for initialization
	void Start () {
		itemType = ItemType.WEAPON;																								//Sets the weapon type as weapon
	}

	//Shoots a projectile with the given range
	public override void Attack(){
		Player player =  GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();									//Gets player
		float mouseAngle = getMouseAngle ();																					//Gets the angle of the mouse relative to the player

		GameObject shot;
		shot = GameObject.Instantiate (projectile.gameObject, player.transform.position, Quaternion.identity) as GameObject;	//Instantiates shot based on player

		shot.GetComponent<Projectile> ().Initialize(force, new Vector2 (Mathf.Cos (mouseAngle), Mathf.Sin (mouseAngle)));		//Sets the velocity of the rigidbody
	}
}
