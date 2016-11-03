using UnityEngine;
using System.Collections;

public class AItestScript : MonoBehaviour {

	public float aggrorange = 30f;

	public DecisionTree ai;
	public Node node_lungedecider = new Node();
	public Node node_moveto = new Node();
	public Node node_stroll = new Node();
	public Node node_lunge = new Node();
	private Vector2 mousepos;
	private Vector2 myVector;
	private Rigidbody2D rb;

	private bool lungecd;
	public object player;
	void Awake(){
		ai = new DecisionTree ();

	}
	// Use this for initialization
	void Start () {
		lungecd = true;
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

	void BuildDecisionTree(){
		ai.root.decdel = RangeCheck;
		/*ai.root.left.actdel = wentLeft;
		ai.root.right.actdel = wentRight;
	*/
		node_lungedecider.value = 25;
		node_lungedecider.decdel = LungeDecider;

		node_lunge.value = 10;
		node_lunge.actdel = Lunge;

		node_moveto.actdel = MoveTo;
		node_moveto.value = 30;

		node_stroll.value = 70;
		node_stroll.actdel = Stroll;

		ai.Insert (node_lungedecider,ai.root);

		ai.Insert (node_moveto, ai.root);
		ai.Insert (node_lunge, ai.root);
		ai.Insert (node_stroll, ai.root);
	
	}

	public void Stroll(){
		//print ("Strolling\n");

	}
	public void MoveTo(){
		//Debug.Log("MOVING");
	}

	public void Lunge(){
		Debug.Log ("LUNGING");
		lungecd = false;
		Invoke ("LungeCD", 5);
	}


	public bool RangeCheck(){
		if (Vector2.Distance(mousepos, myVector) < aggrorange) {
			//Debug.Log ("In range");
			return true;
		}

		return false;
	}
	public bool LungeDecider(){

		if (lungecd == true) {

			int foo = Random.Range (0, 20);
			if (foo >10) {

				return true;
			}
		}

		return false;
	}		

	public void LungeCD(){
		lungecd = true;
	}
}
