using UnityEngine;
using System.Collections;

public class AItestScript : MonoBehaviour {

	public float aggrorange = 30f;

	public DecisionTree ai;
	public DecisionTreeNode node_lungedecider = new DecisionTreeNode();
	public DecisionTreeNode node_moveto = new DecisionTreeNode();
	public DecisionTreeNode node_stroll = new DecisionTreeNode();
	public DecisionTreeNode node_lunge = new DecisionTreeNode();
	private Vector2 mousepos;
	private Vector2 myVector;
	private Rigidbody2D rb;
	private bool isday = true;
	bool b = true;
	private bool lungecd = true;
	public object player;


	void Awake(){
		ai = new DecisionTree ();

	}
	// Use this for initialization
	void Start () {
		
		rb = GetComponent<Rigidbody2D> ();
		BuildDecisionTree ();


	}
	
	// Update is called once per frame
	void Update () {
		mousepos = new Vector2 (Camera.main.ScreenToWorldPoint( Input.mousePosition).x, Camera.main.ScreenToWorldPoint( Input.mousePosition).y);
		myVector = new Vector2 (transform.position.x, transform.position.y);
		ai.Search (ai.root);
		Debug.DrawRay (transform.position, mousepos- myVector, Color.red);


	}


	public bool NightDayCheck(){
		if (isday) {
			return true;
		}
		return false;
	}

	public bool ThreatZoneCheck(){
		//distance check
		return false;
	}


	public bool LOSCheck(){
		//loscheck
		return false;
	}

	public bool RangeCheck(){
		//distancecheck
		return false;

	}

	public void Attack(){
		/*trigger animation
		 * Adda force if lunging*/

	}

	public void Advance(){
		//advance to target
	}

	public bool RandomCheck(){
		//random check
		return false;
	}

	public void Stroll(){
		//move to random point with pathfinding
	}

	public void Talk(){
		//Make a talk bubble, play sound effect?
	}
		

	public bool SpawnRangeCheck(){
		//spawndistance check
		return false;
	}

	public bool HaveItemCheck(){
		//check if has item
		return false;
	}

	public void DropItem(){
		//dropitem around this as near player
	}

	public void Sayhello(){
		//make a talk bubble, play sound effect?
	}



	void BuildDecisionTree(){


	}


}
