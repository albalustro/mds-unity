using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DraggableGroupSlot : MDSBehaviour {

    public Draggable draggableReference;

    public string[] Labels;

    public bool IsBusy
    {
        get
        {
            return draggableReference != null;
        }
    }



}
