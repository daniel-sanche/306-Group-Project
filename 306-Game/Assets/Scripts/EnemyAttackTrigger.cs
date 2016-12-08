using UnityEngine;
using System.Collections;

public class EnemyAttackTrigger : MonoBehaviour {


	public float damage = 5f;
	public float force = 1000f;
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" || col.tag == "BOX") {
			Vector2 direction = transform.position - col.transform.position;

			col.GetComponent<Rigidbody2D> ().AddForce (-direction * force);
			col.SendMessage ("removeHealth", damage);
		}
	}


}
