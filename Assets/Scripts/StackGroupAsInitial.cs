using System.Collections;
using System.Collections.Generic;
using MDS.Gameplay.DragDrop;
using UnityEngine;

public class StackGroupAsInitial : MDSBehaviour {

	
	void Start () {

		StackDropGroupArea stack = GetComponent<StackDropGroupArea>();

		Draggable[] draggablesInScene = FindObjectsOfType<Draggable>();

		foreach(var drag in draggablesInScene)
		{
			DropGroupSlot dummy = null;
			stack.SetInSlot(drag, ref dummy);
		}

	}
	

}
