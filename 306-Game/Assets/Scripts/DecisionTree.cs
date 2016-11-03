using UnityEngine;
using System.Collections;
using System;


public class DecisionTree : MonoBehaviour {
	public delegate bool Decision();
	public delegate void Action();

	public class Node{
		public Node right;
		public Node left;
		public Decision decdel;
		public Action actdel;



	}



	public Node root;

	/*Constructor*/
	public  DecisionTree(){
		root = new Node();
		root.right = new Node ();
		root.left = new Node ();

	

	}



	public void Search(Node node){

		/*Something has gone wrong*/
		if (node == null) {
			throw new Exception("I am an error");

		}

		/*Perform the action*/
		if (node.actdel!= null) {
			node.actdel ();
			return;
		}

		/*Perform decision*/
		if (node.decdel()) {
			Search(node.left);
		} 
		else {
			 Search (node.right); 
		}

	}
	
}
