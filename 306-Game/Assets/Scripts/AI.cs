using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

	public float randompointlimit = 20f;
	public int randombal_spawn = 50;

	UnitPath unitpath;

	bool newpathcd;
	bool monster = true;
	Vector2 spawnpos;
	private Rigidbody2D rb;
	private Animator anime;
	private SpriteRenderer sprite;

	private DecisionTree ai = new DecisionTree();
	private DecisionTreeNode node_monstercheck = new DecisionTreeNode ();
	private DecisionTreeNode node_spawnrangecheck = new DecisionTreeNode();
	private DecisionTreeNode node_inrangecheck= new DecisionTreeNode();
	private DecisionTreeNode node_movetoplayer = new DecisionTreeNode();
	private DecisionTreeNode node_randomcheck = new DecisionTreeNode();
	private DecisionTreeNode node_movetorandom = new DecisionTreeNode();



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

	}

	/* Build my decision tree at spawn in start (awake?)*/
	void BuildDecisionTree(){
		ai.root = node_monstercheck;

		node_monstercheck.decdel = MonsterCheck;
		node_monstercheck.left = node_movetoplayer;
		node_monstercheck.right = node_movetorandom;

		node_movetoplayer.actdel = MoveToPlayer;
		/*node_movetorandom.actdel = MoveToRandom;
		node_movetorandom.decdel = RandomCheckSpawnRange;*/
		node_movetorandom.actdel =MoveToPlayer;
	}

	/* Changes the path in UnitPath script*/
	void ChangePath(){
		unitpath.SendMessage ("RestartPath");
	}

	Vector2 me;
	private Vector2 target;
	private bool playside, playback;
	float diffx, diffy;
	void Update(){
		me = transform.position;
		target = (Vector2) unitpath.target.position;
		ai.Search(ai.root);
		anime.SetBool ("playSide", playside); 
		anime.SetBool ("playBack", playback);

		diffx = me.x - target.x;
		diffy = me.y - target.y;
		if (Mathf.Abs (diffx) > Mathf.Abs (diffy)) {
			playside = true;
			if (diffx > 0) {
				sprite.flipX = true;
			}
			else {
				sprite.flipX = false;
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
	void ChangeForm(){
		if (monster) {
			monster = false;
		}
		else {
			monster = true;
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
		if (!newpathcd) {
			float x, y;
			Vector2 pos;
		;
			var randompoint = new GameObject().transform;
			x = transform.position.x +Random.Range(-randompointlimit,randompointlimit);
			y = transform.position.y +Random.Range (-randompointlimit, randompointlimit);
			pos = new Vector2 (x, y);
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
		if (!newpathcd) {
			unitpath.target = GameObject.FindGameObjectWithTag ("Player").transform;
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

	

}
