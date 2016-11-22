using UnityEngine;
using System.Collections;
using System.Linq;
using System;

namespace MDS.Gameplay.DragDrop
{
    public class StackDropGroupArea : ValidatableDropGroupArea
    {
        public override void Start()
        {
            base.Start();
            for (int i = 0; i < slots.Count; i++)
            {
                Freeze(slots[i].gameObject);
            }
        }

        public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
        {
            bool ret = false;
            if (!slots.Any(s => s.draggableReference == draggable))
            {
                if (base.SetInSlot(draggable, ref slot))
                {
                    draggable.OnAfterDrop += DraggableAfterDropHandler;
                    ret = true;
                }
            }
            return ret;
        }

        private void DraggableAfterDropHandler(Draggable draggable, DropGroupSlot originalSlot)
        {
            if (!slots.Any(s => s.draggableReference == draggable))
            {
                draggable.OnAfterDrop -= DraggableAfterDropHandler;
                OrganizeStack();
            }
        }

        private void OrganizeStack()
        {
            DropGroupSlot freeSlot = null;
            for (int i = 0; i < slots.Count-1; i++)
            {
                if (slots[i].draggableReference == null)
                {
                    freeSlot = slots[i];
                    if (slots[i + 1].draggableReference == null)
                        return;
                    Draggable d = slots[i + 1].draggableReference;
                    DraggableUtilities.SetDraggableInSlot(d, freeSlot);
                }
            }
        }
    }

}