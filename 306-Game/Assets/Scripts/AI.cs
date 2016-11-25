using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

	public float randompointlimit = 20f;


	UnitPath unitpath;
	Transform target;
	bool newpathcd;
	bool monster = true;


	private DecisionTree ai = new DecisionTree();
	private DecisionTreeNode node_monstercheck = new DecisionTreeNode ();
	private DecisionTreeNode node_spawnrangecheck = new DecisionTreeNode();
	private DecisionTreeNode node_inrangecheck= new DecisionTreeNode();
	private DecisionTreeNode node_movetoplayer = new DecisionTreeNode();
	private DecisionTreeNode node_randomcheck = new DecisionTreeNode();
	private DecisionTreeNode node_movetorandom = new DecisionTreeNode();


	// Use this for initialization
	private void NewPathCd(){
		newpathcd = false;
	}

	void Awake(){
		BuildDecisionTree ();

	}
	void Start () {
		unitpath = GetComponent<UnitPath> ();
		unitpath.target = GameObject.FindGameObjectWithTag ("Player").transform;
		InvokeRepeating ("ChangePath", 1f, 1f);

	}

	void BuildDecisionTree(){
		ai.root = node_monstercheck;

		node_monstercheck.decdel = MonsterCheck;
		node_monstercheck.left = node_movetoplayer;
		node_monstercheck.right = node_movetorandom;

		node_movetoplayer.actdel = MoveToPlayer;
		node_movetorandom.actdel = MoveToRandom;
	}
	void ChangePath(){
		unitpath.SendMessage ("RestartPath");


	}

	void Update(){
		ai.Search(ai.root);
	}





	void ChangeForm(){
		if (monster) {
			monster = false;
		}
		else {
			monster = true;
		}

	}

	/* check if currently a monster*/
	public bool MonsterCheck(){
		
		if (monster) {
			/* I am a monster*/
			return true;
		}
		/* I am not a monster*/
		return false;
	}


	/* Move ai to random point in vicinity*/
	public void MoveToRandom(){
		if (!newpathcd) {
			float x, y;
			Vector2 pos;
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
}
