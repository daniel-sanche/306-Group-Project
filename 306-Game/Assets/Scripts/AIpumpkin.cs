﻿using UnityEngine;
using System.Collections;

public class AIpumpkin : MonoBehaviour {

	UnitPath unitpath;


	public float randompointlimit = 20f;
	public int randombal_spawn = 50;
	public int randombal_strolloridle = 50;
	public float agrorange = 10f;
	public float attackrange = 5f;

	Transform player;


	public float dist;



	bool flip = false;
	bool newpathcd;
	private bool attacking = false;
	//change to false when ready for release
	bool monster = true;
	Vector2 spawnpos;
	private Rigidbody2D rb;
	private Animator anime;
	private SpriteRenderer sprite;
	private Grid grid;


	private DecisionTree ai = new DecisionTree();
	private DecisionTreeNode node_donothingcheck = new DecisionTreeNode ();
	private DecisionTreeNode node_donothing = new DecisionTreeNode();
	private DecisionTreeNode node_monstercheck = new DecisionTreeNode ();
	private DecisionTreeNode node_spawnrangecheck = new DecisionTreeNode();
	private DecisionTreeNode node_inrangecheck= new DecisionTreeNode();
	private DecisionTreeNode node_movetoplayer = new DecisionTreeNode();
	private DecisionTreeNode node_randomstrolloridlecheck = new DecisionTreeNode();
	private DecisionTreeNode node_movetorandom = new DecisionTreeNode();
	private DecisionTreeNode node_agrocheck = new DecisionTreeNode();
	private DecisionTreeNode node_attack = new DecisionTreeNode();
	private DecisionTreeNode node_attackcheck = new DecisionTreeNode();
	private DecisionTreeNode node_idle = new DecisionTreeNode();
	private DecisionTreeNode node_ampumpkin = new DecisionTreeNode();
	private DecisionTreeNode node_loscheck = new DecisionTreeNode();
	private DecisionTreeNode node_lastknowncheck = new DecisionTreeNode ();
	private DecisionTreeNode node_movetolastknown = new DecisionTreeNode ();
	private DecisionTreeNode node_checkforbox = new DecisionTreeNode();
	private DecisionTreeNode node_attackbox = new DecisionTreeNode();

	private void NewPathCd(){
		newpathcd = false;
	}

