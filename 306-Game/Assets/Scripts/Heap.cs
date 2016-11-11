using UnityEngine;
using System.Collections;
using System;

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}

public class Heap<T> where T : IHeapItem<T> {
	

	T[] items;
	int currentItemCount;


	public Heap(int maxHeapSize){
		items = new T[maxHeapSize];

	}
	public int Count {
		get {
			return currentItemCount;
		}
	}
	public void Add (T item){
		item.HeapIndex = currentItemCount;
		items [currentItemCount] = item;
		SortUp (item);
		currentItemCount++;
	}

	public T RemoveFirst(){
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item){
		SortUp (item);
	}


	public bool Contains (T item){
		return Equals (items [item.HeapIndex], item);
	}

	void SortDown (T item){
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapindex = 0;
			if (childIndexLeft < currentItemCount) {
				swapindex = childIndexLeft;
				if (childIndexRight < currentItemCount) {
					if (items [childIndexLeft].CompareTo (items [childIndexRight] )< 0) {
						swapindex = childIndexRight;
					}
				}


				
				if (item.CompareTo (items [swapindex]) < 0) {
					Swap (item, items [swapindex]);
				} 
				else {
					return;
				}
			} 
			else {
				return;
			}
		}
	}


	void SortUp (T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parentItem = items [parentIndex];
			//if it has a higher priority, it has a lower fcost
			if (item.CompareTo (parentItem) > 0) {
				Swap (item, parentItem);
			}
			else {
				break;
			}
		
			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void Swap(T a, T b){
		items[a.HeapIndex] = b;
		items[b.HeapIndex] = a;
		int aindex = a.HeapIndex;

		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = aindex;
	}

}
