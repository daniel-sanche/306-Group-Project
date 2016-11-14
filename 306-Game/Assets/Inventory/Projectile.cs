using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	//The speed of the projectile
	private float speed;

	//The lifetime of the projectile
	private float projectileLifetime = 1f;

	//The direction the projectile travels
	private Vector2 direction;

	void Start(){
		Invoke ("Destroy", projectileLifetime);									//Destroys the projectile after the lifetime is up
	}

	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D> ().velocity = direction * speed;				//Sets the velocity of the projectile of the rigibody based on direction and speed
	}

	//Initializes projectile with the given speed, direction, and death time
	public void Initialize(float _speed, Vector2 _direction){
		speed = _speed;
		direction = _direction;
	}

	//Destroys this projectile
	private void Destroy(){
		Destroy (gameObject);
	}
}
