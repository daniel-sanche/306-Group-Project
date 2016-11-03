using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	//The smoothing speed of the follow
	public float smoothingSpeed;
	
	// Update is called once per frame
	void Update () {

		//Find the player
		GameObject player = GameObject.FindGameObjectWithTag ("Player");

		//Calculate 2D distance between camera and player
		float distance = Vector2.Distance (transform.position, player.transform.position);

		//Creates vector that moves towards the player based on distance and smoothing speed
		Vector2 moveTo = Vector2.MoveTowards (transform.position, player.transform.position, Time.deltaTime * distance * smoothingSpeed);

		//Move GameObject to position
		transform.position = new Vector3 (moveTo.x, moveTo.y, transform.position.z);
	}
}
