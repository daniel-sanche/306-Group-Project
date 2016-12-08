﻿using UnityEngine;
using System.Collections;

public class Weapon : Item {

	//The weapon's damage
	public int damage;

	//The weapon's attack cooldown
	public float attackCooldown;

	// Use this for initialization
	void Start () {
		itemType = ItemType.MELEE;
	}

	//Attack class to be used by all descendants
	public virtual void Attack(){}
		
	//************************
	//GENERAL HELPER FUNCTIONS
	//************************

	//Returns the radian representation of the mouse angle
	protected float getMouseAngle(){
		Player player =  GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();						//Gets player

		Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (player.transform.position);//Gets the position of the mouse in relation to the player;

		return Mathf.Atan2 (relativeMousePos.y, relativeMousePos.x);												//Calculate the angle of mouse to player.
	}

	//Returns the radian representation of the angle of a gameobject to the player
	protected float getRelativeAngle(GameObject relativeObj){
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();							//Gets player

		Vector2 relativePos =  relativeObj.transform.position - player.transform.position;							//Gets the position of the GameObject in relation to the player;

		return Mathf.Atan2 (relativePos.y, relativePos.x);															//Calculate the angle of the GameObject to the player.
	}
}
