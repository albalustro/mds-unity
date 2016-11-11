using UnityEngine;
using System.Collections;
using System.Linq;

namespace MDS.Gameplay.DragDrop
{
    public class StackDropGroupArea : ValidatableDropGroupArea
    {

        public override bool SetInSlot(Draggable draggable, ref DropGroupSlot slot)
        {
            bool ret = false;
            Draggable oldDrag;

            if (slot.IsTaken)
            {
                oldDrag = slot.draggableReference;
            }

            slot = slots.FirstOrDefault(s => s.IsTaken == false);

            if(slot != null)
            {
                ret = base.SetInSlot(draggable, ref slot);

                int inicialIndex = slots.IndexOf(slot);

                for(int i = inicialIndex; i < slots.Count; i++)
                {

                }
            }

            return ret;
        }
    }

}