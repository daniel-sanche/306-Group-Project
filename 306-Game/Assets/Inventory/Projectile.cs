using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	//The speed of the projectile
	private float speed;

	//The lifetime of the projectile
	private float projectileLifetime = 1f;

	//The projectile damage
	private int damage;

	//The direction the projectile travels
	private Vector2 direction;

	void Start(){
		Physics2D.IgnoreCollision (GetComponent<Collider2D> (), GameObject.FindWithTag ("Player").GetComponent<Collider2D> ());
		Invoke ("Destroy", projectileLifetime);									//Destroys the projectile after the lifetime is up
	}

	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D> ().velocity = direction * speed;				//Sets the velocity of the projectile of the rigibody based on direction and speed
	}

	//Initializes projectile with the given speed, direction, and death time
	public void Initialize(float _speed, Vector2 _direction, int _damage){
		speed = _speed;
		direction = _direction;
		damage = _damage;
	}

	//Destroys this projectile
	private void Destroy(){
		Destroy (gameObject);
	}

	//If we collide with something
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "NPC") {
			col.gameObject.SendMessage ("TakeDamage", damage);
			Destroy (gameObject);
		}
	}
}
