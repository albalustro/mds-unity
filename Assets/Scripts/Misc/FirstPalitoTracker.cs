using System.Collections;
using System.Collections.Generic;
using MDS.Gameplay.DragDrop;
using UnityEngine;

public class FirstPalitoTracker : MDSBehaviour {

    [SerializeField]
    private DropGroupSlot[] _orderedPalitos;

    public void MemorizeFirstFreePalito()
    {
        for(int i = 0; i < _orderedPalitos.Length; i++)
        {
            if (_orderedPalitos[i].draggableReference==null)
            {
                MemorizeMe.MemorizedGameObject = _orderedPalitos[i].gameObject;
                return;
            }
        }
    }

}
