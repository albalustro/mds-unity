using System.Collections;
using System.Collections.Generic;
using MDS.Core.Interfaces;
using MDS.Gameplay.DragDrop;
using MDS.Utilities;
using UnityEngine;

public class DraggableFactory : MDSBehaviour
{


    [SerializeField]
    private Draggable[] _draggablePrefabs;

    [SerializeField]
    private DropGroupSlot _slotPrefab;

    [SerializeField]
    private Transform _instantiatePositionRef;

    public IEnumerator Start()
    {

        while(true)
        {
            yield return new WaitForSeconds(3);

            var drag = Instantiate(_draggablePrefabs.GetRandom());
            drag.transform.position = _instantiatePositionRef.position;
            drag.instantiableDraggable = true;

            var slot = Instantiate(_slotPrefab, drag.transform, false);
            slot.GetComponent<Collider2D>().enabled = false;

            drag.currentSlot = slot;
            slot.draggableReference = drag;

        }

    }
}
