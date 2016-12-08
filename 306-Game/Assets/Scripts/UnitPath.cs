﻿using UnityEngine;
using System.Collections;
/* Credit is due to Sebastian Lague's tutorial on https://www.youtube.com/watch?v=-L-WgKMFuhE&t=2s*/

public class UnitPath : MonoBehaviour {

	public Transform target;
	public float speed = 5;
	Vector3[] path;
	int targetIndex;

	void Start(){
		if (target !=null)PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
	}

	void RestartPath(){
		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

		targetIndex = 0;
		StopCoroutine ("FollowPath");
		StartCoroutine ("FollowPath");
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}


	IEnumerator FollowPath(){
		/*Go to first waypoint*/
		if (path!= null && path.Length > 0) {
			Vector3 currentWaypoint = path [0];

			while (true) {
				if (transform.position == currentWaypoint) {
					targetIndex++;
					if (targetIndex >= path.Length) {
						yield break;
					}
					currentWaypoint = path [targetIndex];
				}

				transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
				yield return null;
			}
		}
	}

	public void OnDrawGizmos(){
		if(path!=null){
			for (int i = targetIndex; i<path.Length; i++){
				Gizmos.color = Color.blue;
				//Gizmos.DrawCube(path[i],Vector3.one);

				if (i ==targetIndex){
					Gizmos.DrawLine(transform.position,path[i]);
				}
				else{
					Gizmos.DrawLine(path[i-1], path[i]);
			
				}
		
			}

		}
	}
}