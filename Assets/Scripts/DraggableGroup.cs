using UnityEngine;
using System.Collections;
using System.Linq;

public class DraggableGroup : MDSBehaviour {

    public DraggableGroupSlot[] slots;

    public bool GetValidPosition(ref Vector3 position, Draggable draggable)
    {
        DraggableGroupSlot freeSlot = slots.FirstOrDefault(s => s.IsBusy == false);

        if(freeSlot == null) return false;

        freeSlot.draggableReference = draggable;

        position = freeSlot.transform.position;

        return true;

    }
}
