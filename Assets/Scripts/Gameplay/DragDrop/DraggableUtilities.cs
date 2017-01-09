using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDS.Gameplay.DragDrop
{
    public class DraggableUtilities
    {

        public static void SetDraggableInSlot(Draggable draggable, DropGroupSlot slot, float duration = 0.5f)
        {
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
                    oldDraggable.TweenGoto(oldSlot.transform.position, duration);
                }
            }

            slot.draggableReference = draggable;
            draggable.currentSlot = slot;
            draggable.TweenGoto(slot.transform.position, duration);
        }

    }
}
