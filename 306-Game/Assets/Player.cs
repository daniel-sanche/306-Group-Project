using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	/// Public variables

	public float speed;				///The speed of the player


	/// Private variables

	private Rigidbody2D rigidbody;	///The player's rigidbody

	/// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();	//Initializes the rigidbody variable
	}


	/// Update is called once per frame
	void Update () {
		Move ();			//Moves the player
		Look ();			//Rotates the player
	}
		

	/// Moves the player based on input
	private void Move(){
		if(Input.GetKey(KeyCode.LeftShift))													//Sets the player's velocity based on input and speed
			rigidbody.velocity = getInputVector () * speed * 2;								//If shift is held, player sprints
		else
			rigidbody.velocity = getInputVector () * speed;									//Otherwise, it is normal speed
	}
		

	/// Returns a Vector3 representing the player's input
	private Vector2 getInputVector(){
		return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));		//Returns a Vector2 representing the player's input on the x and y axis
	}


	/// Orients such that the player is looking at the mouse 
	private void Look(){
		Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (transform.position);		//Gets the position of the mouse in relation to the player;

		float angle = (float)Mathf.Atan2 (relativeMousePos.y, relativeMousePos.x) * Mathf.Rad2Deg - 90;				//Calculate the angle of mouse to player. Subtracts 90 so that 0 degrees = North

		angle = Mathf.RoundToInt (angle / 90) * 90;																	//Rounds the given angle to the nearest 0, 90, 180, or 270

		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));											//Looks at the given angle (up, right, down, or left)
	}
}