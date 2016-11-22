using UnityEngine;
using System.Collections;
using System.Linq;
using System;

namespace MDS.Gameplay.DragDrop
{
    public class StackDropGroupArea : ValidatableDropGroupArea
    {

        protected override void Awake()
        {
            base.Awake();
        }

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
            if (slots.Any(s => s.draggableReference == draggable))
                return false;
            else
            {
                if (base.SetInSlot(draggable, ref slot))
                {
                    draggable.OnAfterDrop += DraggableAfterDropHandler;
                }
            }
            return ret;
        }

        private void DraggableAfterDropHandler(Draggable draggable, DropGroupSlot originalSlot)
        {
            if (slots.Any(s => s.draggableReference == draggable))
            {
                return;
            }
            else
            {
                draggable.OnAfterDrop -= DraggableAfterDropHandler;
                OrganizeStack();
            }
        }

        private void OrganizeStack()
        {
            DropGroupSlot freeSlot = null;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].draggableReference == null)
                {
                    freeSlot = slots[i];
                    if (slots[i + 1].draggableReference != null)
                    {
                        Draggable d = slots[i + 1].draggableReference;
                        DraggableUtilities.SetDraggableInSlot(d, freeSlot);
                    }
                }
            }
        }
    }

}