using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Specialized weapon that gets enemies in a cone for it's attack.
 * 
 */
public class Melee : Weapon {

	[SerializeField]
	private int swingAngle;

	[SerializeField]
	private float swingRadius;

	[SerializeField]
	private float forceAmount;

	// Use this for initialization
	void Start () {
		itemType = ItemType.WEAPON;

	}

	//Attacks all enemies in a cone
	public override void Attack(){

		float mouseAngle = getMouseAngle ();																		//Get the angle of the mouse

		GameObject[] enemyList = GetEnemyInCone (mouseAngle);														//Get all enemies in a cone relative to the angle

		for (int x = 0; x < enemyList.Length; x++) {																//For each enemy in the cone
			float enemyAngle = getRelativeAngle (enemyList [x]);													//Get the angle of the enemy relative to the player

			//Apply force to the enemy based on relativity
			enemyList [x].GetComponent<Rigidbody2D> ().AddForce ( new Vector2( forceAmount * Mathf.Cos(enemyAngle), forceAmount * Mathf.Sin(enemyAngle) ) );
		}

	}

	//Gets the enemies in a cone with the given angle, swingAngle, and swingRadius
	private GameObject[] GetEnemyInCone(float angle){
		Player player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();							//Gets player
		float leftSide = angle - (swingAngle * Mathf.Deg2Rad);														//Calculates upper angle of cone
		float rightSide = angle + (swingAngle * Mathf.Deg2Rad);														//Calculates lower angle of cone

		Collider2D[] colliderList = Physics2D.OverlapCircleAll ( (Vector2) player.transform.position, swingRadius);	//Performs a circle overlap using swingRadius
		List<GameObject> enemyList = new List<GameObject>();														//Creates a list for holding enemies

		for (int x = 0; x < colliderList.Length; x++) {																//For each collider from the overlap circle
			float objectAngle = getRelativeAngle (colliderList[x].gameObject);										//Get the angle of it to the player

			if (colliderList [x].tag == "Enemy" && objectAngle > leftSide && objectAngle < rightSide) {				//If the collider is an enemy within the cone
				enemyList.Add (colliderList[x].gameObject);															//Add it to the list of enemies
			}
		}

		return enemyList.ToArray ();																				//Return array of enemies
	}
}
