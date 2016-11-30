using UnityEngine;
using System.Collections;

public class EnemyAttackTrigger : MonoBehaviour {


	public string damage;
	public float force;
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			Vector2 direction = transform.position - col.transform.position;

			col.GetComponent<Rigidbody2D> ().AddForce (-direction * force);
			col.SendMessage ("ApplyDamage", damage);
		}
	}


}
