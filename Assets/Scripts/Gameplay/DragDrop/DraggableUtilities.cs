using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MDS.Gameplay.DragDrop
{
    public class DraggableUtilities
    {

        public static void SetDraggableInSlot(Draggable draggable, DropGroupSlot slot, float duration = 0.5f)
        {
            Vector3 destination;
            DropGroupSlot oldSlot = draggable.currentSlot;
            Draggable oldDraggable = slot.draggableReference;

            if(oldSlot != null) 
            {
                oldSlot.draggableReference = null;

                if(oldDraggable != null)
                {
                    oldSlot.draggableReference = oldDraggable;
                    oldDraggable.currentSlot = oldSlot;
                    oldDraggable.ProcessSlotChanging(slot);

                    destination = oldSlot.transform.position;
                    destination.z = oldDraggable.transform.position.z;
                    oldDraggable.TweenGoto(destination, duration);
                }
            }

            slot.draggableReference = draggable;
            draggable.currentSlot = slot;

            destination = slot.transform.position;
            destination.z = draggable.transform.position.z;
            draggable.TweenGoto(slot.transform.position, duration);
        }

    }
}
