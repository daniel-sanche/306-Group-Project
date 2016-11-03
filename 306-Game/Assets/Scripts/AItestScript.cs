using UnityEngine;
using System.Collections;

public class AItestScript : MonoBehaviour {

	private DecisionTree ai;

	public object player;
	void Awake(){
		ai = new DecisionTree ();

	}
	// Use this for initialization
	void Start () {

		BuildDecisionTree ();

		ai.Search (ai.root);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void BuildDecisionTree(){
		ai.root.decdel = RangeCheck;
		ai.root.left.actdel = wentLeft;
		ai.root.right.actdel = wentRight;

	}

	public void wentLeft(){
		print ("Went Left\n");
	}

	public void wentRight(){
		print ("Went Right\n");
	}

	public bool RangeCheck(){


		int foo = Random.Range (0, 20);
		if (foo > 10) {
			return true;
		}
		return false;
	}		

}
