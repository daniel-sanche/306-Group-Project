using UnityEngine;
using System.Collections;
using System;

public class DecisionTreeNode{
	public DecisionTreeNode right;
	public DecisionTreeNode left;
	public Decision decdel;
	public Action actdel;

	/* For Insertion*/
	public int value;
}

public delegate bool Decision();
public delegate void Action();


public class DecisionTree : MonoBehaviour {
	


	public DecisionTreeNode root;
	/* useful for inserting nodes*/
	public int mid = 50;
	/*Constructor*/
	public  DecisionTree(){
	
		root = new DecisionTreeNode();
		root.value = mid;


	}

	public void Insert(DecisionTreeNode newnode, DecisionTreeNode parent){
		if (newnode == null || parent ==null) {
			throw new Exception ("INSERT: newnode is null or parent is null");
		}

		if (parent.value > newnode.value) {
			if (parent.left == null) {
				parent.left = newnode;
			} else {
				Insert (newnode, parent.left);
			}
		}
		else {
			if (parent.right == null) {
				parent.right = newnode;
			}
			else {
				Insert (newnode, parent.right);
			}
		}	

	}

	public void Search(DecisionTreeNode node){

		/*Something has gone wrong*/
		if (node == null) {
			throw new Exception("ERROR: Search node null");

		}
		/*Perform the action*/
		if (node.actdel!= null) {
			node.actdel ();
			return;
		}

		/*Perform decision*/
		if (node.decdel() == true) {
			Search(node.left);
		} 
		else {
			 Search (node.right); 
		}

	}
	
}
