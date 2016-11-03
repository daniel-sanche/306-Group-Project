using UnityEngine;
using System.Collections;

public class AItestScript : MonoBehaviour {

	public float aggrorange = 30f;

	private DecisionTree ai;
	private Node testleft = new Node();
	private Node testright = new Node();
	private Node testleftright = new Node();
	private Vector2 mousepos;
	private Vector2 myVector;

	public object player;
	void Awake(){
		ai = new DecisionTree ();

	}
	// Use this for initialization
	void Start () {

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
		testleft.value = 25;
		testleft.decdel = wentLeft;
		testright.actdel = wentRight;
		testright.value = 75;
		testleftright.value = 30;
		testleftright.actdel = wentLeftRight;
		ai.Insert (testleft,ai.root);
		ai.Insert (testright, ai.root);
		ai.Insert (testleftright, ai.root);
	}

	public bool wentLeft(){
		print ("Went Left\n");
		return false;
	}
	public void wentLeftRight(){
		print ("Went LeftRIght\n");
	}

	public void wentRight(){
		print ("Went Right\n");
	}


	public bool RangeCheck(){
		if (Vector2.Distance(mousepos, myVector) < aggrorange) {
			Debug.Log ("In range");
			return true;
		}

		return false;
	}
	public bool LungeDecider(){


		int foo = Random.Range (0, 20);
		if (foo > 20) {
			return true;
		}
		return false;
	}		

}
