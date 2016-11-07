using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using UnityEngine;

namespace MDS.Gameplay.DragDrop
{
    public class InitialIntanceDropGroupArea : InitialDropGroupArea
    {

        public override void Start()
        {
            base.Start();

            foreach(var slot in slots)
            {
                if (slot.draggableReference!=null)
                {
                    slot.draggableReference.OnDrop += Draggable_OnDrop;
                    if (slot.draggableReference.PreferredInitialIndex.HasValue==false)
                    {
                        Debug.LogError("Para um InitialIntanceDropGroupArea, todos os Draggables precisam ter um PreferredInitialIndex");
                    }

                }
                else
                {
                    Debug.LogError("Para um InitialIntanceDropGroupArea, todos os slots precisam estar preenchidos");
                }
            }
        }

        private void Draggable_OnDrop(Draggable draggable, DropGroupSlot slot)
        {
            if(slot.IsInitialSlot)
            {
                Destroy(draggable);
                return;
            }

            Draggable newDraggable = Instantiate(draggable);
            newDraggable.OnDrop += Draggable_OnDrop;
            DraggableUtilities.SetDraggableInSlot(newDraggable, slots[newDraggable.PreferredInitialIndex.Value]);
        }

    }
}
