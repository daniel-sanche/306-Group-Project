using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BugAI : MonoBehaviour {

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
	bool monster = true;
	Vector2 spawnpos;
	private Rigidbody2D rb;
	private Animator anime;
	private SpriteRenderer sprite;
	private Grid grid;


	private DecisionTree ai = new DecisionTree();
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


	private void NewPathCd(){
		newpathcd = false;
	}

	void Awake(){
		spawnpos = new Vector2 (transform.position.x, transform.position.y);


	}
	void Start () {
		unitpath = GetComponent<UnitPath> ();
		unitpath.target = GameObject.FindGameObjectWithTag ("Player").transform;
		rb = GetComponent<Rigidbody2D> ();
		BuildDecisionTree ();
		anime = gameObject.GetComponent<Animator> ();
		sprite = gameObject.GetComponent<SpriteRenderer> ();
		localscalex = transform.localScale;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	/* Build my decision tree at spawn in start (awake?)*/
	void BuildDecisionTree(){
		ai.root = node_monstercheck;

		node_monstercheck.decdel = MonsterCheck;
		node_monstercheck.left = node_agrocheck;
		node_monstercheck.right = node_ampumpkin;

		node_ampumpkin.actdel = AmPumpkin;

		node_agrocheck.decdel = AgroRangeCheck;
		node_agrocheck.left = node_attackcheck;
		node_agrocheck.right = node_randomstrolloridlecheck;

		node_attackcheck.decdel = AttackRangeCheck;
		node_attackcheck.left = node_attack;
		node_attackcheck.right = node_movetoplayer;


		node_randomstrolloridlecheck.decdel = RandomStrollorIdleCheck;
		node_randomstrolloridlecheck.left = node_movetorandom;
		node_randomstrolloridlecheck.right = node_idle;

		node_idle.actdel = Idle;

		node_attack.actdel = Attack;


		node_movetoplayer.actdel = MoveToPlayer;
		/*node_movetorandom.actdel = MoveToRandom;
		node_movetorandom.decdel = RandomCheckSpawnRange;*/
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
	}

	void AmPumpkin(){
		unitpath.StopAllCoroutines();
		//anime.SetBool ("ampumpkin", !monster);
	}


	void LateUpdate(){
		me = transform.position;
		target = (Vector2) unitpath.target.position;
	//	anime.SetBool ("playSide", playside); 
	//	anime.SetBool ("playBack", playback);

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


	/* During day/night night/day transition the DayNightSystem script sends s message. This handles thate message*/
	public void ChangeForm(){
		if (monster) {
			monster = false;
		}
		else {
			monster = true;
			//anime.SetBool ("ampumpkin", !monster);
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

			Grid grid =(Grid) GameObject.FindGameObjectWithTag ("A*").GetComponent ("Grid");

			do {

				x = transform.position.x + Random.Range (-randompointlimit, randompointlimit);
				y = transform.position.y + Random.Range (-randompointlimit, randompointlimit);
				pos = new Vector2 (x, y);

			} while (grid.NodeFromWorldPoint ((Vector3)pos).walkable != true) ;

			randompoint.position = pos;
			unitpath.target = randompoint;
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
		ChangePath ();
		newpathcd = true;
		Invoke ("NewPathCd", 1f);

	}

	public void Attack(){
		if (!attacking) {
			unitpath.StopAllCoroutines ();
			unitpath.target = player;
			ChangePath ();
			hitPlayerInCone ();
			newpathcd = true;
			attacking = true;
			Invoke ("AttackCD", 3f);
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
	public Vector2 lastknown = Vector2.zero;
	public bool LOSCheck(){

		Vector2 raydir = player.position - transform.position;



		if (Physics2D.Raycast (transform.position, raydir, losmask)== null) {
			lastknown = new Vector2 (player.position.x, player.position.y);
			//unitpath.target = player;
			return true;
		}
		else {
			/*		var lktrans = new GameObject ().transform;
			lktrans.position = lastknown;
			unitpath.target = lktrans;
	*/		return false;


		}


	}

	public void MoveToLastKnown(){
		if (!attacking && !newpathcd && lastknown!=null) {
			var lktrans = new GameObject ().transform;
			lktrans.position = lastknown;
			unitpath.target = lktrans;
		}

	}





	//Returns the radian representation of the angle of a gameobject to the player
	protected float getRelativeAngle(){

		Vector2 relativePos =  player.transform.position - transform.position;							//Gets the position of the GameObject in relation to the player;

		return Mathf.Atan2 (relativePos.y, relativePos.x);															//Calculate the angle of the GameObject to the player.
	}

	//Gets the enemies in a cone with the given angle, swingAngle, and swingRadius
	private void hitPlayerInCone(){
		if (Vector2.Distance (transform.position, player.transform.position) < attackrange) {
			player.GetComponent<HealthEnergy>().TakeDamage(5f);
		}

	}

	/** The enemy's health */
	public int health;

	/** Takes damage */
	public void damage(int toTake){
		health -= toTake;
		StartCoroutine ("DamageFlash");
	}

	/** Temporarily flashes red to indicate the player has taken damage */
	private IEnumerator DamageFlash(){
		GetComponent<SpriteRenderer> ().color = Color.red;
		yield return new WaitForSeconds (0.1f);
		GetComponent<SpriteRenderer> ().color = Color.white;
	}
}