	void Awake(){
		spawnpos = new Vector2 (transform.position.x, transform.position.y);


	}
	void Start () {
		unitpath = GetComponent<UnitPath> ();
		//unitpath.target = GameObject.FindGameObjectWithTag ("Player").transform;
		rb = GetComponent<Rigidbody2D> ();
		BuildDecisionTree ();
		anime = gameObject.GetComponent<Animator> ();
		sprite = gameObject.GetComponent<SpriteRenderer> ();
		localscalex = transform.localScale;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	public void DoNothing(){
		/* do nothing so we dont calculate anything the player is to far from*/
	}

	public float donothingdist = 30f;
	public bool DoNothingCheck(){
		if (Vector2.Distance(transform.position, player.position) > donothingdist ){
			return true;
		}
		return false;
	}

	/* Build my decision tree at spawn in start (awake?)*/
	void BuildDecisionTree(){

		ai.root = node_donothingcheck;

		node_donothingcheck.decdel = DoNothingCheck;
		node_donothingcheck.right = node_monstercheck;
		node_donothingcheck.left = node_donothing;

		node_donothing.actdel = DoNothing;

		node_monstercheck.decdel = MonsterCheck;
		node_monstercheck.left = node_agrocheck;
		node_monstercheck.right = node_ampumpkin;

		node_ampumpkin.actdel = AmPumpkin;

		node_agrocheck.decdel = AgroRangeCheck;
		node_agrocheck.left = node_loscheck;
		node_agrocheck.right = node_checkforbox;

		node_checkforbox.decdel = CheckForBox;
		node_checkforbox.left = node_attackbox;
		node_checkforbox.right = node_randomstrolloridlecheck;

		node_attackbox.actdel = AttackBox;


		node_loscheck.decdel = LOSCheck;
		node_loscheck.left = node_attackcheck;
		node_loscheck.right = node_lastknowncheck;

		node_lastknowncheck.decdel = LastKnownCheck;
		node_lastknowncheck.left = node_movetolastknown;
		node_lastknowncheck.right = node_checkforbox;

		node_movetolastknown.actdel = MoveToLastKnown;


		node_attackcheck.decdel = AttackRangeCheck;
		node_attackcheck.left = node_attack;
		node_attackcheck.right = node_movetoplayer;

		node_randomstrolloridlecheck.decdel = RandomStrollorIdleCheck;
		node_randomstrolloridlecheck.left = node_movetorandom;
		node_randomstrolloridlecheck.right = node_idle;

		node_idle.actdel = Idle;

		node_attack.actdel = Attack;


		node_movetoplayer.actdel = MoveToPlayer;

		node_movetorandom.actdel =MoveToRandom;

	}

	/* Changes the path in UnitPath script*/

	void ChangePath(){

		unitpath.SendMessage ("RestartPath");
	}

	Vector3 localscalex;
	Vector2 me;
	private Vector2 target;
	private bool playside, playback;
	float diffx, diffy;


	void Update(){
		ai.Search (ai.root);
		me = transform.position;
		//target = new Vector2(unitpath.target.position.x, unitpath.target.position.y);
		anime.SetBool ("playSide", playside); 
		anime.SetBool ("playBack", playback);

		diffx = me.x - target.x;
		diffy = me.y - target.y;

		if (Mathf.Abs (diffx) > Mathf.Abs (diffy)) {

			playside = true;

			if (diffx > 0) {

				if (!flip && !attacking) {
					transform.Rotate (Vector3.up, 180);
					flip = true;
				}//transform.localScale = new Vector3 (-scalex, transform.localScale.y, transform.localScale.z);
			}
			else {
				//transform.localScale = new Vector3 (scalex, transform.localScale.y, transform.localScale.z);
				if (flip&& !attacking) {
					transform.Rotate (Vector3.up, 180);
					flip = false;
				}
			}

		}
		else {
			playside = false;

			if (diffy < 0) {

				playback = true;

			}
			else {

				playback = false;

			}
		}
	}

	void AmPumpkin(){
		unitpath.StopAllCoroutines();
		anime.SetBool ("ampumpkin", !monster);

	}


	void LateUpdate(){

	}


	/* During day/night night/day transition the DayNightSystem script sends s message. This handles thate message*/
	public void ChangeForm(){
		if (monster) {
			monster = false;
			anime.SetBool("ampumpkin", !monster);
		}
		else {

			monster = true;
			anime.SetBool ("ampumpkin", !monster);

		}

	}

	/* check if currently am a monster*/
	public bool MonsterCheck(){

		if (monster) {
			/* I am a monster*/
			return true;
		}
		/* I am not a monster*/
		return false;
	}


	/* Move ai to random point in vicinity of where I am now*/
	public void MoveToRandom(){
		if (!newpathcd && !attacking) {
			float x, y;
			Vector2 pos;
			var randompoint = new GameObject ().transform;
			Grid grid =(Grid) GameObject.FindGameObjectWithTag ("A_").GetComponent ("Grid");
			do {

				x = transform.position.x + Random.Range (-randompointlimit, randompointlimit);
				y = transform.position.y + Random.Range (-randompointlimit, randompointlimit);
				pos = new Vector2 (x, y);


			} while (grid.NodeFromWorldPoint ((Vector3)pos).walkable != true) ;

			randompoint.position = pos;
			unitpath.target = randompoint;
			target = randompoint.position;
			ChangePath ();
			newpathcd = true;

			Invoke ("NewPathCd", 4f);
		}
	}

	/* Move Ai to player()*/
	public void MoveToPlayer(){
		/* Only want to change path every so often as it is expensive*/
		if (!newpathcd && !attacking) {
			unitpath.target = player;
			ChangePath ();
			target = player.position;
			newpathcd = true;
			Invoke ("NewPathCd", 1f);

		}

	}

	/* Move back towards our Spawn because some how weve gone to far*/
	/* having issues*/
	public void MoveToSpawn(){

		var randompoint = new GameObject().transform;
		randompoint.position = spawnpos;
		unitpath.target = randompoint;
		target = randompoint.position;
		ChangePath ();
		newpathcd = true;
		Invoke ("NewPathCd", 1f);

	}

	public void Attack(){
		if (!attacking) {
			unitpath.StopAllCoroutines ();

			if (Mathf.Abs (diffx) > Mathf.Abs (diffy)) {
				anime.SetTrigger ("playSideAtk");
			}
			else {
				if (playback == true) {
					anime.SetTrigger ("playBackAtk");
				}
				else {
					anime.SetTrigger ("playFrontAtk");
				}

			}

			attacking = true;
			newpathcd = true;

			Invoke ("AttackCD", 6f);
		}

	}


	private void AttackCD(){
		attacking = false;
		Invoke ("NewPathCd",0f);

	}

	public void Idle(){
		if (!newpathcd) {
			unitpath.target = transform;
			ChangePath ();
			newpathcd = true;
			Invoke ("NewPathCd", 1f);
		}
	}
	/* This provieds generic random node use. Use randombal as a balance point as to where*/
	public bool RandomCheckSpawnRange(){
		int x = Random.Range (0,101);

		return (x > randombal_spawn);
	}

	public bool RandomStrollorIdleCheck(){
		int x = Random.Range (0,101);

		return (x > randombal_strolloridle);
	}

	/*Agro range check*/
	public bool AgroRangeCheck(){
		dist = Vector2.Distance ((Vector2)player.position, (Vector2)transform.position);
		if (dist < agrorange) {
			unitpath.target = player;
			return true;
		}
		return false;
	}

	public bool AttackRangeCheck(){

		if (dist < attackrange) {
			return true;
		}
		return false;

	}


	public LayerMask losmask;
	public Vector2 lastknown = new Vector2();
	public bool LOSCheck(){

		Vector2 raydir = player.position - transform.position;

		RaycastHit2D hit = Physics2D.Raycast (transform.position,raydir, 20f,losmask);

		Debug.DrawRay (transform.position, raydir, Color.red);
		if (hit.collider==null) {
			lastknown = new Vector2 (player.position.x, player.position.y);

			return true;
		}
		return false;


	}


	public bool LastKnownCheck(){
		target = lastknown;
		if ((Vector2.Distance(lastknown, (Vector2)transform.position)>.5)) {
			return true;
		}

		return false;

	}

	/*shaky cuz of circle may be better to make an x*/
	public bool CheckForBox(){
		Collider2D coll = Physics2D.OverlapCircle (transform.position, 2);
		if (coll!=null && coll.tag == "BOX"){
			return true;
		}
		else{
			return false;
		}


	}

	public void AttackBox(){
		Collider2D coll = Physics2D.OverlapCircle (transform.position, 1);
		target = coll.transform.position;
		diffx = me.x - target.x;
		diffy = me.y - target.y;
		Attack ();

	}	




	public void MoveToLastKnown(){

		if(!attacking && !newpathcd) {
			var lktrans = new GameObject ().transform;
			lktrans.position = lastknown;
			unitpath.target = lktrans;

			ChangePath ();
			newpathcd = true;
			Invoke ("NewPathCd", 3f);
		}

	}
	public Item[] dropList;

	private void Die(){

		GameObject dropPrefab = Resources.Load ("Pickup") as GameObject;
		GameObject curDrop;

		for (int i = 0; i < dropList.Length; i++) {
			curDrop = GameObject.Instantiate (dropPrefab, transform.position, Quaternion.identity) as GameObject;
			curDrop.GetComponent<Pickup> ().item = dropList [i];
		}


		Destroy (gameObject);
	}
	public float damage=5;
	public float force=1000f;
	void OnCollisionEnter2D(Collision2D col){


		if (col.gameObject.tag == "Player") {
			Vector2 direction = transform.position - col.transform.position;

			col.gameObject.GetComponent<Rigidbody2D> ().AddForce (-direction * force);

			if (monster) {
				col.gameObject.SendMessage ("removeHealth", damage);
			}

			AttackCD ();
		}
	}

	public int hp = 5;
	private void TakeDamage(int dmg){
		hp -= dmg;
		StartCoroutine ("DamageFlash");
		if (hp <= 0) {
			Die ();	
		}
	}

	/** Temporarily flashes red to indicate the pumpkin has taken damage */
	private IEnumerator DamageFlash(){
		GetComponent<SpriteRenderer> ().color = Color.red;
		yield return new WaitForSeconds (0.1f);
		GetComponent<SpriteRenderer> ().color = Color.white;
	}
}
