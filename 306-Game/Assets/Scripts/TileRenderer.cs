using UnityEngine;
using System.Collections;

public class TileRenderer : MonoBehaviour {

	public TileGenerator generator;

	void Awake () {
		generator = GetComponent<TileGenerator> ();
		InitGame();
	}

	void InitGame(){
		generator.GenerateLevel ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
