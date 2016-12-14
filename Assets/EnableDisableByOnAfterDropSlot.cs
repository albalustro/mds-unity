using System.Collections;
using System.Collections.Generic;
using MDS.Gameplay.DragDrop;
using UnityEngine;

public class EnableDisableByOnAfterDropSlot : MDSBehaviour {

	public void Check(Draggable draggable, DropGroupSlot originSlot)
    {
        GetComponent<Renderer>().enabled = !draggable.currentSlot.IsInitialSlot;
    }

}
