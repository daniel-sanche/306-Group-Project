using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	//The speed of the player
	public float speed;				

	public GameObject weapon;

	//The player's rigidbody
	private Rigidbody2D rigidbody;

	[SerializeField]
	private int swingAngle;

	[SerializeField]
	private float swingRadius;

	[SerializeField]
	private float forceAmount;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();	//Initializes the rigidbody variable
	}


	// Update is called once per frame
	void Update () {
		Move ();									//Moves the player
		Look ();									//Rotates the player
		Attack ();									//Attack using the current weapon
	}


	// Moves the player based on input
	private void Move(){
		if(Input.GetKey(KeyCode.LeftShift))																			//Sets the player's velocity based on input and speed
			rigidbody.velocity = getInputVector () * speed * 2;														//If shift is held, player sprints
		else
			rigidbody.velocity = getInputVector () * speed;															//Otherwise, it is normal speed
	}

	//Attacks using the currently equipped weapon
	private void Attack(){
		
		if (Input.GetMouseButtonDown (0)) {																			//If the player presses the left mouse button
			float mouseAngle = getMouseAngle ();																	//Get the angle of the mouse

			GameObject[] enemyList = GetEnemyInCone (mouseAngle);													//Get all enemies in a cone relative to the angle

			for (int x = 0; x < enemyList.Length; x++) {															//For each enemy in the cone
				float enemyAngle = getRelativeAngle (enemyList [x]);												//Get the angle of the enemy relative to the player
																													//Apply force to the enemy based on relativity
				enemyList [x].GetComponent<Rigidbody2D> ().AddForce ( new Vector2( forceAmount * Mathf.Cos(enemyAngle), forceAmount * Mathf.Sin(enemyAngle) ) );
			}
		}
	}

	//Gets the enemies in a cone with the given angle, swingAngle, and swingRadius
	private GameObject[] GetEnemyInCone(float angle){
		float leftSide = angle - (swingAngle * Mathf.Deg2Rad);														//Calculates upper angle of cone
		float rightSide = angle + (swingAngle * Mathf.Deg2Rad);														//Calculates lower angle of cone

		Collider2D[] colliderList = Physics2D.OverlapCircleAll ( (Vector2) transform.position, swingRadius);		//Performs a circle overlap using swingRadius
		List<GameObject> enemyList = new List<GameObject>();														//Creates a list for holding enemies

		for (int x = 0; x < colliderList.Length; x++) {																//For each collider from the overlap circle
			float objectAngle = getRelativeAngle (colliderList[x].gameObject);										//Get the angle of it to the player

			if (colliderList [x].tag == "Enemy" && objectAngle > leftSide && objectAngle < rightSide) {				//If the collider is an enemy within the cone
				enemyList.Add (colliderList[x].gameObject);															//Add it to the list of enemies
			}
		}

		return enemyList.ToArray ();																				//Return array of enemies
	}

	// Returns a Vector3 representing the player's input
	private Vector2 getInputVector(){
		return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));									//Returns a Vector2 representing the player's input on the x and y axis
	}


	// Orients such that the player is looking at the mouse 
	private void Look(){

		float angle = getMouseAngle () * Mathf.Rad2Deg;																//Gets the mouse angle and translate it to degrees

		angle = Mathf.RoundToInt (angle / 90) * 90;																	//Rounds the given angle to the nearest 0, 90, 180, or 270

		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));											//Looks at the given angle (up, right, down, or left)
	}

	//Returns the radian representation of the mouse angle
	private float getMouseAngle(){
		Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (transform.position);		//Gets the position of the mouse in relation to the player;

		return Mathf.Atan2 (relativeMousePos.y, relativeMousePos.x);												//Calculate the angle of mouse to player.
	}

	//Returns the radian representation of the angle of a gameobject to the player
	private float getRelativeAngle(GameObject relativeObj){
		Vector2 relativePos =  relativeObj.transform.position - transform.position;									//Gets the position of the GameObject in relation to the player;

		return Mathf.Atan2 (relativePos.y, relativePos.x);															//Calculate the angle of the GameObject to the player.
	}
}