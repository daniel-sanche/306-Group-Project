using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

	UnitPath unitpath;

	// Use this for initialization
	void Start () {
		unitpath = GetComponent<UnitPath> ();
		InvokeRepeating ("ChangePath", 1f, 1f);
	}

	void ChangePath(){
		unitpath.SendMessage ("RestartPath");


	}
}
