using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	//The speed of the player
	public float speed;		

	//The swing speed of the player
	public float swingSpeed;

	//The currently equipped weapon
	public Weapon weapon;

	//The energy loss rate of the player (doubled when sprinting)
	public float energyLossRate;

	//The player's rigidbody
	private new Rigidbody2D rigidbody;

	//The player's animator
	private Animator animator;

	//The player's health/energy
	private HealthEnergy healthEnergy;

	[SerializeField]
	private int swingAngle;

	[SerializeField]
	private float swingRadius;

	[SerializeField]
	private float forceAmount;

	//The timer for when the player can attack
	private float attackTimer;

	/** The player's spawn point */
	private Vector3 spawnPoint; 

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();																	//Initializes the rigidbody variable
		animator = GetComponent<Animator>();																		//Initializes the animator variable
		healthEnergy = GetComponent<HealthEnergy>();																//Initializes the player's health/energy
		spawnPoint = GenerateSpawn (); 
		MoveToSpawn (); 
	} 

	private Vector3 GenerateSpawn(){ 
		GameObject camera = GameObject.FindWithTag("MainCamera"); 
		TileRenderer renderer = camera.GetComponent<TileRenderer> (); 
		TileChunk[,] chunkMat = renderer.GetChunkMatrix (); 
		int centerX = chunkMat.GetLength (0) / 2; 
		int centerY = chunkMat.GetLength (1) / 2; 
		for (int i = 0; i < centerX; i++) { 
			for (int j = 0; j < i && j < centerY; j++) { 
				TileChunk thisChunk = chunkMat [centerX + i, centerY + j]; 
				Vector2 offset = thisChunk.offset; 
				List<Vector2> freeSpaces = thisChunk.GetSpaces (new TileType[]{ TileType.Grass, TileType.Sand, TileType.Gravel }); 
				if (freeSpaces.Count > 0) { 
					Vector2 point2D = freeSpaces [0]; 
					return new Vector3 (point2D.x+offset.x, point2D.y+offset.y, 0); 
				} 
			} 
		} 

		return new Vector3 (0, 0, 0); 
	} 

	private void MoveToSpawn(){ 
		this.transform.localPosition = spawnPoint;  
	} 

	// Update is called once per frame
	void Update () {
		if ( !healthEnergy.isDead ) {
			Move ();																									//Moves the player
			Look ();																									//Rotates the player
			Attack ();																									//Attack using the current weapon
			handleEnergy ();																								//Handles the player's energy decay

			if (Input.GetKeyDown (KeyCode.P))
				Inventory.SortByName ();
		}
	}

	void FixedUpdate(){
		
		if(attackTimer >= 0)																						//Decrement attack timer if it is greater than zero
			attackTimer -= Time.deltaTime;

		if (healthEnergy.isDead )																					//If we run out of health, die
			Die ();
	}

	/** Drops this enemy's dropList items and kills the enemy */
	public void Die(){
		GetComponent<Collider2D> ().enabled = false;
		GameObject curDrop;

		for (int i = 0; i < Inventory.itemSlot.Length; i++) {												//For each item in inventory, drop it and add force to dropped item
			if(Inventory.itemSlot[i].item != null){
				curDrop = GameObject.Instantiate (Inventory.itemSlot[i].pickupPrefab, transform.position, Quaternion.identity) as GameObject;
				curDrop.GetComponent<Pickup> ().item = Inventory.itemSlot[i].getItem();
				Vector2 randomPoint = Random.insideUnitCircle * 50 + (Vector2)transform.position;
				Vector2 itemForce = Vector2.MoveTowards((Vector2)transform.position, randomPoint, 50f);		//Finds the force to apply based on the mouse and player
				curDrop.GetComponent<Rigidbody2D> ().AddForce( itemForce - (Vector2)transform.position);	//Add the force to the 
			}
		}

		if (weapon != null) {
			curDrop = GameObject.Instantiate (Inventory.weaponSlot.pickupPrefab, transform.position, Quaternion.identity) as GameObject;
			curDrop.GetComponent<Pickup> ().item = Inventory.weaponSlot.getItem ();
			Vector2 randomPoint = Random.insideUnitCircle * 50 + (Vector2)transform.position;
			Vector2 itemForce = Vector2.MoveTowards((Vector2)transform.position, randomPoint, 50f);		//Finds the force to apply based on the mouse and player
			curDrop.GetComponent<Rigidbody2D> ().AddForce( itemForce - (Vector2)transform.position);	//Add the force to the 
		}

		Invoke("LoadLoseScene", 3f);
	}

	private void LoadLoseScene(){
		SceneManager.LoadScene(3);
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
		if (weapon != null) {
			if (weapon.itemType == ItemType.RANGED)
				animator.SetBool ("Slingshot", true);
		} else {
			animator.SetBool ("Slingshot", false);
		}

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

			if (colliderList [x].tag == "NPC" && objectAngle > leftSide && objectAngle < rightSide) {				//If the collider is an enemy within the cone
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

	/** Returns the radian representation of the mouse angle */
	private float getMouseAngle(){
		Vector2 relativeMousePos = Input.mousePosition - Camera.main.WorldToScreenPoint (transform.position);		//Gets the position of the mouse in relation to the player;

		return Mathf.Atan2 (relativeMousePos.y, relativeMousePos.x);												//Calculate the angle of mouse to player.
	}

	//Returns the radian representation of the angle of a gameobject to the player
	private float getRelativeAngle(GameObject relativeObj){
		Vector2 relativePos =  relativeObj.transform.position - transform.position;									//Gets the position of the GameObject in relation to the player;

		return Mathf.Atan2 (relativePos.y, relativePos.x);															//Calculate the angle of the GameObject to the player.
	}

	public void SwingWeapon(Melee weapon){
		StartCoroutine ("SwingWeaponRoutine", weapon);
	}

	/**
	 * Swings a melee weapon relative to the player
	 */
	private IEnumerator SwingWeaponRoutine(Melee weapon){
		animator.SetBool ("Attacking", true);																		//Tell animator to attack
		Look ();																									//Look in the current mouse direction
		float inputAngle = getMouseAngle ();																		//Get the mouse angle relative to the player
		float lowerAngle = inputAngle - Mathf.Deg2Rad * (weapon.swingAngle / 2f);									//Get the lower swing angle
		float upperAngle = inputAngle + Mathf.Deg2Rad * (weapon.swingAngle / 2f);									//Get the upper swing angle						

		GameObject swingObject = GameObject.Instantiate (weapon.swingPrefab) as GameObject;							//Creates the swing object using the swing prefab

		float curAngle = lowerAngle;																				//Current angle to draw at begins at the lower angle

		//If we are facing right, swing from right to left
		if (GetComponent<SpriteRenderer> ().flipX == false) {
			for (float f = lowerAngle; f <= upperAngle; f = f + Time.deltaTime * swingSpeed) {
				curAngle = f;
				Vector2 swingPos = new Vector2 (transform.position.x + 1.25f * Mathf.Cos (curAngle), transform.position.y + 1.25f * Mathf.Sin (curAngle));
				swingObject.transform.position = swingPos;
				swingObject.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, getRelativeAngle (swingObject) * Mathf.Rad2Deg));
				yield return new WaitForEndOfFrame ();
			}
		//Otherwise, swing from left to right
		} else {
			for (float f = upperAngle; f >= lowerAngle; f = f - Time.deltaTime * swingSpeed) {
				curAngle = f;
				Vector2 swingPos = new Vector2 (transform.position.x + 1.25f * Mathf.Cos (curAngle), transform.position.y + 1.25f * Mathf.Sin (curAngle));
				swingObject.transform.position = swingPos;
				swingObject.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, getRelativeAngle (swingObject) * Mathf.Rad2Deg));
				yield return new WaitForEndOfFrame ();
			}
		}

		animator.SetBool ("Attacking", false);																		//Tell animator attacking is done
		Destroy (swingObject.gameObject);																			//Destroy swing object
	}


	/** Damages the player for the given amount */
	public void removeHealth(float amount){
		healthEnergy.TakeDamage (amount);
		StartCoroutine ("DamageFlash");
	}

	/** Damages the player for the given amount */
	public void ApplyDamge(float amount){
		healthEnergy.TakeDamage (amount);
		StartCoroutine ("DamageFlash");
	}

	/** Heals the player for the given amount */
	public void addHealth(float amount){
		healthEnergy.RecoverHealth (amount);
		StartCoroutine ("HealFlash");
	}

	/** Damages the player for the given amount */
	public void removeEnergy(float amount){
		healthEnergy.LoseEnergy(amount);
	}

	/** Heals the player for the given amount */
	public void addEnergy(float amount){
		healthEnergy.RecoverEnergy (amount);
	}

	/** Decrements the player's energy based on energyLossRate */
	private void handleEnergy(){
		
		if (healthEnergy.energy.CurrentVal <= 0f) {
			if (Input.GetKeyDown (KeyCode.LeftShift))
				healthEnergy.TakeDamage ((Time.deltaTime * energyLossRate * 2) / 10);
			else
				healthEnergy.TakeDamage (Time.deltaTime * energyLossRate / 10);
		} else {
			if (Input.GetKeyDown (KeyCode.LeftShift))
				healthEnergy.LoseEnergy ((Time.deltaTime * energyLossRate) / 10f);
			else
				healthEnergy.LoseEnergy ((Time.deltaTime * energyLossRate * 2) / 10f);
		}
	}

	/** Temporarily flashes red to indicate the player has taken damage */
	private IEnumerator DamageFlash(){
		GetComponent<SpriteRenderer> ().color = Color.red;
		yield return new WaitForSeconds (0.1f);
		GetComponent<SpriteRenderer> ().color = Color.white;
	}

	/** Temporarily flashes red to indicate the player has taken damage */
	private IEnumerator HealFlash(){
		GetComponent<SpriteRenderer> ().color = Color.green;
		yield return new WaitForSeconds (0.1f);
		GetComponent<SpriteRenderer> ().color = Color.white;
	}

}