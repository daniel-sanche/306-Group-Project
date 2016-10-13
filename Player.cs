using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	[SerializeField]
	float speed;

	Vector2 target;

	// Use this for initialization
	void Start () {
		target = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		HandleInput();
		Move();
	}

	private void HandleInput() { 
		Vector2 pos = this.transform.position;
		if (target == pos) {
			if (Input.GetKey(KeyCode.W)) {
				target.y += 1;
			}
			if (Input.GetKey(KeyCode.A)) {
				target.x -= 1;
			}
			if (Input.GetKey(KeyCode.S)) {
				target.y -= 1;
			}
			if (Input.GetKey(KeyCode.D)) {
				target.x += 1;
			}
		}
	}

	private void Move(){
		Vector2 pos = this.transform.position;
		this.transform.position = Vector2.MoveTowards(pos, target, speed * Time.deltaTime);
	}
}
