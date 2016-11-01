using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {


	public float speed;

	public Vector2 direction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D> ().velocity = direction * speed;
	}

	public void Initialize(float _speed, Vector2 _direction){
		speed = _speed;
		direction = _direction;
	}
}
