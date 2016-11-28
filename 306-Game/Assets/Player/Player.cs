using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	//The speed of the player
	public float speed;				

	//The currently equipped weapon
	public Weapon weapon;

	//The player's health and energy
	public HealthEnergy healthEnergy;

	//The player's rate of energy loss (doubled when sprinting)
	public float energyLossRate;

	//The player's rigidbody
	private new Rigidbody2D rigidbody;

	//The player's animator
	private Animator animator;

	[SerializeField]
	private int swingAngle;

	[SerializeField]
	private float swingRadius;

	//The amount of 
	[SerializeField]
	private float forceAmount;

	//The timer for when the player can attack
	private float attackTimer;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();																	//Initializes the rigidbody variable
		animator = GetComponent<Animator>();																		//Initializes the animator variable
		healthEnergy = GetComponent<HealthEnergy>();																//Initializes the healthEnergy variable
	}

	// Update is called once per frame
	void Update () {
		Move ();																									//Moves the player
		Look ();																									//Rotates the player
		Attack ();																									//Attack using the current weapon
		handleEnergy ();
	}

	void FixedUpdate(){
		if(attackTimer >= 0)																						//Decrement attack timer if it is greater than zero
			attackTimer -= Time.deltaTime;
	}

	// Moves the player based on input
	private void Move(){
		Vector2 inputVector = getInputVector ();

		if(Input.GetKey(KeyCode.LeftShift))																			//If the player is holding shift 
			inputVector *= 2;																						//Go twice as fast

		animator.SetFloat ("X", inputVector.x);																		//Set animator X variable
		animator.SetFloat ("Y", inputVector.y);																		//Set animator Y variable

		if (inputVector.x < 0)																						//If the player moves left
			GetComponent<SpriteRenderer> ().flipX = true;															//Flip the sprite renderer's x coord
		if (inputVector.x > 0)																						//If the player moves right
			GetComponent<SpriteRenderer> ().flipX = false;															//The sprite renderer's x coord will not be flipped
		
		rigidbody.velocity = inputVector * speed;																	//Set the velocity of the player based on input and speed
	}

	//Attacks using the currently equipped weapon
	private void Attack(){
		
		if (Input.GetMouseButtonDown (0)) {																			//If the player presses the left mouse button
			if (weapon != null && attackTimer <= 0) {
				weapon.Attack ();
				attackTimer = weapon.attackCooldown;
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

		int integerAngle = Mathf.RoundToInt (angle / 90) * 90;														//Rounds the given angle to the nearest 0, 90, 180, or 270

		Vector2 direction = new Vector2(0,0);																		//Initialize direction vector for updating animator

		if (integerAngle == 0)																						//If we are facing right
			direction = new Vector2(1,0);																			//Set the direction to right
		else if (integerAngle == 90)																				//Else, if we're facing up
			direction = new Vector2(0,1);																			//Set the direction to up
		else if (integerAngle == 180 || integerAngle == -180)														//Else, if we're facing left
			direction = new Vector2(-1,0);																			//Set the direction to left
		else if (integerAngle == -90)																				//Else, if we're facing down
			direction = new Vector2(0,-1);																			//Set the direction to down

		animator.SetFloat ("DirectionX", direction.x);																//Set the x direction in the animator
		animator.SetFloat ("DirectionY", direction.y);																//Set the y direction in the animator
		//transform.rotation = Quaternion.Euler (new Vector3 (0, 0, integerAngle));									//Looks at the given angle (up, right, down, or left)
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

	//Damages the player for the given amount
	public void removeHealth(float amount){
		healthEnergy.TakeDamage (amount);
	}

	//Heals the player for the given amount
	public void addHealth(float amount){
		healthEnergy.RecoverHealth (amount);
	}

	//Damages the player for the given amount
	public void removeEnergy(float amount){
		healthEnergy.TakeDamage (amount);
	}

	//Heals the player for the given amount
	public void addEnergy(float amount){
		healthEnergy.RecoverHealth (amount);
	}


	private void handleEnergy(){
		if (Input.GetKeyDown (KeyCode.LeftShift))
			healthEnergy.LoseEnergy ((Time.deltaTime * energyLossRate) / 10f);
		else
			healthEnergy.LoseEnergy ((Time.deltaTime * energyLossRate * 2) / 10f);
	}
}